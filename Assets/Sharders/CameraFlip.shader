Shader "Unlit/CameraFlip" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _FlipX ("Flip Horizontal", Int) = 0 // 0=不翻转，1=翻转
        _FlipY ("Flip Vertical", Int) = 0  // 0=不翻转，1=翻转
    }
    SubShader {
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            int _FlipX;
            int _FlipY;

            v2f vert (appdata_base v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                // 水平翻转：UV.x取1 - 原UV.x
                o.uv.x = _FlipX ? (1 - o.uv.x) : o.uv.x;
                // 垂直翻转：UV.y取1 - 原UV.y
                o.uv.y = _FlipY ? (1 - o.uv.y) : o.uv.y;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}
