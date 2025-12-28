Shader "URP/PSX/UnlitTransparentCracks"
{
    Properties
    {
        _Color              ("Color (RGBA)", Color) = (1,1,1,1)
        _MainTex            ("Texture", 2D) = "white" {}
        _CrackTex           ("Crack Texture", 2D) = "black" {}
        _CrackAmount        ("Crack Amount", Range(0,1)) = 0
        _CustomDepthOffset  ("Custom Depth Offset", Float) = 0

        _MainTex_ST         ("", Vector) = (1,1,0,0)
        _CrackTex_ST        ("", Vector) = (1,1,0,0)
    }

    SubShader
    {
        Tags
        {
            "RenderType"     = "Transparent"
            "Queue"          = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Back
        LOD 100

        Pass
        {
            Name "Forward"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM

            #pragma vertex   vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float  _CrackAmount;
                float  _CustomDepthOffset;
                float4 _MainTex_ST;
                float4 _CrackTex_ST;
            CBUFFER_END

            TEXTURE2D(_MainTex);   SAMPLER(sampler_MainTex);
            TEXTURE2D(_CrackTex);  SAMPLER(sampler_CrackTex);

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uvMain     : TEXCOORD0;
                float2 uvCrack    : TEXCOORD1;
                half   fogFactor  : TEXCOORD2;
            };

            Varyings vert (Attributes IN)
            {
                Varyings OUT;

                VertexPositionInputs posInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                float3 positionWS = posInputs.positionWS;
                positionWS += float3(0,0,1) * _CustomDepthOffset;

                OUT.positionCS = TransformWorldToHClip(positionWS);

                OUT.uvMain  = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.uvCrack = TRANSFORM_TEX(IN.uv, _CrackTex);

                OUT.fogFactor = ComputeFogFactor(OUT.positionCS.z);
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                float4 col   = SAMPLE_TEXTURE2D(_MainTex,  sampler_MainTex,  IN.uvMain) * _Color;
                float  crack = SAMPLE_TEXTURE2D(_CrackTex, sampler_CrackTex, IN.uvCrack).r;

                crack = saturate(crack * _CrackAmount);

                // 0 = без эффекта, 1 = полностью «трещина» того же цвета
                // коэффициент можно подправить (например 1.5 для ярче)
                col.rgb *= lerp(1.0, 0.0, crack);      // трещины темнее base
                col.a    = max(col.a, crack);          // alpha от маски

                #if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
                col.rgb = MixFog(col.rgb, IN.fogFactor);
                #endif

                return col;
            }

            ENDHLSL
        }
    }

    FallBack Off
}
