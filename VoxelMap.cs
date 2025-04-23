// takashicompany.Unity.VoxelMap  ― カラー指定対応版（上下左右ループ）
// -----------------------------------------------------------------------------
// * インスペクターで「海の色」「陸（低い/高い）の色」を自由に指定可能
// * シームレス 2D トーラス地形。R キーで再生成
// -----------------------------------------------------------------------------

using UnityEngine;

namespace takashicompany.Unity
{
    public class VoxelMap : MonoBehaviour
    {
        public enum Code { Sea, Land }

        // ───────── 地形パラメータ ─────────
        [Header("Map Settings")]
        public int width  = 256;
        public int height = 256;
        [Range(0f,1f)] public float seaLevel = 0.45f;
        [Range(0.5f,10f)] public float frequency = 2.5f;
        [Range(1,8)] public int octaves = 6;
        public float persistence = 0.45f;
        public int seed = 1337;
        [Range(0,3)] public int blurPasses = 1;

        // ───────── カラー設定 ─────────
        [Header("Color Settings")]
        public Color seaColor      = new Color32(0, 0, 255, 255);
        public Color landLowColor  = new Color32(0, 255, 0, 255);
        public Color landHighColor = Color.white;

        private Code[,]  map;
        private float[,] heights;
        private Texture2D tex;
        private SeamlessNoise noise;
        private Vector4[] octaveRot;

        // ------------------------------------------------------------------
        private void Awake()  => Generate();
        private void Update(){ if(Input.GetKeyDown(KeyCode.R)) Generate(); }

        // ------------------------------------------------------------------
        public void Generate()
        {
            noise     = new SeamlessNoise(seed);
            octaveRot = BuildRotations(seed, octaves);
            map       = new Code [width,height];
            heights   = new float[width,height];

            tex = new Texture2D(width,height,TextureFormat.RGB24,false)
            {
                filterMode = FilterMode.Bilinear,
                wrapMode   = TextureWrapMode.Repeat,
            };

            // ── 1) 標高生成 ──
            for(int z=0; z<height; z++)
            {
                float v   = (float)z/height;
                float phi = v * Mathf.PI * 2f;
                float cPhi= Mathf.Cos(phi);
                float sPhi= Mathf.Sin(phi);

                for(int x=0; x<width; x++)
                {
                    float u   = (float)x/width;
                    float the = u * Mathf.PI * 2f;
                    float cThe= Mathf.Cos(the);
                    float sThe= Mathf.Sin(the);

                    float nx0=cThe, ny0=sThe, nz0=cPhi, nw0=sPhi;

                    double e=0, amp=1, freq=frequency;
                    for(int o=0;o<octaves;o++)
                    {
                        Vector4 r=octaveRot[o];
                        double nx=nx0*r.x + ny0*r.y + nz0*r.z + nw0*r.w;
                        double ny=nx0*r.y - ny0*r.x + nz0*r.w - nw0*r.z;
                        double nz=nx0*r.z - ny0*r.w - nz0*r.x + nw0*r.y;
                        double nw=nx0*r.w + ny0*r.z - nz0*r.y - nw0*r.x;
                        e += amp * noise.Evaluate(nx*freq, ny*freq, nz*freq, nw*freq);
                        amp *= persistence;
                        freq*= 2.0;
                    }
                    float ef=(float)(e*0.5+0.5);

                    bool land = ef>seaLevel;
                    map[x,z] = land?Code.Land:Code.Sea;
                    heights[x,z] = land? (ef-seaLevel)/(1f-seaLevel) : 0f;
                }
            }

            // ── 2) ブラー ──
            for(int p=0;p<blurPasses;p++) BlurLand();

            // ── 3) カラーマップ ──
            for(int z=0; z<height; z++)
            for(int x=0; x<width;  x++)
            {
                if(map[x,z]==Code.Land)
                {
                    float h=heights[x,z];
                    tex.SetPixel(x,z, Color.Lerp(landLowColor, landHighColor, Mathf.Pow(h,1.7f)) );
                }
                else
                {
                    tex.SetPixel(x,z, seaColor );
                }
            }
            tex.Apply();
        }

        // ------------------------------------------------------------------
        private void BlurLand()
        {
            var src=(float[,])heights.Clone();
            for(int z=0; z<height; z++)
            for(int x=0; x<width;  x++)
            {
                if(map[x,z]==Code.Sea) continue;
                float sum=0; int cnt=0;
                for(int dz=-1;dz<=1;dz++)
                for(int dx=-1;dx<=1;dx++)
                {
                    int nx=(x+dx).Mod(width);
                    int nz=(z+dz).Mod(height);
                    if(map[nx,nz]==Code.Land){ sum+=src[nx,nz]; cnt++; }
                }
                heights[x,z]=sum/cnt;
            }
        }

        // ------------------------------------------------------------------
        private void OnGUI()
        {
            if(tex==null) return;
            float sc = Mathf.Min(Screen.width/(float)width, Screen.height/(float)height)*0.9f;
            float w=width*sc, h=height*sc;
            GUI.DrawTexture(new Rect((Screen.width-w)/2f,10,w,h), tex, ScaleMode.ScaleToFit,false);
            GUI.Label(new Rect(10,Screen.height-25,400,20), "[R] regenerate | color‑configurable torus map" );
        }

        // ------------------------------------------------------------------
        public float GetHeight(int x,int z)=>heights[x.Mod(width),z.Mod(height)];
        public Code  GetCode  (int x,int z)=>map    [x.Mod(width),z.Mod(height)];

        // ------------------------------------------------------------------
        private static Vector4[] BuildRotations(int seed,int count)
        {
            var rng=new System.Random(seed*7919);
            var arr=new Vector4[count];
            for(int i=0;i<count;i++)
            {
                float x=(float)rng.NextDouble()*2f-1f;
                float y=(float)rng.NextDouble()*2f-1f;
                float z=(float)rng.NextDouble()*2f-1f;
                float w=(float)rng.NextDouble()*2f-1f;
                arr[i]=new Vector4(x,y,z,w).normalized;
            }
            return arr;
        }
    }

    // ----------------------------------------------------------------------
    internal static class IntExt
    {
        public static int Mod(this int a,int m)=>((a%m)+m)%m;
    }

    // ----------------------------------------------------------------------
    // シームレス Perlin fBm（4D 擬似）
    // ----------------------------------------------------------------------
    internal sealed class SeamlessNoise
    {
        private readonly float[] off=new float[8];
        public SeamlessNoise(int seed){ var rng=new System.Random(seed); for(int i=0;i<8;i++) off[i]=(float)rng.NextDouble()*512f; }
        public double Evaluate(double x,double y,double z,double w)
        {
            float n1=Mathf.PerlinNoise((float)x+off[0],(float)y+off[1]);
            float n2=Mathf.PerlinNoise((float)z+off[2],(float)w+off[3]);
            float n3=Mathf.PerlinNoise((float)x+off[4],(float)z+off[5]);
            float n4=Mathf.PerlinNoise((float)y+off[6],(float)w+off[7]);
            return (n1+n2+n3+n4)*0.5f - 1f;
        }
    }
}
