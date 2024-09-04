Shader "Custom/WorldCoordinatesUV" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_Zoom ("Zoom", Range(0.1,100)) = 0.0
		_OffsetX("Offset X", Float) = 0.0
		_OffsetY("Offset Y", Float) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		ZWrite On
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		half _Zoom;
		float _OffsetX;
		float _OffsetY;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			float2 coord;
			coord[0] = IN.worldPos.x;
			coord[1] = IN.worldPos.y;
			fixed4 c = float4(0,0,0,0);//tex2D (_MainTex, coord / _Zoom) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Emission = (tex2D (_MainTex, (coord + float2(_OffsetX, _OffsetY)) / _Zoom) * _Color).rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
