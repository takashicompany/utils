Shader "takashicompany/SpriteGlow"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        [Header(Glow)]
        _GlowColor ("Glow Color", Color) = (1,1,0,1)
        _GlowSize ("Glow Size (px)", Float) = 5
        _GlowSoftness ("Glow Softness", Range(0.01, 1)) = 0.5
        _GlowIntensity ("Glow Intensity", Float) = 1
        _GlowDirections ("Glow Directions", Integer) = 16
        _GlowSteps ("Glow Steps", Integer) = 3

        [Header(Additive)]
        _AdditiveColor ("Additive Color", Color) = (1,1,1,0)
        _AdditiveIntensity ("Additive Intensity", Range(0, 1)) = 0

        [Header(Advanced)]
        _PixelsPerUnit ("Pixels Per Unit", Float) = 100
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector" = "True"
            "CanUseSpriteAtlas" = "True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            Name "SpriteGlow"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_TexelSize;

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _GlowColor;
                float _GlowSize;
                float _GlowSoftness;
                float _GlowIntensity;
                float _GlowDirections;
                float _GlowSteps;
                float4 _AdditiveColor;
                float _AdditiveIntensity;
                float _PixelsPerUnit;
            CBUFFER_END

            #define TWO_PI 6.28318530718

            // UV が [0,1] 範囲内のときだけアルファを返す (範囲外 = スプライト外 = 透明)
            half SafeAlpha(float2 uv)
            {
                float2 inside = step(0, uv) * step(uv, 1);
                half a = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, saturate(uv)).a;
                return a * inside.x * inside.y;
            }

            Varyings vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);

                // メッシュを GlowSize 分だけ外側に拡張して光彩の描画領域を確保
                float2 expandUV = max(0, _GlowSize) * _MainTex_TexelSize.xy;
                float2 scale = 1.0 + 2.0 * expandUV;

                // UV中心(0.5,0.5)から外側に拡張 (Pivot位置に依存しない)
                float2 uvDir = input.uv * 2.0 - 1.0; // UV(0,0)→(-1,-1), UV(1,1)→(1,1)
                float expandObj = max(0, _GlowSize) / max(_PixelsPerUnit, 1.0);

                float3 pos = input.positionOS.xyz;
                pos.xy += uvDir * expandObj;
                output.positionCS = TransformObjectToHClip(pos);

                // UV も中心から拡張 (拡張部分は [0,1] 外になる)
                output.uv = (input.uv - 0.5) * scale + 0.5;

                output.color = input.color;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);

                // 元のスプライト描画
                half spriteAlpha = SafeAlpha(input.uv);
                half4 mainTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, saturate(input.uv));
                half4 spriteColor = mainTex * input.color * _Color;
                spriteColor.a = spriteAlpha * input.color.a * _Color.a;

                // 加算処理: スプライト自体を明るくする (白くする等)
                spriteColor.rgb += _AdditiveColor.rgb * _AdditiveIntensity * spriteAlpha * input.color.a;

                // GlowSize=0 or Intensity=0 なら光彩なし
                if (_GlowSize <= 0 || _GlowIntensity <= 0)
                    return spriteColor;

                float2 pixelSize = _MainTex_TexelSize.xy;
                float glowAlpha = 0;

                // 方向数・ステップ数をクランプ (安全上限)
                int dirCount = max((int)_GlowDirections, 1);
                int stepCount = max((int)_GlowSteps, 1);
                float angleStep = TWO_PI / (float)dirCount;
                float invSteps = 1.0 / (float)stepCount;

                // 最も近い不透明ピクセルまでの距離を探索
                float minDist = _GlowSize + 1.0;

                [loop]
                for (int d = 0; d < dirCount; d++)
                {
                    float angle = (float)d * angleStep;
                    float2 dir = float2(cos(angle), sin(angle));

                    [loop]
                    for (int s = 1; s <= stepCount; s++)
                    {
                        float t = (float)s * invSteps;
                        float dist = _GlowSize * t;
                        float2 offset = dir * dist * pixelSize;
                        half a = SafeAlpha(input.uv + offset);
                        // 不透明サンプルなら距離を記録、透明なら無視 (分岐なし)
                        minDist = min(minDist, lerp(_GlowSize + 1.0, dist, step(0.01, a)));
                    }
                }

                // 距離ベースの連続的な減衰 (バンディングなし)
                if (minDist <= _GlowSize)
                {
                    float normalizedDist = minDist / _GlowSize; // 0=エッジ近く, 1=最外部
                    // Softness: 小=ソリッドな光彩+硬い外縁, 大=エッジ付近から徐々に減衰
                    float fadeStart = 1.0 - _GlowSoftness;
                    glowAlpha = 1.0 - smoothstep(fadeStart, 1.0, normalizedDist);
                    glowAlpha = saturate(glowAlpha * _GlowIntensity);
                }

                // 合成: 光彩を後ろに、スプライトを上に
                half4 glow = half4(_GlowColor.rgb, glowAlpha * _GlowColor.a * input.color.a);

                half4 result;
                result.rgb = lerp(glow.rgb, spriteColor.rgb, spriteColor.a);
                result.a = saturate(max(spriteColor.a, glow.a));

                return result;
            }
            ENDHLSL
        }
    }

    Fallback "Sprites/Default"
}
