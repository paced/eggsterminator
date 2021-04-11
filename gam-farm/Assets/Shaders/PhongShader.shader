// Original Cg/HLSL code stub copyright (c) 2010-2012 SharpDX - Alexandre Mutel
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 
// Adapted for COMP30019 by Jeremy Nicholson, 10 Sep 2012
// Adapted further by Chris Ewin, 23 Sep 2013
// Adapted further (again) by Alex Zable (port to Unity), 19 Aug 2016
// Adapted further (again (again)) for assignment 1, 09, September 2017

//UNITY_SHADER_NO_UPGRADE

Shader "Unlit/PhongShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_PointLightColor("Point Light Color", Color) = (0, 0, 0)
		_PointLightPosition("Point Light Position", Vector) = (0.0, 0.0, 0.0)
		_SpecularThreshold("Specular Threshold", Float) = 0 // Height above which there are specular reflections (for snow)
		_AmbientConstant ("Ambient Constant", Range(0.0, 2.0)) = 1.5
		_LambertianConstant("Lambertian Constant", Range(0.0, 2.0)) = 1.0
		_SpecularConstant("Specular Constant", Range(0.0, 100.0)) = 0.0
			// for the toonshading
		_Color("Main Color", Color) = (.5,.5,.5,1)
		_OutlineColor("Outline Color", Color) = (0,0,0,1)
		_Outline("Outline width", Range(.002, 0.03)) = .005
		_ToonShade("ToonShader Cubemap(RGB)", CUBE) = "" { }
	}
	SubShader
	{
		Pass
		{
            Tags { "LightMode" = "ForwardBase" }
            
			Name "BASE"
			CGPROGRAM
			// Defines functions used by vertex and fragment shaders
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			// Unity lib thta includes lighting shadow macros
            #include "AutoLight.cginc"

			// To pass in details of the light source and snow line
			uniform float3 _PointLightColor;
			uniform float3 _PointLightPosition;
			uniform float _SpecularThreshold;
			uniform float _AmbientConstant;
			uniform float _LambertianConstant;
			uniform float _SpecularConstant;

			// Texture to map to surface
			sampler2D _MainTex;
			float4 _MainTex_ST;

			// Input vertices
			struct vertIn
			{
				float4 vertex : POSITION;
				float4 normal : NORMAL;
				float2 uv : TEXCOORD0;
				float4 color : COLOR;
				LIGHTING_COORDS(3,4)
			};
			// Vertex Shader output, Fragment shader input
			struct vertOut
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
				float4 worldVertex : TEXCOORD1;
				float3 worldNormal : TEXCOORD2;
				LIGHTING_COORDS(3,4)
			};

			// Implementation of the vertex shader
			vertOut vert(vertIn v)
			{
				vertOut o;
				// Map texture to the vertex/polygon
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				// Map shadow to the vertex/polygon
				TRANSFER_VERTEX_TO_FRAGMENT(o);

				// Convert Vertex position and corresponding normal into world coords.
				// Note that we have to multiply the normal by the transposed inverse of the world 
				// transformation matrix (for cases where we have non-uniform scaling; we also don't
				// care about the "fourth" dimension, because translations don't affect the normal) 
				float4 worldVertex = mul(unity_ObjectToWorld, v.vertex);
				float3 worldNormal = normalize(mul(transpose((float3x3)unity_WorldToObject), v.normal.xyz));

				// Transform vertex in world coordinates to camera coordinates, and pass colour
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;

				// Pass out the world vertex position and world normal to be interpolated
				// in the fragment shader (and utilised)
				o.worldVertex = worldVertex;
				o.worldNormal = worldNormal;

				return o;
			}

			// Implementation of the fragment shader
			fixed4 frag(vertOut v) : SV_Target
			{
				// Colour of surface at this pixel is grabbed from a mapped texture
				v.color = tex2D(_MainTex, v.uv);

				// Our interpolated normal might not be of length 1
				float3 interpNormal = normalize(v.worldNormal);

				// Calculate ambient RGB intensities
				float Ka =  _AmbientConstant;
				// Combines the color of the surface with the ambient lighting
				float3 amb = v.color.rgb * UNITY_LIGHTMODEL_AMBIENT.rgb * Ka;

				// Calculating the incident ray
				float3 L = normalize(_PointLightPosition - v.worldVertex.xyz); 

				// Calculate diffuse RBG reflections, we save the results of L.N because we will use it again
				// (when calculating the reflected ray in our specular component)
				//float fAtt = 1; // Light source attenuation factor
				float fAtt = LIGHT_ATTENUATION(v); // LIGHT_ATTENUATION samples the shadowmap
				float Kd = _LambertianConstant;	// The Albedo for the lambertian component of light
				float LdotN = dot(L, interpNormal); // Calculating Cos(theta) between incident ray and normal
				float3 dif = fAtt * _PointLightColor.rgb * Kd * v.color.rgb * saturate(LdotN); // Set diffuse illumination

				// Calculate specular reflections
				float Ks = _SpecularConstant;   // Specular reflectivity
				// For this terrain, increase specular reflectivity above snow line
				if (v.worldVertex.y > _SpecularThreshold && _SpecularThreshold != 0) {
					Ks = 0.4;
				}

				// Calculate vector from viewer to surface
				float3 V = normalize(_WorldSpaceCameraPos - v.worldVertex.xyz);

				// Using Blinn-Phong approximation:
				float specN = 100; // We usually need a higher specular power when using Blinn-Phong
				float3 H = normalize(V + L); // Apply specular illumination
				float3 spe = fAtt * _PointLightColor.rgb * Ks * pow(saturate(dot(interpNormal, H)), specN);

				// Combine ambient, Lambertian and specular components
				float4 returnColor = float4(0.0f, 0.0f, 0.0f, 0.0f);
				returnColor.rgb = amb.rgb + dif.rgb + spe.rgb;
				returnColor.a = v.color.a;
				return returnColor;
			}
			ENDCG
		}

		UsePass "Toon/Outline/OUTLINE"

	}

	Fallback "VertexLit"
}
