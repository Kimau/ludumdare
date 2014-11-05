Shader "Karakuro/Input Block" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)   	
		_MainTex ("Source Image (RGB)", 2D) = "" {}
		_BlockMask ("Block Mask", int) = 511
	}
	
	// Shader code pasted into all further CGPROGRAM blocks
	CGINCLUDE

	#pragma fragmentoption ARB_precision_hint_fastest
	
	#include "UnityCG.cginc"
	
	struct v2f {
		float4 pos : POSITION;
		half2 uv : TEXCOORD0;
	};
	
	float4 _Color;
	sampler2D _MainTex;
	int _BlockMask;
	
	v2f vert( appdata_img v ) 
	{
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = v.texcoord.xy;
		return o;
	} 
		
	fixed4 frag(v2f i) : COLOR 
	{
		float2 pos = float2(1.0f - floor(i.uv.x * 3.0f)*0.5f, 1.0f - floor(i.uv.y * 3.0f)*0.5f);
		
		
		float numID = pow(2,pos.x*2.0f + pos.y*6.0f);
		
		float x = fmod(floor(_BlockMask / numID),2.0f);
		
		fixed4 texColor = tex2D(_MainTex, i.uv);
		//return fixed4(x,0,0,1);
		return texColor * fixed4(x,x,x,1.0f) * _Color;
	}

	ENDCG 
	
Subshader {
 Pass {
	  Fog { Mode off }      

      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      ENDCG
  }
}
	Fallback off
} // shader