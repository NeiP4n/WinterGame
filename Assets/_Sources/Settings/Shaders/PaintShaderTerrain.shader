Shader "PSX/WinterUnlit" {
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
    }
    SubShader {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 100

        Pass {
            Cull Off // ДОБАВЛЕНО: двусторонний рендеринг
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                UNITY_FOG_COORDS(3)
                float4 pos : SV_POSITION;
                float snow : TEXCOORD4;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _SnowTex;
            fixed4 _Color;
            float _SnowAmount;
            float _DitherStrength;
            float _ResolutionX;
            float _ResolutionY;
            float _TextureScale;
            float _PixelSize;
            float _ColorVariation;

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

            float4 sampleTriplanar(sampler2D tex, float3 worldPos, float3 normal) {
                float3 blend = abs(normal);
                blend = normalize(max(blend, 0.00001));
                blend = pow(blend, 8.0);
                blend /= (blend.x + blend.y + blend.z);
                
                float2 xzUV = floor(worldPos.xz / _PixelSize) * _PixelSize * _TextureScale;
                float2 xyUV = floor(worldPos.xy / _PixelSize) * _PixelSize * _TextureScale;
                float2 zyUV = floor(worldPos.zy / _PixelSize) * _PixelSize * _TextureScale;
                
                float4 xProj = tex2D(tex, zyUV) * blend.x;
                float4 yProj = tex2D(tex, xzUV) * blend.y;
                float4 zProj = tex2D(tex, xyUV) * blend.z;
                
                return xProj + yProj + zProj;
            }

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                // PSX снаппинг вершин
                float4 clip = o.pos;
                clip.xy /= clip.w;
                clip.xy = floor(clip.xy * float2(_ResolutionX, _ResolutionY)) / float2(_ResolutionX, _ResolutionY);
                clip.xy *= clip.w;
                o.pos = clip;

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.snow = saturate(dot(v.normal, float3(0,1,0)) * _SnowAmount);

                UNITY_TRANSFER_FOG(o, o.pos);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // Используем triplanar mapping
                fixed4 col = sampleTriplanar(_MainTex, i.worldPos, i.worldNormal);
                fixed4 snowCol = sampleTriplanar(_SnowTex, i.worldPos, i.worldNormal);

                // Микс со снегом
                col = lerp(col, snowCol, i.snow);

                // Вариация для каждого "пикселя"
                float2 pixelID = floor(i.worldPos.xz / _PixelSize);
                float variation = hash12(pixelID);
                float colorMod = 1.0 + (variation - 0.5) * _ColorVariation;
                col.rgb *= colorMod;

                // Зимний тинт
                col *= _Color;

                // Дитеринг
                float2 screenPos = floor(fmod(i.pos.xy, 4.0));
                float dither = ditherTable[screenPos.x][screenPos.y];
                col.rgb = floor(col.rgb * 31.0 + dither * _DitherStrength) / 31.0;

                UNITY_APPLY_FOG(i.fogCoord, col);

                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
