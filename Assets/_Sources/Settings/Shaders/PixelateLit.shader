Shader "PSX/WinterLit" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Color ("Winter Tint", Color) = (0.85, 0.92, 1.0, 1.0)
        _SnowTex ("Snow Texture", 2D) = "white" {}
        _SnowAmount ("Snow Amount", Range(0,1)) = 0.5
        _DitherStrength ("Dither Strength", Range(0,1)) = 0.5
        _ResolutionX ("Screen Width", Float) = 320
        _ResolutionY ("Screen Height", Float) = 280
        
        _TextureScale ("Texture Scale", Float) = 1.0
        _PixelSize ("Pixel Size (World)", Float) = 0.1
        _ColorVariation ("Color Variation", Range(0,0.3)) = 0.08
        
        _Smoothness ("Smoothness", Range(0,1)) = 0.0
        _Metallic ("Metallic", Range(0,1)) = 0.0
        
        [Enum(Off,0,On,1)] _AlphaTest ("Alpha Cutout", Float) = 0
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags { 
            "RenderType"="TransparentCutout" 
            "Queue"="AlphaTest"
            "RenderPipeline"="UniversalPipeline"
        }
        LOD 200

        Pass {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }
            Cull Off
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
                float4 color : COLOR;
            };

            struct Varyings {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                float4 vertexColor : COLOR;
                float snow : TEXCOORD3;
                float fogFactor : TEXCOORD4;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_SnowTex);
            SAMPLER(sampler_SnowTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float _SnowAmount;
                float _DitherStrength;
                float _ResolutionX;
                float _ResolutionY;
                float _TextureScale;
                float _PixelSize;
                float _ColorVariation;
                float _Smoothness;
                float _Metallic;
                float _AlphaTest;
                float _Cutoff;
            CBUFFER_END

            static const float4x4 ditherTable = float4x4(
                float4( 0.0/16.0,  8.0/16.0,  2.0/16.0, 10.0/16.0),
                float4(12.0/16.0,  4.0/16.0, 14.0/16.0,  6.0/16.0),
                float4( 3.0/16.0, 11.0/16.0,  1.0/16.0,  9.0/16.0),
                float4(15.0/16.0,  7.0/16.0, 13.0/16.0,  5.0/16.0)
            );

            float hash12(float2 p) {
                float3 p3 = frac(float3(p.xyx) * 0.1031);
                p3 += dot(p3, p3.yzx + 33.33);
                return frac((p3.x + p3.y) * p3.z);
            }

            float4 sampleTriplanar(TEXTURE2D_PARAM(tex, texSampler), float3 worldPos, float3 normal) {
                float3 blend = abs(normal);
                blend = normalize(max(blend, 0.00001));
                blend = pow(blend, 8.0);
                blend /= (blend.x + blend.y + blend.z);
                
                float2 xzUV = floor(worldPos.xz / _PixelSize) * _PixelSize * _TextureScale;
                float2 xyUV = floor(worldPos.xy / _PixelSize) * _PixelSize * _TextureScale;
                float2 zyUV = floor(worldPos.zy / _PixelSize) * _PixelSize * _TextureScale;
                
                float4 xProj = SAMPLE_TEXTURE2D(tex, texSampler, zyUV) * blend.x;
                float4 yProj = SAMPLE_TEXTURE2D(tex, texSampler, xzUV) * blend.y;
                float4 zProj = SAMPLE_TEXTURE2D(tex, texSampler, xyUV) * blend.z;
                
                return xProj + yProj + zProj;
            }

            Varyings vert(Attributes input) {
                Varyings output;
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vertexInput.positionCS;

                // PSX снаппинг
                float4 clip = output.positionCS;
                clip.xy /= clip.w;
                clip.xy = floor(clip.xy * float2(_ResolutionX, _ResolutionY)) / float2(_ResolutionX, _ResolutionY);
                clip.xy *= clip.w;
                output.positionCS = clip;

                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.positionWS = vertexInput.positionWS;
                
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS);
                output.normalWS = normalInput.normalWS;
                
                output.snow = saturate(dot(input.normalOS, float3(0,1,0)) * _SnowAmount);
                output.vertexColor = input.color;
                output.fogFactor = ComputeFogFactor(output.positionCS.z);

                return output;
            }

            half4 frag(Varyings input) : SV_Target {
                // Triplanar sampling
                half4 col = sampleTriplanar(TEXTURE2D_ARGS(_MainTex, sampler_MainTex), input.positionWS, input.normalWS);
                half4 snowCol = sampleTriplanar(TEXTURE2D_ARGS(_SnowTex, sampler_SnowTex), input.positionWS, input.normalWS);

                // Alpha cutout
                if (_AlphaTest > 0.5) {
                    clip(col.a - _Cutoff);
                }

                // Snow mix
                col = lerp(col, snowCol, input.snow);
                
                // Vertex color
                col *= input.vertexColor;

                // Pixel variation
                float2 pixelID = floor(input.positionWS.xz / _PixelSize);
                float variation = hash12(pixelID);
                float colorMod = 1.0 + (variation - 0.5) * _ColorVariation;
                col.rgb *= colorMod;

                // Tint
                col *= _Color;

                // Lighting
                InputData lightingInput = (InputData)0;
                lightingInput.positionWS = input.positionWS;
                lightingInput.normalWS = normalize(input.normalWS);
                lightingInput.viewDirectionWS = GetWorldSpaceNormalizeViewDir(input.positionWS);
                lightingInput.shadowCoord = TransformWorldToShadowCoord(input.positionWS);
                lightingInput.fogCoord = input.fogFactor;

                SurfaceData surfaceData = (SurfaceData)0;
                surfaceData.albedo = col.rgb;
                surfaceData.alpha = col.a;
                surfaceData.metallic = _Metallic;
                surfaceData.smoothness = _Smoothness;
                surfaceData.normalTS = float3(0, 0, 1);
                surfaceData.occlusion = 1.0;

                half4 color = UniversalFragmentPBR(lightingInput, surfaceData);

                // Dithering
                float2 screenPos = floor(fmod(input.positionCS.xy, 4.0));
                float dither = ditherTable[screenPos.x][screenPos.y];
                color.rgb = floor(color.rgb * 31.0 + dither * _DitherStrength) / 31.0;

                color.rgb = MixFog(color.rgb, input.fogFactor);
                return color;
            }
            ENDHLSL
        }

        Pass {
            Name "ShadowCaster"
            Tags { "LightMode"="ShadowCaster" }
            
            HLSLPROGRAM
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }
    }
    FallBack "Universal Render Pipeline/Lit"
}
