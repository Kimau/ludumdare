Shader "Minimal/FlatTex" {
	Properties {
		_MainTex ("Main Texutre", 2D) = "white" {}
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
		sampler2D _MainTex;
		
		v2f basicVert(appdata_base v)
		{
		    v2f o;
		    o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		    o.uv = v.texcoord;
		    return o;
		}
		
		half4 basicFrag(v2f i) : COLOR
		{
			half4 texColour = tex2D(_MainTex, i.uv);
			texColour *= _FlatColour;
			clip(texColour.a - 0.5f);
		    return texColour;
		}
		ENDCG
		}
	} 
	FallBack "Diffuse"
}
