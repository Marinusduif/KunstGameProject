Shader "EdgeDetection"
{
    Properties
    {
        _EdgeColor ("Edge Color", Color) = (0, 0, 0, 1)
        _BackgroundColor ("Background Color", Color) = (1, 1, 1, 1)
        _DepthThreshold ("Depth Threshold", Float) = 0.01
        _ColorThreshold ("Color Threshold", Float) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalRenderPipeline" }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            // Edge detection parameters
            float4 _EdgeColor;
            float4 _BackgroundColor;
            float _DepthThreshold;
            float _ColorThreshold;

            TEXTURE2D(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);
            TEXTURE2D(_CameraDepthTexture);
            SAMPLER(sampler_CameraDepthTexture);

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS);
                output.uv = input.uv;
                return output;
            }

            float DepthDifference(float2 uv)
            {
                float depth = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, uv).r;
                float2 dx = float2(ddx(uv.x), ddx(uv.y));
                float2 dy = float2(ddy(uv.x), ddy(uv.y));

                float depthDiffX = abs(depth - SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, uv + dx).r);
                float depthDiffY = abs(depth - SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, uv + dy).r);

                return max(depthDiffX, depthDiffY);
            }

            float ColorDifference(float2 uv)
            {
                float3 color = SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, uv).rgb;
                float2 dx = float2(ddx(uv.x), ddx(uv.y));
                float2 dy = float2(ddy(uv.x), ddy(uv.y));

                float3 colorDx = SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, uv + dx).rgb - color;
                float3 colorDy = SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, uv + dy).rgb - color;

                return max(length(colorDx), length(colorDy));
            }

            float4 frag(Varyings input) : SV_Target
            {
                float depthDiff = DepthDifference(input.uv);
                float colorDiff = ColorDifference(input.uv);

                float edge = step(_DepthThreshold, depthDiff) + step(_ColorThreshold, colorDiff);

                return lerp(_BackgroundColor, _EdgeColor, saturate(edge));
            }
            ENDHLSL
        }
    }
}
