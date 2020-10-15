Shader "Custom/TerrainShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        float minHeight;
        float maxHeight;

        const static int maxColourCount = 9;
        const static int epsilon = 1E-4;

        int colourCount;
        float3 colours[maxColourCount];
        float startHeights[maxColourCount];
        float blends[maxColourCount];

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        float InverseLerp(float a, float b, float value)
        {
            return saturate((value - a) / (b - a));
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float heightPercent = InverseLerp(minHeight, maxHeight, IN.worldPos.y);
            for (int i = 0; i < colourCount; i++)
            {
                float drawStrength = InverseLerp(-blends[i]/2 - epsilon,blends[i]/2, (heightPercent - startHeights[i]));
                //float drawStrength = saturate(sign(heightPercent - startHeights[i]));
                o.Albedo = (o.Albedo * (1 - drawStrength)) + colours[i] * drawStrength;
            }
            //o.Albedo = colours[0];
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
