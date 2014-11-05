Shader "LudumDare/Block Renderer" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)    	
		_MainTex ("Source Image (RGB)", 2D) = "" {}
	}
		
Subshader {
 Pass {
	  Fog { Mode off }      

      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #pragma target 3.0
      
      #include "UnityCG.cginc"
      
      struct v2f { 
		float4 pos  : POSITION;
		fixed2 uv   : TEXCOORD0;
	}; 
	
	float4 _Color;
	sampler2D _MainTex;
	float4 _MainTex_ST;
	
	v2f vert( appdata_base v ) 
	{
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = (v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw);
		return o;
	} 
		
	fixed4 frag(v2f i) : COLOR  
	{
		float4 col = tex2D(_MainTex, i.uv) * _Color;  
		col.rgb = col.rgb * (cos(col.a * 50) * 0.25 + 0.75);
		return col;
	} 
      ENDCG
  }
}
	Fallback off
} // shader