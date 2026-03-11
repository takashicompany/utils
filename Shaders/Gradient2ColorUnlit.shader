Shader "takashicompany/Gradient2ColorUnlit"
{
    Properties
    {
        _ColorA ("Color A", Color) = (0.2, 0.4, 0.8, 1)
        _ColorB ("Color B", Color) = (0.8, 0.2, 0.4, 1)
        _Direction ("Direction", Vector) = (0, 1, 0, 0)
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }

        Pass
        {
            Name "Gradient2ColorUnlit"
            Tags { "LightMode" = "UniversalForward" }

            Cull Back
            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _ColorA;
                float4 _ColorB;
                float4 _Direction;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionOS : TEXCOORD0;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.positionOS = input.positionOS.xyz;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float3 dir = normalize(_Direction.xyz);
                float len = length(_Direction.xyz);

                // dot product: -0.5 ~ +0.5 for unit cube/plane → remap to 0~1
                float d = dot(input.positionOS, dir);

                // Remap: use direction magnitude as scale hint
                // For a default plane/cube (extents -0.5 to 0.5), d ranges -0.5 to 0.5
                // We remap so that t=0 at the "negative" end and t=1 at the "positive" end
                // Adding 0.5 works for unit meshes; for arbitrary meshes, normalize by bounds
                float t = saturate(d + 0.5);

                half4 color = lerp(_ColorA, _ColorB, t);
                return color;
            }
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Unlit"
}
