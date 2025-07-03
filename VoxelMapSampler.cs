// takashicompany.Unity.VoxelMap  — PNG 保存対応 完全版 (scroll & save)
// -----------------------------------------------------------------------------
// * Procedural seamless torus map with island‑aware ports.
// * VoxelMapGenerator.Generate(string savePath = null) → PNG 保存。
// * VoxelMapSampler で自動スクロール描画。コピペ即動作。
// -----------------------------------------------------------------------------

namespace takashicompany.Unity
{
	using System.Collections.Generic;
	using System.IO;
	using UnityEngine;
	using System.Linq;

	// ════════════════════════════════
	#region IVoxelMap
	// ════════════════════════════════
	public interface IVoxelMap
	{
		public enum Code { Sea, Land }
		int width { get; }
		int depth { get; }
		Code GetCode(int x, int z);
		float GetHeight(int x, int z);
		IEnumerable<(int x, int z)> GetPorts();
	}
	#endregion

	// ════════════════════════════════
	#region VoxelMap (Texture2D 読み込み実装)
	// ════════════════════════════════
	public class VoxelMap : IVoxelMap
	{
		private readonly int _width;
		private readonly int _depth;
		private readonly IVoxelMap.Code[,] _codes;
		private readonly float[,] _heights;
		private readonly List<(int, int)> _ports = new();

		public int width => _width;
		public int depth => _depth;

		public VoxelMap(Texture2D tex, Color seaColor, Color portColor, Color landLowColor)
		{
			_width = tex.width;
			_depth = tex.height;
			_codes = new IVoxelMap.Code[_width, _depth];
			_heights = new float[_width, _depth];

			Color32 seaC = seaColor;
			Color32 portC = portColor;
			Color32 landB = landLowColor;

			for (int z = 0; z < _depth; z++)
			{
				for (int x = 0; x < _width; x++)
				{
					Color32 c = tex.GetPixel(x, z);
					if (Equal(c, portC))
					{
						_codes[x, z] = IVoxelMap.Code.Land; _ports.Add((x, z)); _heights[x, z] = 0.5f;
					}
					else if (Equal(c, seaC))
					{
						_codes[x, z] = IVoxelMap.Code.Sea; _heights[x, z] = 0f;
					}
					else
					{
						_codes[x, z] = IVoxelMap.Code.Land; _heights[x, z] = EstimateH(c, landB);
					}
				}
			}
		}

		/// <summary>
		/// PNG ファイルから VoxelMap を生成するファクトリ
		/// </summary>
		public static VoxelMap LoadFromPNG(string path, Color seaColor, Color portColor, Color landLowColor)
		{
			if (!File.Exists(path))
			{
				Debug.LogError($"PNG not found: {path}");
				return null;
			}
			byte[] bytes = File.ReadAllBytes(path);
			var tex = new Texture2D(2, 2, TextureFormat.RGB24, false) { filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Repeat };
			tex.LoadImage(bytes);
			return new VoxelMap(tex, seaColor, portColor, landLowColor);
		}

		public IVoxelMap.Code GetCode(int x, int z) => _codes[Mod(x, _width), Mod(z, _depth)];
		public float GetHeight(int x, int z) => _heights[Mod(x, _width), Mod(z, _depth)];
		public IEnumerable<(int, int)> GetPorts() => _ports;

		private static bool Equal(Color32 a, Color32 b) => a.r == b.r && a.g == b.g && a.b == b.b;
		private static int Mod(int a, int m) => ((a % m) + m) % m;
		private static float EstimateH(Color32 c, Color32 b) { float y = 0.2126f * c.r / 255f + 0.7152f * c.g / 255f + 0.0722f * c.b / 255f; float y0 = 0.2126f * b.r / 255f + 0.7152f * b.g / 255f + 0.0722f * b.b / 255f; return Mathf.Clamp01((y - y0) / (1f - y0)); }
	}
	#endregion

	// ════════════════════════════════
	#region VoxelMapGenerator (プロシージャル生成 & PNG 保存)
	// ════════════════════════════════
	[System.Serializable]
	public class VoxelMapGenerator
	{
		// ---- Map Settings ----
		[Header("Map Settings")]
		public int width = 256;
		public int height = 256; // depth と同値
		public int depth => height;
		[Range(0f, 1f)] public float seaLevel = 0.45f;
		[Range(0.5f, 10f)] public float frequency = 2.5f;
		[Range(1, 8)] public int octaves = 6;
		public float persistence = 0.45f;
		public int seed = 1337;
		[Range(0, 3)] public int blurPasses = 1;

