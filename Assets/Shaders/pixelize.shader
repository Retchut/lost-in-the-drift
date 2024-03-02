Shader "PostProcessing/pixelize"
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
                return float4(BoxSample(i.uv, _Upsample_Factor), 1.0f);
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
                return col;
            }
            ENDCG
        }
    }
}
