Shader "Custom/TransparentBlendShader"
{
    Properties
    {
        _MainTex ("Texture 1", 2D) = "white" {}
        _SecondTex ("Texture 2", 2D) = "white" {}
        _ThirdTex ("Texture 3", 2D) = "white" {}
        _Color1 ("Color 1", Color) = (1,1,1,1)
        _Color2 ("Color 2", Color) = (1,1,1,1)
        _Color3 ("Color 3", Color) = (1,1,1,1)
        _BlendFactor ("Blend Factor", Range(0,1)) = 0.5
        _Transparency ("Transparency", Range(0,1)) = 0.5
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _NormalStrength ("Normal Strength", Range(0,1)) = 1.0
        _yMin ("Y Min", Float) = -1.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        CGPROGRAM
        #pragma surface surf Standard alpha:blend
        
        sampler2D _MainTex;
        sampler2D _SecondTex;
        sampler2D _ThirdTex;
        sampler2D _NormalMap;
        float4 _Color1;
        float4 _Color2;
        float4 _Color3;
        float _BlendFactor;
        float _Transparency;
        float _NormalStrength;
        float _yMin;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            if (IN.worldPos.y < _yMin) discard;

            fixed4 col1 = tex2D(_MainTex, IN.uv_MainTex) * _Color1;
            fixed4 col2 = dot(tex2D(_SecondTex, IN.uv_MainTex) , _Color2);
            fixed4 col3 = tex2D(_ThirdTex, IN.uv_MainTex) * _Color3;
            
            fixed4 blendedColor = (col1.a == 0.0) ? col3 : (col1 * col2);
            blendedColor.a *= _Transparency;
            o.Albedo = blendedColor.rgb;
            o.Alpha = blendedColor.a;
            
            float3 normalSample = UnpackNormal(tex2D(_NormalMap, IN.uv_MainTex));
            normalSample.xy *= _NormalStrength;
            normalSample.z = sqrt(1.0 - saturate(dot(normalSample.xy, normalSample.xy)));
            o.Normal = normalSample;
        }
        ENDCG
    }
}
