Shader "Piece/BaseShader" 
{
	Properties 
	{
	  _MainTex ("Texture", 2D) = "white" {}
	}
	
	SubShader 
	{
		Pass {
			Fog { Mode Off }
			Tags { "RenderType" = "Opaque" }
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			struct appdata 
			{
				float4 vertex : POSITION;
				float4 color : COLOR0;
			};
			
			struct v2f 
			{
				float4 pos : SV_POSITION;
				float3 color : COLOR0;
			};	
			
			sampler2D _MainTex;
			
			v2f vert (appdata v)
			{
			    v2f o;
			    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			    o.color = v.color;
			    return o;
			}
			
			half4 frag(v2f i) : COLOR
			{
			    return half4 (i.color, 1);
			}
	
			ENDCG
		}
	} 
	
	Fallback "Diffuse"
}