		// ---- Color Settings ----
		[Header("Color Settings")]
		public Color seaColor = new Color32(0, 0, 255, 255);
		public Color landLowColor = new Color32(0, 255, 0, 255);
		public Color landHighColor = Color.white;
		public Color portColor = Color.red;

		// ---- Port Settings ----
		[Header("Port Settings")]
		public int maxPorts = 80;
		public int portsPerIsland = 1;
		public int portSpacing = 6;

		// internal
		protected IVoxelMap.Code[,] _map;
		protected float[,] _heights;
		protected bool[,] _ports;
		protected Texture2D _tex; public Texture2D tex => _tex;
		protected SeamlessNoise _noise;
		protected Vector4[] _octaveRot;
		private readonly int[] _dx = { 1, -1, 0, 0 };
		private readonly int[] _dz = { 0, 0, 1, -1 };

		public IVoxelMap.Code GetCode(int x, int z) => _map[x.Mod(width), z.Mod(height)];
		public float GetHeight(int x, int z) => _heights[x.Mod(width), z.Mod(height)];
		public bool IsPort(int x, int z) => _ports[x.Mod(width), z.Mod(height)];

		// ---- Public Generate ----
		public void Generate()
		{
			_noise = new SeamlessNoise(seed);
			_octaveRot = BuildRotations(seed, octaves);
			_map = new IVoxelMap.Code[width, height];
			_heights = new float[width, height];
			_ports = new bool[width, height];

			_tex = new Texture2D(width, height, TextureFormat.RGB24, false) { filterMode = FilterMode.Bilinear, wrapMode = TextureWrapMode.Repeat };

			GenerateHeights();
			for (int i = 0; i < blurPasses; i++) BlurLand();
			PlacePorts();
			Paint();
		}

		// ---- Private Generate ----
		private void OnValidate()
		{
			if (_tex != null) _tex.Reinitialize(width, height);
			if (_map != null) _map = new IVoxelMap.Code[width, height];
			if (_heights != null) _heights = new float[width, height];
			if (_ports != null) _ports = new bool[width, height];
		}

		// ---- Height field ----
		private void GenerateHeights()
		{
			for (int z = 0; z < height; z++)
			{
				float v = (float)z / height; float phi = v * Mathf.PI * 2f; float cPhi = Mathf.Cos(phi), sPhi = Mathf.Sin(phi);
				for (int x = 0; x < width; x++)
				{
					float u = (float)x / width; float the = u * Mathf.PI * 2f; float cThe = Mathf.Cos(the), sThe = Mathf.Sin(the);
					float nx0 = cThe, ny0 = sThe, nz0 = cPhi, nw0 = sPhi;
					double e = 0, amp = 1, fr = frequency;
					for (int o = 0; o < octaves; o++)
					{
						Vector4 r = _octaveRot[o];
						double nx = nx0 * r.x + ny0 * r.y + nz0 * r.z + nw0 * r.w;
						double ny = nx0 * r.y - ny0 * r.x + nz0 * r.w - nw0 * r.z;
						double nz = nx0 * r.z - ny0 * r.w - nz0 * r.x + nw0 * r.y;
						double nw = nx0 * r.w + ny0 * r.z - nz0 * r.y - nw0 * r.x;
						e += amp * _noise.Evaluate(nx * fr, ny * fr, nz * fr, nw * fr);
						amp *= persistence; fr *= 2.0;
					}
					float ef = (float)(e * 0.5 + 0.5);
					bool land = ef > seaLevel;
					_map[x, z] = land ? IVoxelMap.Code.Land : IVoxelMap.Code.Sea;
					_heights[x, z] = land ? (ef - seaLevel) / (1f - seaLevel) : 0f;
				}
			}
		}

