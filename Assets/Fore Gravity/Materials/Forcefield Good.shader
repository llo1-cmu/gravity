Shader "Custom/Forcefield Good"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Transparency("Transparency", Range(0.0,1)) = 1
		_Emission ("Emission", Color) = (0,0,0,0)
		_Period ("Period", Range(0,10)) = 1
		_AlphaBoost ("AlphaBoost", Range(0,1)) = 0.5
		_AlphaBias ("AlphaBias", Range(0,1)) = 0.1
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent" "IgnoreProjector"="true"}
        LOD 200
		Cull Off

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Lambert alpha //Change to lambert???

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        fixed4 _Color;
		fixed4 _Emission;
		float _Period;
		float _AlphaBoost;
		float _AlphaBias;


        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutput o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			float e = abs(sin(_Time * 4 * 3.1415 / _Period));
            o.Albedo = c.rgb;
			o.Alpha = c.a + (_AlphaBoost * e) - _AlphaBias ;
			o.Emission = _Emission * e;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
