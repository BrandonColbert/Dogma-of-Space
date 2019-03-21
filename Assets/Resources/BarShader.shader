// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "DoS/BarShader" {
	Properties {
		[PerRendererData] _MainTex ("Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		_Cutoff("Cutoff", float) = 1
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
	}
	
	SubShader {
		Tags {
			"Queue" = "Transparent" 
			"IgnoreProjector" = "True" 
			"RenderType" = "Transparent" 
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}
		
		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
			#include "UnityCG.cginc"

			struct Input {
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct Output {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			sampler2D _MainTex;
			fixed4 _Color;
			float _Cutoff;

			Output vert(Input input) {
				Output o;

				o.vertex = UnityObjectToClipPos(input.vertex);
				o.color = input.color;
				o.texcoord = input.texcoord;
#ifdef PIXELSNAP_ON
				o.vertex = UnityPixelSnap (OUT.vertex);
#endif

				return o;
			}

			float4 frag(Output output) : COLOR {
				fixed4 c = tex2D(_MainTex, output.texcoord) * output.color;
				c.rgb *= output.texcoord.x < _Cutoff ? c.a : 0;

				return c;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}