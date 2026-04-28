Shader "Custom/InvertCamera"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FlipX ("Flip X", Float) = 0 // 0 = 不翻转, 1 = 水平翻转
        _FlipY ("Flip Y", Float) = 0 // 0 = 不翻转, 1 = 垂直翻转
        _InvertColors ("Invert Colors", Float) = 1 // 0 = 不反相, 1 = 颜色反相
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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
            float4 _MainTex_ST;
            float _FlipX;
            float _FlipY;
            float _InvertColors;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                // 处理水平翻转
                if (_FlipX > 0.5)
                    o.uv.x = 1 - o.uv.x;
                    
                // 处理垂直翻转
                if (_FlipY > 0.5)
                    o.uv.y = 1 - o.uv.y;
                    
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // 处理颜色反相
                if (_InvertColors > 0.5)
                    col.rgb = 1 - col.rgb;
                    
                return col;
            }
            ENDCG
        }
    }
}
