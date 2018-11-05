// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "DualColor" {
	Properties{
		_MainTex("Diffuse Texture", 2D) = "white" {}
	}
		SubShader{
			Tags {"QUEUE" = "Transparent" "IGNOREPROJECTOR" = "true" "RenderType" = "Opaque"}

			Pass {
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
				CGPROGRAM

		//pragmas
		#pragma vertex vert
		#pragma fragment frag

		//user variables
		uniform sampler2D _MainTex;
		uniform half4 _MainTex_ST;

		//base input structs
		struct vertexInput {
			half4 vertex : POSITION;
			half4 texcoord : TEXCOORD0;
			half4 colors : COLOR;
		};

		struct vertexOutput {
			half4 pos : SV_POSITION;
			half4 tex : TEXCOORD0;
			half3 vertexColor : Color;
		};

		//vertex function
		vertexOutput vert(vertexInput v) {
			vertexOutput o;

			o.pos = UnityObjectToClipPos(v.vertex);
			o.tex = v.texcoord;
			o.vertexColor = v.colors.xyz;

			return o;
		}

		//fragment function
		half4 frag(vertexOutput i) : COLOR
		{
			half4 tex = half4(tex2D(_MainTex, _MainTex_ST.xy * i.tex.xy + _MainTex_ST.zw) * half4(i.vertexColor, 1.0));
			half alpha = tex.a;

			return half4(tex.xyz , alpha);
		}

		ENDCG
	}
	}
		//FallBack "Diffuse"
}