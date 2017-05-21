Shader "Custom/Emile/GrabPassInvert"
{

	// Variables
	Properties{
		_Color("Foreground Color", Color) = (1,1,1,1)
		_BGColor("Background Color", Color) = (0,0,0,0)
		_InvertScale ("Inverts Color (0 no, 1, yes)", Range (0,1)) = 0 // sliders
	}

	SubShader
    {
        // Draw ourselves after all opaque geometry
        Tags { "Queue" = "Transparent" }

        // Grab the screen behind the object into _BackgroundTexture
        GrabPass
        {
            "_BackgroundTexture"
        }

        // Render the object with the texture generated above, and invert the colors
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _Color;
             float4 _BGColor;

            struct v2f
            {
                float4 grabPos : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata_base v) {
                v2f o;
                // use UnityObjectToClipPos from UnityCG.cginc to calculate 
                // the clip-space of the vertex
                o.pos = UnityObjectToClipPos(v.vertex);
                // use ComputeGrabScreenPos function from UnityCG.cginc
                // to get the correct texture coordinate
                o.grabPos = ComputeGrabScreenPos(o.pos);
                return o;
            }

            sampler2D _BackgroundTexture;

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 bgcolor = tex2Dproj(_BackgroundTexture, i.grabPos);
//                return 1 - bgcolor;


				// if bgcolor = _BGColor == 0, render the foreground color else render the background color
				return (_BGColor * (_BGColor - bgcolor)) + (_Color * (1-(floor(_BGColor - bgcolor + 0.5))));

				// if background alpha is 0 -> return foreground color if alpha is 1 -> return background color
//				return (_BGColor * bgcolor.w) + (_Color * (1-bgcolor.w));

				
            }
            ENDCG
        }

    }
}
