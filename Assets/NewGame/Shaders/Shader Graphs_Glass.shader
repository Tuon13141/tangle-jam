Shader "Sprites/GlassBlackOutline_InnerEdgeAlphaBoost"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("Sprite Texture", 2D) = "white" {}

        _ColorGlass ("Glass Color", Color) = (1,1,1,1)
        _AlphaGlass ("Glass Alpha", Range(0,1)) = 0.8

        _ColorOutline ("Outline Color", Color) = (0,0,0,1)
        _AlphaOutLine ("Outline Alpha", Range(0,1)) = 1
        _DarkColorThreshold ("Dark Color Threshold", Range(0, 0.5)) = 0.05
        _OutlineOffset ("Outline Offset (XY)", Vector) = (0, 0.01, 0, 0)

        _InnerAlphaBoost ("Inner Edge Alpha Multiplier", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        // ---------- PASS 1: DRAW GLASS (with inner edge alpha boost) ----------
        Pass
        {
            Name "GLASS_PASS"
            Cull Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _ColorGlass;
            float _AlphaGlass;
            float _DarkColorThreshold;
            float _InnerAlphaBoost;
            float4 _OutlineOffset;
            float4 _MainTex_TexelSize;

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            bool IsOutline(float2 uv)
            {
                fixed4 t = tex2D(_MainTex, uv);
                float brightness = dot(t.rgb, float3(0.299, 0.587, 0.114));
                return (brightness < _DarkColorThreshold && t.a > 0.01);
            }

            bool HasPixel(float2 uv)
            {
                fixed4 t = tex2D(_MainTex, uv);
                return t.a > 0.01;
            }

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 pixelSize = float2(1.0 / _ScreenParams.x, 1.0 / _ScreenParams.y);
                fixed4 tex = tex2D(_MainTex, i.uv);

                float alpha = tex.a * _ColorGlass.a * _AlphaGlass;

                // Kiểm tra vùng viền bên trong
                if (IsOutline(i.uv) && tex.a > 0.01)
                {
                       bool nearOutline =
                             !HasPixel(i.uv + float2(0, pixelSize.y / _OutlineOffset.y));
                            //!IsOutline(i.uv + float2(pixelSize.x, 0)) &&
                            //!IsOutline(i.uv + float2(0, pixelSize.y));



                        if (nearOutline)
                        {
                            alpha = _InnerAlphaBoost;
                            //return fixed4(1,0,0,1); // Debug đỏ nếu cần
                        }  
                }

                return fixed4(tex.rgb * _ColorGlass.rgb, saturate(alpha));
            }
            ENDCG
        }

        // ---------- PASS 2: DRAW OUTLINE ----------
        Pass
        {
            Name "OUTLINE_PASS"
            Cull Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _ColorOutline;
            float _AlphaOutLine;
            float _DarkColorThreshold;
            float4 _OutlineOffset;

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            bool IsOutline(float2 uv)
            {
                fixed4 t = tex2D(_MainTex, uv);
                float brightness = dot(t.rgb, float3(0.299, 0.587, 0.114));
                return (brightness < _DarkColorThreshold && t.a > 0.01);
            }

            v2f vert(appdata_t v)
            {
                v2f o;
                float4 offsetPos = v.vertex + float4(_OutlineOffset.xy, 0, 0);
                o.vertex = UnityObjectToClipPos(offsetPos);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                if (IsOutline(i.uv))
                {
                    return fixed4(_ColorOutline.rgb, _AlphaOutLine);
                }

                return fixed4(0, 0, 0, 0);
            }
            ENDCG
        }
    }

    FallBack "Sprites/Default"
}