		// ---- Ports ----
		private void PlacePorts()
		{
			int[,] id = new int[width, height]; for (int z = 0; z < height; z++) for (int x = 0; x < width; x++) id[x, z] = -1;
			var coastIslands = new List<List<(int, int)>>(); int cur = 0;
			for (int z = 0; z < height; z++)
				for (int x = 0; x < width; x++)
				{
					if (_map[x, z] == IVoxelMap.Code.Land && id[x, z] == -1)
					{
						var q = new Queue<(int, int)>(); q.Enqueue((x, z)); id[x, z] = cur; var coast = new List<(int, int)>();
						while (q.Count > 0)
						{
							var (cx, cz) = q.Dequeue(); bool coastF = false;
							for (int k = 0; k < 4; k++)
							{
								int nx = (cx + _dx[k]).Mod(width); int nz = (cz + _dz[k]).Mod(height);
								if (_map[nx, nz] == IVoxelMap.Code.Sea) coastF = true;
								else if (id[nx, nz] == -1) { id[nx, nz] = cur; q.Enqueue((nx, nz)); }
							}
							if (coastF && LandNeigh(cx, cz) >= 2) coast.Add((cx, cz));
						}
						coastIslands.Add(coast); cur++;
					}
				}
			var rng = new System.Random(seed * 59); var order = new List<int>(); for (int i = 0; i < coastIslands.Count; i++) order.Add(i); order.Sort((a, b) => rng.Next(-1, 2));
			int placed = 0;
			foreach (int idx in order)
			{
				var coast = coastIslands[idx]; coast.Sort((a, b) => rng.Next(-1, 2)); int ip = 0;
				foreach (var (x, z) in coast)
				{
					if (placed >= maxPorts) return; if (ip >= portsPerIsland) break; if (IsFar(x, z)) { _ports[x, z] = true; placed++; ip++; }
				}
			}
		}
		private int LandNeigh(int x, int z) { int c = 0; for (int k = 0; k < 4; k++) { if (_map[(x + _dx[k]).Mod(width), (z + _dz[k]).Mod(height)] == IVoxelMap.Code.Land) c++; } return c; }
		private bool IsFar(int x, int z) { for (int dz = -portSpacing; dz <= portSpacing; dz++) for (int dx = -portSpacing; dx <= portSpacing; dx++) if (_ports[(x + dx).Mod(width), (z + dz).Mod(height)]) return false; return true; }

		// ---- Blur ----
		private void BlurLand()
		{
			var src = (float[,])_heights.Clone();
			for (int z = 0; z < height; z++)
				for (int x = 0; x < width; x++)
				{
					if (_map[x, z] == IVoxelMap.Code.Sea) continue; float sum = 0; int cnt = 0;
					for (int dz = -1; dz <= 1; dz++)
						for (int dx = -1; dx <= 1; dx++)
						{ int nx = (x + dx).Mod(width); int nz = (z + dz).Mod(height); if (_map[nx, nz] == IVoxelMap.Code.Land) { sum += src[nx, nz]; cnt++; } }
					_heights[x, z] = sum / cnt;
				}
		}

		// ---- Paint ----
		protected virtual void Paint()
		{
			for (int z = 0; z < height; z++) for (int x = 0; x < width; x++)
				{
					Color c = _map[x, z] == IVoxelMap.Code.Sea ? seaColor : Color.Lerp(landLowColor, landHighColor, Mathf.Pow(_heights[x, z], 1.7f));
					if (_ports[x, z]) c = portColor; _tex.SetPixel(x, z, c);
				}
			_tex.Apply();
		}

		// ---- Save PNG ----
		public void SavePNG(string path)
		{
			if (string.IsNullOrEmpty(path)) return;
			byte[] bytes = _tex.EncodeToPNG();
			string full = Path.IsPathRooted(path) ? path : Path.Combine(Application.dataPath, path.TrimStart('/', '\\'));
			File.WriteAllBytes(full, bytes);
#if UNITY_EDITOR
			UnityEditor.AssetDatabase.Refresh();
#endif
			Debug.Log($"Voxel map PNG saved to {full}");
		}

		// ---- Utilities ----
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
	#endregion

	// ════════════════════════════════
	#region VoxelMapSampler (自動スクロール GUI)
	// ════════════════════════════════
	public class VoxelMapSampler : MonoBehaviour
	{
		[SerializeField] private VoxelMapGenerator _map;
		[Header("Scroll Settings")]
		[SerializeField] private float scrollSpeedX = 0.02f;
		[SerializeField] private float scrollSpeedY = 0.01f;

