Shader "LudumDare/Brush Renderer" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)    	
		_MainTex ("Base (RGB) Transparency (A)", 2D) = "" {}
	}
		
Subshader {
Tags { "Queue" = "AlphaTest"}
 Pass {
      ZWrite Off
 	  Blend SrcAlpha OneMinusSrcAlpha
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
		col.rgb -= float3(0.001, 0.001, 0.001);
		clip(col.rgb);
		col.a = min(min(col.r,col.g), col.b);
		col.r = 1.0;
		col.a = max(col.a, 0.2);
		return col;
	} 
      ENDCG
  }
}
	Fallback off
} // shader