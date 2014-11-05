//-----------------------------------------------------------------------------
// Debug Draw
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Globals
//-----------------------------------------------------------------------------
uniform const float3	DiffuseColor	: register(c0) = 1;
uniform const float		Alpha			: register(c1) = 1;
uniform const float4x4	World		: register(vs, c2);		//  2- 5
uniform const float4x4	View		: register(vs, c6);		//  6- 9
uniform const float4x4	Projection	: register(vs, c10);	// 10-13

//-----------------------------------------------------------------------------
// Shader I/O structures
//-----------------------------------------------------------------------------

struct VSInput
{
	float4	Position	: POSITION;
	float4	Color		: COLOR;
};

struct VSOutputBasic
{
	float4	PositionPS	: POSITION;		// Position in projection space
	float4	Color		: COLOR;
};

//-----------------------------------------------------------------------------
// Pixel shader inputs
//-----------------------------------------------------------------------------

struct PSInputBasic
{
	float4	Color		: COLOR0;
};

//-----------------------------------------------------------------------------
// Vertex shaders
//-----------------------------------------------------------------------------

VSOutputBasic VSBasic(VSInput vin)
{
	VSOutputBasic vout;
	
	float4 pos_ws = mul(vin.Position, World);
	float4 pos_vs = mul(pos_ws, View);
	float4 pos_ps = mul(pos_vs, Projection);
	
	vout.PositionPS = pos_ps;
	vout.Color = vin.Color;
	
	return vout;
}

//-----------------------------------------------------------------------------
// Pixel shaders
//-----------------------------------------------------------------------------
float4 PSBasic(PSInputBasic pin) : COLOR
{
	float4 color;
	color.rgb = pin.Color;
	color.a = Alpha;
	return color;
}

//-----------------------------------------------------------------------------
// Shader and technique definitions
//-----------------------------------------------------------------------------

Technique BasicEffect
{
	Pass
	{
		VertexShader = compile vs_1_1 VSBasic();
		PixelShader	 = compile ps_1_1 PSBasic();
	}
}