		private void Awake() => _map.Generate();
		private void Update() { if (Input.GetKeyDown(KeyCode.R)) _map.Generate(); }

		[ContextMenu("Generate & Save PNG")]
		private void GenerateAndSave()
		{
			_map.Generate();
			_map.SavePNG(SaveData.GeneratePathByPersistent("VoxelMap.png"));
		}


		private void OnGUI()
		{
			if (_map.tex == null) return;
			float scale = Mathf.Min(Screen.width / (float)_map.width, Screen.height / (float)_map.depth) * 0.9f;
			float w = _map.width * scale, h = _map.depth * scale;
			Rect rect = new Rect((Screen.width - w) / 2f, 10, w, h);
			float u = (Time.time * scrollSpeedX) % 1f, v = (Time.time * scrollSpeedY) % 1f;
			GUI.DrawTextureWithTexCoords(rect, _map.tex, new Rect(u, v, 1f, 1f), false);
			GUI.Label(new Rect(10, Screen.height - 25, 600, 20), $"[R] regenerate | auto-scroll ({scrollSpeedX:F2},{scrollSpeedY:F2}) tiles/s");
		}
	}
	#endregion

	// ════════════════════════════════
	#region Helpers
	// ════════════════════════════════
	public static class VoxelMapExtension
	{
		public static int Mod(this int a, int m) => ((a % m) + m) % m;
		/// <summary>
		/// URP 環境向け：IVoxelMap を 3 種（海・陸・港）で色分けした
		/// デバッグ用キューブを並べる静的関数。まとめて扱える親 Transform を返します。
		/// </summary>
		public static Transform SpawnVoxelDebugCubesURP(
			IVoxelMap map,
			Color seaColor,
			Color landColor,
			Color portColor,
			float cellSize = 1f)
		{
			// ―― URP Lit マテリアルを生成（単色・マット） ――
			static Material MakeLit(Color col)
			{
				var m = new Material(Shader.Find("Universal Render Pipeline/Lit"));
				m.SetColor("_BaseColor", col);
				m.SetFloat("_Smoothness", 0f);
				return m;
			}
			Material seaMat = MakeLit(seaColor);
			Material landMat = MakeLit(landColor);
			Material portMat = MakeLit(portColor);

			// 親ノード
			var parent = new GameObject("URP_VoxelDebugCubes").transform;

			Vector3 half = Vector3.one * cellSize * 0.5f;
			for (int z = 0; z < map.depth; z++)
				for (int x = 0; x < map.width; x++)
				{
					IVoxelMap.Code code = map.GetCode(x, z);

					// キューブ生成
					var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
					cube.transform.SetParent(parent, false);
					cube.transform.localScale = Vector3.one * cellSize * 0.95f;
					cube.transform.position = new Vector3(x * cellSize + half.x, 0f, z * cellSize + half.z);

					// 種別ごとにマテリアル割当
					var renderer = cube.GetComponent<MeshRenderer>();
					renderer.sharedMaterial = code switch
					{
						IVoxelMap.Code.Sea => seaMat,
						_ => landMat        // デフォルト陸
					};

					// 港セルは上書き
					if (map is { } && map.GetPorts() is IEnumerable<(int px, int pz)> ports
						&& ports.Contains((x, z)))
					{
						renderer.sharedMaterial = portMat;
					}
				}
			return parent;
		}
	}

	public sealed class SeamlessNoise
	{
		private readonly float[] off = new float[8];
		public SeamlessNoise(int seed) { var rng = new System.Random(seed); for (int i = 0; i < 8; i++) off[i] = (float)rng.NextDouble() * 512f; }
		public double Evaluate(double x, double y, double z, double w) { float n1 = Mathf.PerlinNoise((float)x + off[0], (float)y + off[1]); float n2 = Mathf.PerlinNoise((float)z + off[2], (float)w + off[3]); float n3 = Mathf.PerlinNoise((float)x + off[4], (float)z + off[5]); float n4 = Mathf.PerlinNoise((float)y + off[6], (float)w + off[7]); return (n1 + n2 + n3 + n4) * 0.5f - 1f; }
	}
	#endregion
}
