// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "DoS/BoldOutline" {
	Properties {
		_Color("Color", Color) = (1, 1, 1, 1)
		_OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
		_OutlineThickness("Outline Thickness", Range(0, 1)) = 0.05
		_EmitColors("Emission", Range(0, 1)) = 0
		_MainTex("Texture", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0, 1)) = 0.5
		_Metallic("Metallic", Range(0, 1)) = 0.0
	}

	SubShader {
		Tags {
			"RenderType" = "Transparent"
			"Queue" = "Transparent"
		}

		LOD 100

		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard fullforwardshadows alpha:fade

		sampler2D _MainTex;
		fixed4 _Color;
		fixed4 _OutlineColor;
		half _OutlineThickness;
		half _EmitColors;
		half _Glossiness;
		half _Metallic;

		struct Input {
			float2 uv_MainTex;
		};

		void surf(Input input, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D(_MainTex, input.uv_MainTex) * _OutlineColor;
			o.Albedo = c.rgb;
			o.Emission = c.rgba * _EmitColors;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG

		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard fullforwardshadows alpha:fade vertex:vert

		sampler2D _MainTex;
		float4 _MainTex_TexelSize;
		fixed4 _Color;
		fixed4 _OutlineColor;
		half _OutlineThickness;
		half _EmitColors;
		half _Glossiness;
		half _Metallic;

		struct Input {
			float2 uv_MainTex;
		};

		void vert(inout appdata_full v) {
			v.vertex.xy *= (1 - _OutlineThickness);
		}

		void surf(Input input, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D(_MainTex, input.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Emission = c.rgba * _EmitColors;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}

	FallBack "Diffuse"
}
