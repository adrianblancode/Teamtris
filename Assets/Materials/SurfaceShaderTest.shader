//#    <!--
//#    Surface shader test
//#
//#    Author : lemoele
//#	   Created on 2015-10-10
//#	   -->

Shader "Custom/SurfaceShaderTest" {

	Properties {
		_Color ("Color", Color) = (0.5,0.5,0.5,1)
		_Color2 ("Color2", Color) = (0.5,0.5,0.5,1)
		_Color3 ("Color3", Color) = (0.5,0.5,0.5,1)
		_Color4 ("Color4", Color) = (0.5,0.5,0.5,1)
		_BorderColor ("Border Color", Color) = (1,1,1,1)
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
      	_Step ("Step", Range(0.001, 0.01)) = 0.01
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows vertex:vert
		#pragma target 3.0

		struct Input {
			float3 customColor;
			float3 worldPos;
		};

		float4 _Color;
		float4 _Color2;
		float4 _Color3;
		float4 _Color4;
      	float4 _BorderColor;
		half _Glossiness;
		half _Metallic;
		float3 _Center;
		float _Radius;
      	float _Step;

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		//							BORDER EFFECTS : NOT USED
		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		
		void circle (Input IN, inout SurfaceOutputStandard o) {
			float d = distance(_Center, IN.worldPos);
			float dN = 1 - saturate(d / _Radius);
			
			if (dN > 0.25 && dN < 0.3)
				o.Albedo = half3(1,1,1);
			else
				o.Albedo = _Color.rgb;
		}

		float norm (float x, float y, float z, float norm) {
			float power = pow(abs(x), norm) + pow(abs(y), norm) + pow(abs(z), norm);
			return pow(power, 1/norm);
		}

		void colorBorder (inout SurfaceOutputStandard o, float4 c, float a1, float a2, float n) {
			a1 += _Step;
			a2 += _Step;
			if (n > a1 && n < a2) {
				o.Albedo = c.rgb;
				o.Alpha = c.a;
			}
		}

		void borders (Input IN, inout SurfaceOutputStandard o) {
			float3 pix = IN.worldPos - _Center;
			float n = norm (pix.x, pix.y, pix.z, 10);
			float a1 = 0.45;
			float a2 = 0.46;
			
			o.Albedo = (1, 1, 1);
			o.Alpha = 0;
				
			if (n > 0 && n < a2) {
				o.Albedo = _Color.rgb;
				o.Alpha = _Color.a;
			}
			colorBorder(o, _Color2, a1, a2, n);
			colorBorder(o, _Color3, a1, a2, n);
			colorBorder(o, _Color4, a1, a2, n);
		}

		void whiteBorder (Input IN, inout SurfaceOutputStandard o) {
			float3 pix = IN.worldPos - _Center;
			float n = norm (pix.x, pix.y, pix.z, 10);
			float a1 = 0.43;
			float a2 = 0.47;
			
			o.Albedo = _Color.rgb;
			o.Alpha = 0;
				
			if (n > a1 && n < a2) {
				o.Albedo = _Color.rgb;
				o.Alpha = _Color.a;
			}
		}
		
		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		//									USEFUL FUNCTIONS
		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		
		// Return decimal part of a number
		float fract(float num) {
			return (num - floor(num));
		}
		
		float2 fract(float2 num) {
			return float2(num.x - floor(num.y), num.x - floor(num.y));
		}
		
		// Linear interpolation of a between x and y
		float mix(float x, float y, float a) {
			return (x*(1-a) + y*a);
		}
		
		// Pseudo-random generator
		float random(float p) {
		  	return fract(sin(p)*10000.);
		}
		
		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		//									NOISE EFFECT
		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		
		float noise(float2 p) {
		  	return random(p.x + p.y*10000.);
		}
		
		float stepNoise(float2 p) {
		  	return noise(floor(p));
		}

		float2 sw(float2 p) {return float2( floor(p.x) , floor(p.y) );}
		float2 se(float2 p) {return float2( ceil(p.x)  , floor(p.y) );}
		float2 nw(float2 p) {return float2( floor(p.x) , ceil(p.y)  );}
		float2 ne(float2 p) {return float2( ceil(p.x)  , ceil(p.y)  );}

		float smoothNoise(float2 p) {
		  	float2 inter = smoothstep(0., 1., fract(p));
		  	float s = mix(noise(sw(p)), noise(se(p)), inter.x);
		  	float n = mix(noise(nw(p)), noise(ne(p)), inter.x);
		  	return mix(s, n, inter.y);
		  	return noise(nw(p));
		}
		
		float fractalNoise(float2 p) {
		  	float total = 0.0;
		  	total += smoothNoise(p);
		  	total += smoothNoise(p*2.) / 2.;
		  	total += smoothNoise(p*4.) / 4.;
		  	total += smoothNoise(p*8.) / 8.;
		  	total += smoothNoise(p*16.) / 16.;
		  	total /= 1. + 1./2. + 1./4. + 1./8. + 1./16.;
		  	return total;
		}

		float movingNoise(float2 p, float time) {
		  	float total = 0.0;
		  	total += smoothNoise(p     - time);
		  	total += smoothNoise(p*2.  + time) / 2.;
		  	total += smoothNoise(p*4.  - time) / 4.;
		  	total += smoothNoise(p*8.  + time) / 8.;
		  	total += smoothNoise(p*16. - time) / 16.;
		  	total /= 1. + 1./2. + 1./4. + 1./8. + 1./16.;
		  	return total;
		}
		
		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		//								VERTEX AND PIXEL COMPUTING
		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		
		float3 trans (float3 pos, float time) {
			float rand1 = random(pos.x * time);
			float rand2 = random(pos.y * time);
			float rand3 = random(pos.z * time);
			float3 translate = float3(rand1*sin(20*time), rand2*cos(20*time), rand3*sin(20*time));
			return translate/10;
		}
		
		void vert (inout appdata_full v, out Input o) {
			float time = _Time[1];
			v.vertex.xyz += trans(v.vertex.xyz, time);
			
          	UNITY_INITIALIZE_OUTPUT(Input,o);
//		  	float2 p = float2(v.vertex.x, v.vertex.y) * 4.;
//		  	float brightness = smoothNoise(p);
//		  	float brightness = fractalNoise(p);
//		  	float brightness = movingNoise(p, time);
//		  	o.customColor = float3(brightness);
		}
		
		void surf (Input IN, inout SurfaceOutputStandard o) {
			o.Albedo = _Color.rgb;
//			o.Alpha = _Color.a;

//			float time = _Time[1];
//		  	float2 p = float2(IN.worldPos.x, IN.worldPos.y) * 40.;
//		  	float brightness = stepNoise(p);
//		  	float brightness = smoothNoise(p);
//		  	float brightness = fractalNoise(p);
//		  	float brightness = movingNoise(p, time);
//		  	float3 customColor = float3(brightness);

//			o.Albedo = customColor * _Color.rgb;
			o.Alpha = 1.0;
						
//			circle(IN, o);
//			borders(IN, o);
//			whiteBorder(IN, o);

			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
