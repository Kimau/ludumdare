//--------------------------------------------------------------------------------------
// File: DebugLine.fx
//
//--------------------------------------------------------------------------------------
// blatantly stolen from Simon http://www.sjbrown.co.uk/?article=directx_texels for details

// this constant and function live in a global HLSL include
uniform float2 texel_offset : register( c0 );
float4 final_position( float4 screen_pos )
{
    float4 result = screen_pos;
    result.xy += texel_offset * result.ww;
    return result;
}\

//--------------------------------------------------------------------------------------
// Global variables
//--------------------------------------------------------------------------------------
shared float4x4 g_mProjection : Projection;						// Projection matrix

void V_WorldDebugLine(	float4 vPos : POSITION,
					float4 vColour : COLOR0,
					out float4 oColour : COLOR0,
					out float4 oPos : POSITION )
{
	// Transform the pre-projection space to screen space
	oPos = mul( vPos, g_mProjection );
	oPos = final_position( oPos );
    oColour = vColour;
}

void V_NDCDebugLine(	float4 vPos : POSITION,
						float4 vColour : COLOR0,
						out float4 oColour : COLOR0,
						out float4 oPos : POSITION )
{
	// TODO Aspect ratio correction?
	oPos = vPos;
	oPos = final_position( oPos );
    oColour = vColour;
}


float4 P_DebugLine( float4 Colour : COLOR0 ) : COLOR0
{
    return Colour;
}


//--------------------------------------------------------------------------------------
// Techniques
//--------------------------------------------------------------------------------------
technique SM1_1
{
	// World Space Lines
    pass P0
    {
		ZEnable = true;
        VertexShader = compile vs_1_1 V_WorldDebugLine();
        PixelShader  = compile ps_1_1 P_DebugLine();
    }
    // NDC Space lines
    pass P1
    {
		ZEnable = false;
        VertexShader = compile vs_1_1 V_NDCDebugLine();
        PixelShader  = compile ps_1_1 P_DebugLine();
    }

}
