Shader "Custom/GridColorShader64_Transparent"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GridSize ("Grid Size", Float) = 8
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            float _GridSize;

            // Define the 64 colors as a buffer
            StructuredBuffer<float4> _Colors;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                // Calculate the grid cell indices
                float2 gridCoords = floor(i.uv * _GridSize);
                int gridIndex = clamp((int)(gridCoords.y * _GridSize + gridCoords.x), 0, 1023);

                // Get the color from the 64-color array
                float4 gridColor = _Colors[gridIndex];

                // Sample the texture and blend it with the grid color
                float4 texColor = tex2D(_MainTex, i.uv);
                float4 outputColor = texColor * gridColor;

                // Ensure proper transparency by keeping alpha from texture
                outputColor.a = texColor.a;

                return outputColor;
            }
            ENDCG
        }
    }
}
