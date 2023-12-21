Shader "Custom/SurfaceShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _MetallicMap ("Metallic Map", 2D) = "white" {}
        _GlossMap ("Smoothness Map", 2D) = "white" {}
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _HeightMap ("Height Map", 2D) = "white" {}
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
        sampler2D _MetallicMap;
        sampler2D _GlossMap;
        sampler2D _BumpMap;
        sampler2D _HeightMap;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Scale factor for texture coordinates
            float scale = 0.2;

            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.worldPos.xz * scale) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from texture maps
            o.Metallic = tex2D(_MetallicMap, IN.uv_MainTex).r;
            o.Smoothness = tex2D(_GlossMap, IN.uv_MainTex).r;
            // Normal map
            o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
            // Height map
            float height = tex2D(_HeightMap, IN.uv_MainTex).r;
            o.Normal += height * 0.1; // adjust the 0.1 to control the strength of the effect
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}