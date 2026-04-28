Shader "Custom/PureComicStyle"
{
    Properties
    {
        _MainTex ("原图", 2D) = "white" {}
        _Threshold ("黑白阈值", Range(0,1)) = 0.5 // 控制黑白分界（0.5为中间值）
        _LineStrength ("线条强度", Range(0,5)) = 1.5 // 漫画描边粗细
        _DotSize ("网点大小", Range(50, 300)) = 100 // 网点密度（值越大网点越细）
        _DotDensity ("网点浓度", Range(0.3, 0.7)) = 0.5 // 网点填充比例
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent" }

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
                float4 pos : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Threshold;
            float _LineStrength;
            float _DotSize;
            float _DotDensity;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            // 生成漫画网点图案（无动态，纯静态）
            float staticDotPattern(float2 uv)
            {
                // 固定网格网点，无时间变化
                float2 grid = frac(uv * _DotSize);
                float dist = distance(grid, float2(0.5, 0.5)); // 圆形网点
                return step(_DotDensity, 1 - dist * 2); // 固定浓度
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 1. 采样原图
                fixed4 col = tex2D(_MainTex, i.uv);

                // 2. 转为灰度（漫画基础）
                float gray = dot(col.rgb, float3(0.299, 0.587, 0.114));

                // 3. 硬切割黑白（无过渡，纯黑白）
                float bw = step(_Threshold, gray);

                // 4. 强化边缘线条（漫画描边）
                float edgeX = abs(ddx(gray)) * _LineStrength;
                float edgeY = abs(ddy(gray)) * _LineStrength;
                float edge = step(0.05, edgeX + edgeY); // 边缘强制为黑
                bw = min(bw, 1 - edge); // 线条覆盖在黑白上

                // 5. 叠加静态网点（无动态闪烁）
                float dots = staticDotPattern(i.uv);
                // 暗部加网点，亮部保持纯白
                bw = lerp(bw * dots, bw, bw * 0.8); // 网点只影响暗部

                return fixed4(bw, bw, bw, col.a);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}