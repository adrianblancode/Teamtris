Shader "Custom/CustomShader" {
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

            //Our Fragment Shader
			fixed4 frag (v2f input) : COLOR{
			
				float scanlineIntensity = 1.5;
				
				fixed4 color = tex2D(_MainTex, input.texCoord); //Get the orginal rendered color
				//input.texCoord = RadialDistortion(_MainTex)
				input.texCoord = curve(input.texCoord);

				// Vignette
				float vig = (0.0 + 1.0 * 16.0 * input.texCoord.x * input.texCoord.y *(1.0 - input.texCoord.x) * (1.0 - input.texCoord.y));
				color *= (pow(vig,0.3));

				// We generate the scanline abberations through lightening and darkening based on screen coordinates
				// Instead of a sine wave, we use a fourier transform to emulate a square-like wave
				float scanlines = sin(input.texCoord.y * 1000.0 * scanlineIntensity)
					+ 0.13 * sin(input.texCoord.y * 1000.0 * scanlineIntensity * 3);

				color = color * (1.0 + 0.3 * scanlines);
				
				// When we curved the image, some of the texture coordinates will reach out of bounds
				// These fragments, we simply color them black
				if (input.texCoord.x > 1.0 || input.texCoord.y > 1.0){
					color *= 0.0;
				}
				if (input.texCoord.x < 0.0 || input.texCoord.y < 0.0) {
					color *= 0.0;
				}
				
				// Verical scanlines
				color *= 1.0 - 0.65 * (clamp(((input.position.x % 2.0) - 1.0) * 2.0, 0.0, 1.0));

				//fixed4(pow(res, fixed3(1.0f / gamma, 1.0f / gamma, 1.0f / gamma)), col.a) * col.a;
			
				return color * 1.5f;
			}
			
            ENDCG
        }
    }
}