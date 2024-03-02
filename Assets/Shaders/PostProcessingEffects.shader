Shader "PostProcessing/PostProcessingEffects"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Downsample_Factor ("Downsample factor", Float) = 0.1
        _Upsample_Factor ("Upsample factor", Float) = 0.1
        _WidthPixelation ("Width Pixelation", Float) = 1.0
        _HeightPixelation ("Height Pixelation", Float) = 1.0
        _ColorPrecision ("Color Precision", Float) = 1.0
        _Intensity ("Gamma Correction Intensity", Float) = 0.1
        _CarSpeed ("Car Speed", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        CGINCLUDE
            #include "UnityCG.cginc"

            struct VertexData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float2 _MainTex_TexelSize;
            float4 _TempTex;
            
            float3 Sample(float2 uv){
                return tex2D(_MainTex, uv);
            }

            float3 BoxSample(float2 uv, float offset){
                float2 xOffsets = _MainTex_TexelSize.x * float2(-offset, offset); // x neg texelX*offset, y pos
                float2 yOffsets = _MainTex_TexelSize.y * float2(-offset, offset); // x neg texelY*offset, y pos
                // weighted average sample to the corners
                float3 averageSample = (Sample(uv + float2(xOffsets.x, yOffsets.x))+ Sample(uv + float2(xOffsets.x, yOffsets.y))+ Sample(uv + float2(xOffsets.y, yOffsets.x))+ Sample(uv + float2(xOffsets.y, yOffsets.y))) / 4;
                return averageSample;
            }

            // simple vertex shader which simply converts to clip space
            v2f vert (VertexData v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
        ENDCG

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float _Downsample_Factor;


            float4 frag (v2f i) : SV_Target
            {
                // downsample
                return float4(BoxSample(i.uv, _Downsample_Factor), 1.0f);
            }
            ENDCG
        }

        Pass
        {
            // Blend 1 to 1
            Blend One One
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float _Upsample_Factor;

            float4 frag (v2f i) : SV_Target
            {
                // upsample
                return float4(BoxSample(i.uv, _Upsample_Factor), 1.0f);;
            }
            ENDCG
        }

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // pixelation amount
            float _WidthPixelation;
            float _HeightPixelation;
            
            // color precision
            float _ColorPrecision;

            // Gamma correction intensity
            float _Intensity;

            float4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                // round uvs to the closest multiple of the pixelization amount
                uv.x = floor(uv.x * _WidthPixelation) / _WidthPixelation;
                uv.y = floor(uv.y * _HeightPixelation) / _HeightPixelation;
                
                // sample texture
                float4 col = tex2D(_MainTex, uv) ;
                //color precision
                col = floor(col * _ColorPrecision)/_ColorPrecision;
                col =  pow(col, _Intensity);

                // upsample
                float4 ret = col;
                _TempTex = ret;
                return col;
            }
            ENDCG
        }

        GrabPass { "_PixelizedTexture" }

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // possible functions generated by unity's shader graph
            inline float unity_noise_randomValue (float2 uv)
            {
                return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453);
            }

            inline float unity_noise_interpolate (float a, float b, float t)
            {
                return (1.0-t)*a + (t*b);
            }

            inline float unity_valueNoise (float2 uv)
            {
                float2 i = floor(uv);
                float2 f = frac(uv);
                f = f * f * (3.0 - 2.0 * f);

                uv = abs(frac(uv) - 0.5);
                float2 c0 = i + float2(0.0, 0.0);
                float2 c1 = i + float2(1.0, 0.0);
                float2 c2 = i + float2(0.0, 1.0);
                float2 c3 = i + float2(1.0, 1.0);
                float r0 = unity_noise_randomValue(c0);
                float r1 = unity_noise_randomValue(c1);
                float r2 = unity_noise_randomValue(c2);
                float r3 = unity_noise_randomValue(c3);

                float bottomOfGrid = unity_noise_interpolate(r0, r1, f.x);
                float topOfGrid = unity_noise_interpolate(r2, r3, f.x);
                float t = unity_noise_interpolate(bottomOfGrid, topOfGrid, f.y);
                return t;
            }

            float Unity_SimpleNoise_float(float2 UV, float Scale)
            {
                float t = 0.0;

                float freq = pow(2.0, float(0));
                float amp = pow(0.5, float(3-0));
                t += unity_valueNoise(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

                freq = pow(2.0, float(1));
                amp = pow(0.5, float(3-1));
                t += unity_valueNoise(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

                freq = pow(2.0, float(2));
                amp = pow(0.5, float(3-2));
                t += unity_valueNoise(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

                return t;
            }

            float inverseLerp(float A, float B, float T){
                return (T - A)/(B - A);
            }

            float2 rotateRad(float2 uv, float2 center, float rotation)
            {
                uv -= center;
                float s = sin(rotation);
                float c = cos(rotation);
                float2x2 rMatrix = float2x2(c, -s, s, c);
                rMatrix *= 0.5;
                rMatrix += 0.5;
                rMatrix = rMatrix * 2 - 1;
                uv.xy = mul(uv.xy, rMatrix);
                uv += center;
                return uv;
            }
            // ---------------------------------------------------------

            sampler2D _PixelizedTexture;
            float _CarSpeed;

            float4 frag (v2f i) : SV_Target
            {
                // If speed = 0, due to the random nature of the noise generator, we might get some small random lines when the car is meant to be stopped
                if(_CarSpeed <= 0){
                    return tex2D(_PixelizedTexture, i.uv);
                }

                float2 rotationSpeed = 1.0f;

                // noise pattern
                float2 center = float2(0.5, 0.5);
                float radialScale = 0.05f;
                float lengthScale = 20.0f;
                float2 delta = i.uv - center;
                float radius = length(delta) * 2 * radialScale;
                float angle = atan2(delta.x, delta.y) * 1.0/6.28 * lengthScale;
                float2 polarCoords = float2(radius, angle);
                float2 rotatedPolarCoords = rotateRad(polarCoords, center, _Time * rotationSpeed);

                float noiseScale = 100;
                float noiseVal = Unity_SimpleNoise_float(rotatedPolarCoords, noiseScale);
                // ---------------------------------------------------------

                // mask
                float centerMaskSize = 0.06;
                float centerMaskEdge = clamp(1.0 - _CarSpeed/5.0, 0.5, 1.0);
                float distanceToCenter = distance(center, i.uv);
                float cleanMask = inverseLerp(centerMaskSize, centerMaskSize + centerMaskEdge, distanceToCenter);

                float lineDensity = 0.5;
                float lineMul = mul(cleanMask, lineDensity);
                float lineMax = 1 - lineMul;

                float lineFalloff = 0.25;
                float lineMin = lineMax + lineFalloff;
                float finalMask = smoothstep(lineMax, lineMin, noiseVal);
                float4 finalMask4D = float4(finalMask, finalMask, finalMask, 1.0f);

                float4 sampleRes = tex2D(_PixelizedTexture, i.uv);
                float4 maskRemoveColor = float4(1.0f, 1.0f, 1.0f, 1.0f);

                float4 cleanedSample = lerp(sampleRes, maskRemoveColor, finalMask4D);

                return cleanedSample;
            }
            
            ENDCG
        }
    }
}
