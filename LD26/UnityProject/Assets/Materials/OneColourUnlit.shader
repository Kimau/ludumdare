Shader "Minimal/FlatColour" {
	Properties {
		_FlatColour ("Colour", Color) = (1,0,0,1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		Pass {
		
		AlphaTest Greater 0.05
		ZWrite On
		ZTest Less
		
		CGPROGRAM
		#pragma vertex basicVert
		#pragma fragment basicFrag
		#include "UnityCG.cginc"
		
		struct v2f {
		    float4 pos : SV_POSITION;
		    float2 sPos : TEXCOORD0;
		};
		
		float4 _FlatColour;
		
		v2f basicVert(appdata_base v)
		{
		    v2f o;
		    o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		    o.sPos = mul(UNITY_MATRIX_MVP, v.vertex).xy;
		    return o;
		}
		
		half4 basicFrag(v2f i) : COLOR
		{
			
			// float x = (sin(i.sPos.x * 400.0f) * 0.5f + 0.75f) * (cos(i.sPos.y * 225.0f) * 0.5f + 0.75f);		
			// half4 myCol = _FlatColour;
			// myCol.a *= (x);
			
		    return _FlatColour;
		}
		ENDCG
		}
	} 
	FallBack "Diffuse"
}
