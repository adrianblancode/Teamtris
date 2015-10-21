Shader "Custom/CustomCRTShader" {
	Properties {
		_MainTex ("", 2D) = "white" {}
	}

	SubShader {

	ZTest Always Cull Off ZWrite Off Fog { Mode Off } //Rendering settings

		Pass {
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"

			// Data structure to pass information from vertex shader to fragment shader
			struct v2f {
				float4 position : POSITION;
				float2 texCoord : TEXCOORD0;
			};

			v2f vert(appdata_base v) : POSITION {
				v2f output;
				output.position = mul (UNITY_MATRIX_MVP, v.vertex);
				output.texCoord = MultiplyUV (UNITY_MATRIX_TEXTURE0, v.texcoord.xy);
				return output;
			}
			
			sampler2D _MainTex; //Reference in Pass is necessary to let us use this variable in shaders

			// This function curves the texture coordinates like a CRT screen
			float2 curve(float2 texCoord)
			{
				// Normalize from [0, 1] to [-1, 1]
				texCoord = (texCoord - 0.5) * 2.0;
				
				// Magic formula to stretch the screen coordinates
				texCoord.x *= 1.0 + pow((abs(texCoord.y) / 6.0), 2.0);
				texCoord.y *= 1.0 + pow((abs(texCoord.x) / 5.0), 2.0);

				// Denormalize to [0, 1]
				texCoord = (texCoord / 2.0) + 0.5;
				
				// Then we stretch out the image properly
				texCoord = texCoord * 1.02;
				
				// And offset the position
				texCoord -= 0.01;
				
				return texCoord;
			}
			
			// Vignette effect with black around the edges
			fixed4 vignette(v2f input){
				float vignetteIntensity = 0.2;
				float vignetteBrightness = 20.0;
			
				// If either the horizontal or vertical position is out of bounds (due to our curving)
				// The vignette will return negative values which turns the pixels into black
				// The effect is softer near the edges
				float horizontal = input.texCoord.x  * (1.0 - input.texCoord.x);
				float vertical = input.texCoord.y * (1.0 - input.texCoord.y);
				float vignette = horizontal * vertical * vignetteBrightness;
				
				// If we are out of bounds on both horizontal and vertical, we need to readjust
				if(horizontal < 0 && vertical < 0){
					return pow(-vignette, vignetteIntensity);
				}
				
				return pow(vignette, vignetteIntensity);
			}
			
			// Generates horizontal scanlines with fourier transforms
			fixed4 horizontalScanlines(v2f input){
				float scanlineIntensity = 1.5;
			
				// We generate the scanline abberations through lightening and darkening based on screen coordinates
				// Instead of a sine wave, we use a fourier transform to emulate a square-like wave
				float scanlines = sin(input.texCoord.y * 1000.0 * scanlineIntensity)
					+ 0.13 * sin(input.texCoord.y * 1000.0 * scanlineIntensity * 3);

				return (1.0 + 0.2 * scanlines);
			}
			
			// Turns every other pixel line into black
			fixed4 verticalScanlines(v2f input){
				float darkenIntensity = 0.65;
				
				float hor = (input.position.x % 2.0);
				if(hor > 1.0){
					return 1.0;
				} else return 1.0 - darkenIntensity;
			}

			//Our Fragment Shader
			fixed4 frag (v2f input) : COLOR{
				fixed4 color = tex2D(_MainTex, input.texCoord); //Get the orginal rendered color

				input.texCoord = curve(input.texCoord);
				color *= vignette(input);
				color *= horizontalScanlines(input);
				color *= verticalScanlines(input);
			
				// The image is now darker so we increase brightness
				color *= 1.8f;

				return color;
			}

			ENDCG
		}
	}
}