Shader "Custom/TerrainColor"
{
    Properties
    {

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
// Upgrade NOTE: excluded shader from DX11, OpenGL ES 2.0 because it uses unsized arrays
#pragma exclude_renderers d3d11 gles
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        
        float minHeight;
        float maxHeight;

        const static int maxColourCount = 8;
        int colourCount;
        float3 colours[maxColourCount];
        float startHeights[maxColourCount];


        struct Input
        {
            float3 worldPos;
        };

        float InverseLerp(float a, float b, float value) {
            return saturate((value - a) / (b - a));
        }

        UNITY_INSTANCING_BUFFER_START(Props)

        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float heightPercent = InverseLerp(minHeight, maxHeight, IN.worldPos.y);
            /*for (int i = 0; i < colourCount; i++)
            {
                float drawStrength = saturate(sign(heightPercent - startHeights[i]));
                o.Albedo = colours[i];
            }*/
            o.Albedo = colours[1];
        }
        ENDCG
    }
    FallBack "Diffuse"
}
