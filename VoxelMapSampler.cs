// takashicompany.Unity.VoxelMap  ― 港を島ごとに分散させる版
// -----------------------------------------------------------------------------
// * 各島(連続した陸塊)ごとに最大 portsPerIsland 個まで港(赤点)を配置
// * 港全体数は maxPorts で上限。portSpacing で最小距離を確保
// * 上下左右ループする 2D トーラス地図
// -----------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace takashicompany.Unity
{
	public interface IVoxelMap
	{
		public enum Code { Sea, Land }

		public int width { get; }
		public int depth { get; }   // z座標なのでdepth。
		public Code GetCode(int x, int z);
		public float GetHeight(int x, int z);
		public IEnumerable<(int x, int z)> GetPorts();
	}

	// ════════════════════════════════════════════════════════════════════
	#region VoxelMap (Texture2D -> IVoxelMap)
	// ════════════════════════════════════════════════════════════════════
	public class VoxelMap : IVoxelMap
	{
		private readonly int _width;
		private readonly int _depth;
		private readonly IVoxelMap.Code[,] _codes;
		private readonly float[,] _heights;
		private readonly List<(int x, int z)> _ports = new();

		public int width => _width;
		public int depth => _depth;

		/// <summary>
		/// コンストラクタ: テクスチャから海・陸・港・標高を抽出
		/// </summary>
		/// <param name="tex">入力 Texture2D（Read/Write 有効）</param>
		/// <param name="seaColor">海と見なす色 (exact match)</param>
		/// <param name="portColor">港(赤点) と見なす色 (exact match)</param>
		/// <param name="landLowColor">陸低地色 (標高 0 の基準) — 標高推定用</param>
		public VoxelMap(Texture2D tex, Color seaColor, Color portColor, Color landLowColor)
		{
			_width = tex.width;
			_depth = tex.height;
			_codes = new IVoxelMap.Code[_width, _depth];
			_heights = new float[_width, _depth];

			// 前計算: sea/port 色を直比較できるよう Color32
			Color32 seaC = seaColor;
			Color32 portC = portColor;
			Color32 landBase = landLowColor;

			for (int z = 0; z < _depth; z++)
				for (int x = 0; x < _width; x++)
				{
					Color32 c = tex.GetPixel(x, z);
					if (EqualsColor(c, portC))
					{
						_codes[x, z] = IVoxelMap.Code.Land;
						_ports.Add((x, z));
						_heights[x, z] = 0.5f; // 港セルは海抜 0.5 (任意)
					}
					else if (EqualsColor(c, seaC))
					{
						_codes[x, z] = IVoxelMap.Code.Sea;
						_heights[x, z] = 0f;
					}
					else // 陸
					{
						_codes[x, z] = IVoxelMap.Code.Land;
						_heights[x, z] = EstimateHeight(c, landBase);
					}
				}
		}

		// 高さ取得 0‑1
		public float GetHeight(int x, int z) => _heights[Mod(x, _width), Mod(z, _depth)];
		public IVoxelMap.Code GetCode(int x, int z) => _codes[Mod(x, _width), Mod(z, _depth)];
		public IEnumerable<(int x, int z)> GetPorts() => _ports;

		// ────────────────────────────────────────────────────────────
		#region Utility
		private static bool EqualsColor(Color32 a, Color32 b) => a.r == b.r && a.g == b.g && a.b == b.b;
		private static int Mod(int a, int m) => ((a % m) + m) % m;

		// RGB → 輝度推定 (ITU‑R BT.709) から landLowColor との差で 0‑1
		private static float EstimateHeight(Color32 c, Color32 landBase)
		{
			float y = 0.2126f * c.r / 255f + 0.7152f * c.g / 255f + 0.0722f * c.b / 255f;
			float y0 = 0.2126f * landBase.r / 255f + 0.7152f * landBase.g / 255f + 0.0722f * landBase.b / 255f;
			return Mathf.Clamp01((y - y0) / (1f - y0));
		}
		#endregion
	}
	#endregion

	[System.Serializable]
	public class VoxelMapGenerator
	{
		// ───────── 地形パラメータ ─────────
		[Header("Map Settings")]
		public int width = 256;
		public int height = 256;
		public int depth => height;
		[Range(0f, 1f)] public float seaLevel = 0.45f;
		[Range(0.5f, 10f)] public float frequency = 2.5f;
		[Range(1, 8)] public int octaves = 6;
		public float persistence = 0.45f;
		public int seed = 1337;
		[Range(0, 3)] public int blurPasses = 1;

		// ───────── カラー設定 ─────────
		[Header("Color Settings")]
		public Color seaColor = new Color32(0, 0, 255, 255);
		public Color landLowColor = new Color32(0, 255, 0, 255);
		public Color landHighColor = Color.white;
		public Color portColor = Color.red;

		// ───────── 港設定 ─────────
		[Header("Port Settings")]
		[Tooltip("最大港数 (赤点の数)")]
		public int maxPorts = 80;
		[Tooltip("1つの島あたりの最大港数")]
		public int portsPerIsland = 1;
		[Tooltip("港間の最小セル距離 (トーラス距離)")]
		public int portSpacing = 6;

		// ------------------------------------------------------------------
		private IVoxelMap.Code[,] _map;
		private float[,] _heights;
		private bool[,] _ports;
		private Texture2D _tex;
		public Texture2D tex => _tex;
		private SeamlessNoise _noise;
		private Vector4[] _octaveRot;
		private readonly int[] _dx = { 1, -1, 0, 0 };
		private readonly int[] _dz = { 0, 0, 1, -1 };

		public float GetHeight(int x, int z) => _heights[x.Mod(width), z.Mod(height)];
		public IVoxelMap.Code GetCode(int x, int z) => _map[x.Mod(width), z.Mod(height)];
		public bool IsPort(int x, int z) => _ports[x.Mod(width), z.Mod(height)];

		// ------------------------------------------------------------------

		// ------------------------------------------------------------------
		public void Generate()
		{
			_noise = new SeamlessNoise(seed);
			_octaveRot = BuildRotations(seed, octaves);
			_map = new IVoxelMap.Code[width, height];
			_heights = new float[width, height];
			_ports = new bool[width, height];

			_tex = new Texture2D(width, height, TextureFormat.RGB24, false)
			{
				filterMode = FilterMode.Bilinear,
				wrapMode = TextureWrapMode.Repeat,
			};

			GenerateHeights();
			for (int p = 0; p < blurPasses; p++) BlurLand();
			PlacePortsIslandAware();
			PaintTexture();
		}

		// ------------------------------------------------------------------
		private void GenerateHeights()
		{
			for (int z = 0; z < height; z++)
			{
				float v = (float)z / height;
				float phi = v * Mathf.PI * 2f;
				float cPhi = Mathf.Cos(phi); float sPhi = Mathf.Sin(phi);
				for (int x = 0; x < width; x++)
				{
					float u = (float)x / width;
					float the = u * Mathf.PI * 2f;
					float cThe = Mathf.Cos(the); float sThe = Mathf.Sin(the);
					float nx0 = cThe, ny0 = sThe, nz0 = cPhi, nw0 = sPhi;
					double e = 0, amp = 1, freq = frequency;
					for (int o = 0; o < octaves; o++)
					{
						Vector4 r = _octaveRot[o];
						double nx = nx0 * r.x + ny0 * r.y + nz0 * r.z + nw0 * r.w;
						double ny = nx0 * r.y - ny0 * r.x + nz0 * r.w - nw0 * r.z;
						double nz = nx0 * r.z - ny0 * r.w - nz0 * r.x + nw0 * r.y;
						double nw = nx0 * r.w + ny0 * r.z - nz0 * r.y - nw0 * r.x;
						e += amp * _noise.Evaluate(nx * freq, ny * freq, nz * freq, nw * freq);
						amp *= persistence; freq *= 2.0;
					}
					float ef = (float)(e * 0.5 + 0.5);
					bool land = ef > seaLevel;
					_map[x, z] = land ? IVoxelMap.Code.Land : IVoxelMap.Code.Sea;
					_heights[x, z] = land ? (ef - seaLevel) / (1f - seaLevel) : 0f;
				}
			}
		}

		// ------------------------------------------------------------------
		private void PlacePortsIslandAware()
		{
			int[,] islandId = new int[width, height];
			for (int z = 0; z < height; z++) for (int x = 0; x < width; x++) islandId[x, z] = -1;
			List<List<(int x, int z)>> islandCoasts = new();
			int currentId = 0;
			// ① 島ごとに連結判定 (BFS)
			for (int z = 0; z < height; z++)
				for (int x = 0; x < width; x++)
				{
					if (_map[x, z] == IVoxelMap.Code.Land && islandId[x, z] == -1)
					{
						var queue = new Queue<(int, int)>();
						queue.Enqueue((x, z));
						islandId[x, z] = currentId;
						var coastList = new List<(int, int)>();
						while (queue.Count > 0)
						{
							var (cx, cz) = queue.Dequeue();
							bool isCoast = false;
							for (int k = 0; k < 4; k++)
							{
								int nx = (cx + _dx[k]).Mod(width);
								int nz = (cz + _dz[k]).Mod(height);
								if (_map[nx, nz] == IVoxelMap.Code.Sea) isCoast = true;
								else if (islandId[nx, nz] == -1)
								{
									islandId[nx, nz] = currentId;
									queue.Enqueue((nx, nz));
								}
							}
							// 沿岸かつ周囲の陸が2セル以上 => 少し内陸寄りで海上に見えにくい
							if (isCoast && LandNeighborCount(cx, cz) >= 2) coastList.Add((cx, cz));
						}
						islandCoasts.Add(coastList);
						currentId++;
					}
				}
			// ② ランダム配置
			var rng = new System.Random(seed * 59);
			var islandIndices = new List<int>(); for (int i = 0; i < islandCoasts.Count; i++) islandIndices.Add(i);
			islandIndices.Sort((a, b) => rng.Next(-1, 2));

			int placed = 0;
			foreach (int idx in islandIndices)
			{
				var coast = islandCoasts[idx];
				coast.Sort((a, b) => rng.Next(-1, 2));
				int islandPlaced = 0;
				foreach (var (x, z) in coast)
				{
					if (placed >= maxPorts) return;
					if (islandPlaced >= portsPerIsland) break;
					if (IsFarFromExisting(x, z))
					{
						_ports[x, z] = true;
						placed++; islandPlaced++;
					}
				}
			}
		}

		// 周囲の陸セル数 (4近傍)
		private int LandNeighborCount(int x, int z)
		{
			int cnt = 0;
			for (int k = 0; k < 4; k++)
			{
				int nx = (x + _dx[k]).Mod(width);
				int nz = (z + _dz[k]).Mod(height);
				if (_map[nx, nz] == IVoxelMap.Code.Land) cnt++;
			}
			return cnt;
		}

		private bool IsFarFromExisting(int x, int z)
		{
			for (int dz = -portSpacing; dz <= portSpacing; dz++)
				for (int dx = -portSpacing; dx <= portSpacing; dx++)
				{
					int nx = (x + dx).Mod(width);
					int nz = (z + dz).Mod(height);
					if (_ports[nx, nz]) return false;
				}
			return true;
		}
		// ------------------------------------------------------------------
		private void BlurLand()
		{
			var src = (float[,])_heights.Clone();
			for (int z = 0; z < height; z++)
				for (int x = 0; x < width; x++)
				{
					if (_map[x, z] == IVoxelMap.Code.Sea) continue;
					float sum = 0; int cnt = 0;
					for (int dz = -1; dz <= 1; dz++)
						for (int dx = -1; dx <= 1; dx++)
						{
							int nx = (x + dx).Mod(width);
							int nz = (z + dz).Mod(height);
							if (_map[nx, nz] == IVoxelMap.Code.Land) { sum += src[nx, nz]; cnt++; }
						}
					_heights[x, z] = sum / cnt;
				}
		}

		// ------------------------------------------------------------------
		private void PaintTexture()
		{
			for (int z = 0; z < height; z++)
				for (int x = 0; x < width; x++)
				{
					Color c;
					if (_ports[x, z]) c = portColor;
					else if (_map[x, z] == IVoxelMap.Code.Land)
					{
						float h = _heights[x, z];
						c = Color.Lerp(landLowColor, landHighColor, Mathf.Pow(h, 1.7f));
					}
					else c = seaColor;
					_tex.SetPixel(x, z, c);
				}
			_tex.Apply();
		}

		private static Vector4[] BuildRotations(int seed, int count)
		{
			var rng = new System.Random(seed * 7919);
			var arr = new Vector4[count];
			for (int i = 0; i < count; i++)
			{
				float x = (float)rng.NextDouble() * 2f - 1f;
				float y = (float)rng.NextDouble() * 2f - 1f;
				float z = (float)rng.NextDouble() * 2f - 1f;
				float w = (float)rng.NextDouble() * 2f - 1f;
				arr[i] = new Vector4(x, y, z, w).normalized;
			}
			return arr;
		}

	}

	public class VoxelMapSampler : MonoBehaviour
	{
		[SerializeField] private VoxelMapGenerator _map;

		[Header("Scroll Settings")]
		[SerializeField] private float scrollSpeedX = 0.02f; // タイル/秒 (横方向)
		[SerializeField] private float scrollSpeedY = 0.01f; // タイル/秒 (縦方向)

		private void Awake() => _map.Generate();
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.R)) _map.Generate();
		}

		private void OnGUI()
		{
			if (_map.tex == null) return;

			float scale = Mathf.Min(Screen.width / (float)_map.width,
									Screen.height / (float)_map.height) * 0.9f;
			float w = _map.width * scale;
			float h = _map.depth * scale;
			Rect screenRect = new Rect((Screen.width - w) / 2f, 10, w, h);

			// 0-1 範囲でループする UV オフセット
			float uOff = (Time.time * scrollSpeedX) % 1f;
			float vOff = (Time.time * scrollSpeedY) % 1f;
			Rect uvRect = new Rect(uOff, vOff, 1f, 1f);   // WrapMode.Repeat なのでリピート表示

			GUI.DrawTextureWithTexCoords(screenRect, _map.tex, uvRect, false);
			GUI.Label(new Rect(10, Screen.height - 25, 600, 20),
					  $"[R] regenerate | auto-scroll ({scrollSpeedX:F2},{scrollSpeedY:F2}) tiles/s");
		}
	}

	internal static class IntExt
	{
		public static int Mod(this int a, int m) => ((a % m) + m) % m;
	}

	internal sealed class SeamlessNoise
	{
		private readonly float[] off = new float[8];
		public SeamlessNoise(int seed) { var rng = new System.Random(seed); for (int i = 0; i < 8; i++) off[i] = (float)rng.NextDouble() * 512f; }
		public double Evaluate(double x, double y, double z, double w)
		{
			float n1 = Mathf.PerlinNoise((float)x + off[0], (float)y + off[1]);
			float n2 = Mathf.PerlinNoise((float)z + off[2], (float)w + off[3]);
			float n3 = Mathf.PerlinNoise((float)x + off[4], (float)z + off[5]);
			float n4 = Mathf.PerlinNoise((float)y + off[6], (float)w + off[7]);
			return (n1 + n2 + n3 + n4) * 0.5f - 1f;
		}
	}
}
