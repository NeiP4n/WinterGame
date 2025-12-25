Shader "URP/PSX/VertexLitTransparent_Normal"
{
    Properties
    {
        _Color ("Color (RGBA)", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}

        _NormalMap ("Normal Map", 2D) = "bump" {}
        _NormalStrength ("Normal Strength", Range(0,2)) = 1

        _EmissionColor ("Emission Color", Color) = (0,0,0,0)
        _EmissiveTex ("Emission Texture", 2D) = "black" {}

        _Alpha ("Alpha", Range(0,1)) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalRenderPipeline"
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }

        Pass
        {
            Name "Forward"
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 tangentWS : TEXCOORD2;
                float3 bitangentWS : TEXCOORD3;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);

            TEXTURE2D(_EmissiveTex);
            SAMPLER(sampler_EmissiveTex);

            float4 _Color;
            float4 _EmissionColor;
            float _Alpha;
            float _NormalStrength;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;

                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.tangentWS = TransformObjectToWorldDir(IN.tangentOS.xyz);
                OUT.bitangentWS = cross(OUT.normalWS, OUT.tangentWS) * IN.tangentOS.w;

                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                // Base color
                half4 baseCol = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv) * _Color;

                // Normal map (tangent space)
                half3 normalTS = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, IN.uv));
                normalTS.xy *= _NormalStrength;

                // TBN matrix
                half3x3 TBN = half3x3(
                    normalize(IN.tangentWS),
                    normalize(IN.bitangentWS),
                    normalize(IN.normalWS)
                );

                half3 normalWS = normalize(mul(normalTS, TBN));

                // PSX-style simple lighting
                half3 lightDir = normalize(float3(0.3, 0.8, 0.5));
                half lighting = saturate(dot(normalWS, lightDir) * 0.5 + 0.5);

                // Emission (ice cracks)
                half3 emission = SAMPLE_TEXTURE2D(_EmissiveTex, sampler_EmissiveTex, IN.uv).rgb
                                 * _EmissionColor.rgb;

                half3 color = baseCol.rgb * lighting + emission;

                return half4(color, baseCol.a * _Alpha);
            }
            ENDHLSL
        }
    }
}
