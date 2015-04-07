Shader "Minimal/Outline" {
	Properties {
		_OutlineWidth ("Outline Width", Range(0,1)) = 0.1
		_FlatColour ("Colour", Color) = (1,0,0,1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		Pass {
		AlphaTest Greater 0.5
		ZWrite On
		ZTest Less 
		
		CGPROGRAM
		#pragma vertex basicVert
		#pragma fragment basicFrag
		#include "UnityCG.cginc"
		
		struct v2f {
		    float4 pos : SV_POSITION;
		    float2 uv : TEXCOORD0;
		};
		
		float4 _FlatColour;
		float _OutlineWidth;
		
		v2f basicVert(appdata_base v)
		{
		    v2f o;
		    o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		    o.uv = v.texcoord;
		    return o;
		}
		
		half4 basicFrag(v2f i) : COLOR
		{
			half4 texColour = _FlatColour;
			texColour.a = 
				step(frac(i.uv.x + _OutlineWidth), _OutlineWidth * 2.0f) + 
				step(frac(i.uv.y + _OutlineWidth), _OutlineWidth * 2.0f);
			clip(texColour.a - 0.5);
		    return texColour;
		}
		ENDCG
		}
	} 
	FallBack "Diffuse"
}
