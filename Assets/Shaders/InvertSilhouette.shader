// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Emile/InvertSilhouette" {

	// Variables
	Properties{
		_MainTexture("Main Color (RGB)", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
	}

	SubShader{
		Pass{
			CGPROGRAM

			// define vertex and frag functions
			#pragma vertex vertexFunction
			#pragma fragment fragmentFunction

			// unity built-in function with math libs and useful things
			#include "UnityCG.cginc"

			// importing variables into the CG part of the program
			float4 _Color;
			sampler2D _MainTexture;

			// get data from objec
			// Normal
			// Color
			// uvs
			// etc
			struct appdata{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};


			struct v2f{
				float4 position: SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			// builds the object
			v2f vertexFunction(appdata IN){

				v2f OUT;

				// check nvidia docs for function docs
				// mul() -> multiply
				// UNITY_MATRIX_MVP -> takes perspective of camera into account
				OUT.position = UnityObjectToClipPos(IN.vertex);

				// just pass the uvs across to frag function
				OUT.uv = IN.uv;

				return OUT;

			}
		
			// - color the object in
			// just return a color per pixel on the screen
			fixed4 fragmentFunction(v2f IN) : SV_Target{

				float4 textureColor = tex2D(_MainTexture, IN.uv);// this wraps the texture according to the uvs passedinto from the vertex function

				return textureColor * _Color;// just multiplying the texture with the color
			}

			ENDCG
		}
	}

}