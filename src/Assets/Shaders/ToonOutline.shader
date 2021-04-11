// Based on implementation of toon outline shading from Unity Standard Assets
// and http://rbwhitaker.wikidot.com/toon-shader for explanation of technique

Shader "Toon/Outline" {
	Properties {
		_OutlineColor ("Outline Color", Color) = (0,0,0,1)
		_Outline ("Outline width", Range (.002, 0.03)) = .005
	}
	
	CGINCLUDE
	#include "UnityCG.cginc"
	
	// For storing information between the application and the vertex shader
	struct vertIn {
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};

	// For storing information between the vertex shader and fragment shader
	struct vertOut {
		float4 vertex : SV_POSITION;
		fixed4 outlineColour : COLOR;
	};
	
	// Width and colour of the outline
	uniform float _Outline;
	uniform float4 _OutlineColor;
	
	// Vertex shader
	vertOut vert(vertIn v) {
		vertOut o;

		// Converts vertex from object space into worldspace
		o.vertex = UnityObjectToClipPos(v.vertex);

		// convert the vertex normal into world coordinates
		float3 worldNormal = normalize(mul(transpose((float3x3)unity_WorldToObject), v.normal.xyz));

		// Convert the normal into screen coordinates
		float2 offset = TransformViewToProjection(worldNormal.xy);

		// Draw a coloured outline in direction of the normal
		// with constant thickness; independent of distance from camera
		o.vertex.xy += offset * UNITY_Z_0_FAR_FROM_CLIPSPACE(o.vertex.z) * _Outline;
		o.outlineColour = _OutlineColor;
		return o;
	}
	ENDCG

	SubShader {
		Tags { "RenderType"="Opaque" }
		// This OUTLINE pass is called from Phong Shader
		Pass {
			Name "OUTLINE"
			//Tags { "LightMode" = "Always" }
			// Cull Front since we only want outline at edges
			// If we did Cull Back, the outline would cover everything in black
			Cull Front

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// All logic completed in vertex shader, no need for further fragment shader logic
			fixed4 frag(vertOut i) : SV_Target
			{
				return i.outlineColour;
			}
			ENDCG
		}
	}	
}
