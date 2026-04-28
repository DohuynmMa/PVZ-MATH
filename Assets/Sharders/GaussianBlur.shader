Shader "Unlit/GaussianBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Parameter ("Blur Parameters", Vector) = (2,2,2,2)
        _BlurSpread ("Blur Spread", Range(0.5, 3.0)) = 1.5
        _FrostedGlassIntensity ("Frosted Glass Intensity", Range(0, 1)) = 0.5 
        _BlurTex ("Blur Texture", 2D) = "white" {}
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        // 水平模糊通道
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv[9] : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float4 _Parameter;
            float _BlurSpread;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                
                float2 uv = v.uv;
                o.uv[0] = uv;
                
                #if UNITY_UV_STARTS_AT_TOP
                if (_MainTex_TexelSize.y < 0)
                    uv.y = 1 - uv.y;
                #endif
                
                o.uv[1] = uv + _MainTex_TexelSize.xy * _Parameter.xy * _BlurSpread * 0.5;
                o.uv[2] = uv - _MainTex_TexelSize.xy * _Parameter.xy * _BlurSpread * 0.5;
                o.uv[3] = uv + _MainTex_TexelSize.xy * _Parameter.xy * _BlurSpread * 1.5;
                o.uv[4] = uv - _MainTex_TexelSize.xy * _Parameter.xy * _BlurSpread * 1.5;
                o.uv[5] = uv + _MainTex_TexelSize.xy * _Parameter.xy * _BlurSpread * 2.5;
                o.uv[6] = uv - _MainTex_TexelSize.xy * _Parameter.xy * _BlurSpread * 2.5;
                o.uv[7] = uv + _MainTex_TexelSize.xy * _Parameter.xy * _BlurSpread * 3.5;
                o.uv[8] = uv - _MainTex_TexelSize.xy * _Parameter.xy * _BlurSpread * 3.5;
                
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 color = tex2D(_MainTex, i.uv[0]) * 0.31621;
                color += tex2D(_MainTex, i.uv[1]) * 0.27195;
                color += tex2D(_MainTex, i.uv[2]) * 0.27195;
                color += tex2D(_MainTex, i.uv[3]) * 0.07833;
                color += tex2D(_MainTex, i.uv[4]) * 0.07833;
                color += tex2D(_MainTex, i.uv[5]) * 0.01334;
                color += tex2D(_MainTex, i.uv[6]) * 0.01334;
                color += tex2D(_MainTex, i.uv[7]) * 0.00145;
                color += tex2D(_MainTex, i.uv[8]) * 0.00145;
                return color;
            }
            ENDCG
        }
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv[9] : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float4 _Parameter;
            float _BlurSpread;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                
                float2 uv = v.uv;
                o.uv[0] = uv;
                
                #if UNITY_UV_STARTS_AT_TOP
                if (_MainTex_TexelSize.y < 0)
                    uv.y = 1 - uv.y;
                #endif
                
                o.uv[1] = uv + _MainTex_TexelSize.xy * _Parameter.wz * _BlurSpread * 0.5;
                o.uv[2] = uv - _MainTex_TexelSize.xy * _Parameter.wz * _BlurSpread * 0.5;
                o.uv[3] = uv + _MainTex_TexelSize.xy * _Parameter.wz * _BlurSpread * 1.5;
                o.uv[4] = uv - _MainTex_TexelSize.xy * _Parameter.wz * _BlurSpread * 1.5;
                o.uv[5] = uv + _MainTex_TexelSize.xy * _Parameter.wz * _BlurSpread * 2.5;
                o.uv[6] = uv - _MainTex_TexelSize.xy * _Parameter.wz * _BlurSpread * 2.5;
                o.uv[7] = uv + _MainTex_TexelSize.xy * _Parameter.wz * _BlurSpread * 3.5;
                o.uv[8] = uv - _MainTex_TexelSize.xy * _Parameter.wz * _BlurSpread * 3.5;
                
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 color = tex2D(_MainTex, i.uv[0]) * 0.31621;
                color += tex2D(_MainTex, i.uv[1]) * 0.27195;
                color += tex2D(_MainTex, i.uv[2]) * 0.27195;
                color += tex2D(_MainTex, i.uv[3]) * 0.07833;
                color += tex2D(_MainTex, i.uv[4]) * 0.07833;
                color += tex2D(_MainTex, i.uv[5]) * 0.01334;
                color += tex2D(_MainTex, i.uv[6]) * 0.01334;
                color += tex2D(_MainTex, i.uv[7]) * 0.00145;
                color += tex2D(_MainTex, i.uv[8]) * 0.00145;
                return color;
            }
            ENDCG
        }
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
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
            sampler2D _BlurTex;
            float _FrostedGlassIntensity;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 original = tex2D(_MainTex, i.uv);
                
                fixed4 blurred = tex2D(_BlurTex, i.uv);
                
                fixed4 finalColor = lerp(original, blurred, _FrostedGlassIntensity);
                
                return finalColor;
            }
            ENDCG
        }
    }
}    