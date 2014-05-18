cbuffer MatrixBuffer
{
	matrix worldMatrix;
	matrix viewMatrix;
	matrix projectionMatrix;
	matrix wvp;
};

DepthStencilState DSSDepthLessEqual
{
	DepthEnable = true;
	DepthWriteMask = 0x00;
	DepthFunc = Less_Equal;
};

DepthStencilState DSSDepthWriteLess
{
	DepthEnable = true;
	DepthWriteMask = All;
	DepthFunc = Less;
};

RasterizerState RSFill
{
	FillMode = SOLID;
	CullMode = None;
	DepthBias = false;
	MultisampleEnable = true;
};

BlendState BSBlending
{
	BlendEnable[0] = TRUE;
	SrcBlend = SRC_ALPHA;
	DestBlend = INV_SRC_ALPHA;
	BlendOp = ADD;
	SrcBlendAlpha = SRC_ALPHA;
	DestBlendAlpha = DEST_ALPHA;
	BlendOpAlpha = ADD;
	RenderTargetWriteMask[0] = 0x0F;
};

float4 LightVector = float4(0, 0, 1, 0);

int Tri = 1;
float LineWidth = 1.5;
float FadeDistance = 5.5;
float PatternPeriod = 1.5;

float4 FillColor = float4(1, 1, 1, 1);
float4 WireColor = float4(0, 0, 0, 1);
float4 PatternColor = float4(1, 1, 0.5, 1);

struct VS_INPUT
{
	float4 position : POSITION;
	float2 tex : TEXCOORD0;
	float3 normal: NORMAL;
	float3 tangent: TANGENT;
};

struct GS_INPUT
{
	float4 Pos  : POSITION;
	float4 PosV : TEXCOORD0;
};

struct PS_INPUT
{
	float4 Pos : SV_POSITION;
	float4 Col : TEXCOORD0;
};

struct PS_INPUT_WIRE
{
	float4 Pos : SV_POSITION;
	float4 Col : TEXCOORD0;
	noperspective float3 Heights : TEXCOORD1;
};


//--------------------------------------------------------------------------------------
// Utils functions
//--------------------------------------------------------------------------------------

// Compute the triangle face normal from 3 points
float3 faceNormal(in float3 posA, in float3 posB, in float3 posC)
{
	return normalize(cross(normalize(posB - posA), normalize(posC - posA)));
}

// Compute the final color of a face depending on its facing of the light
float4 shadeFace(in float4 verA, in float4 verB, in float4 verC)
{
	// Compute the triangle face normal in view frame
	float3 vera = verA.rgb;
		float3 verb = verB.rgb;
		float3 verc = verC.rgb;
		float3 normal = faceNormal(vera, verb, verc);

		// Then the color of the face.
		float3 lightvector = LightVector.rgb;
		float shade = 0.4 + 0.3*abs(dot(normal, lightvector));

	return float4(FillColor.xyz*shade, 1);
}

float evalMinDistanceToEdges(in PS_INPUT_WIRE input)
{
	float dist;
	uint3 order = uint3(0, 1, 2);

		float3 ddxHeights = ddx(input.Heights);
		float3 ddyHeights = ddy(input.Heights);
		float3 ddHeights2 = ddxHeights*ddxHeights + ddyHeights*ddyHeights;
		float3 invddHeights = 1.0 / sqrt(ddHeights2);
		float3 pixHeights2 = input.Heights *  input.Heights / ddHeights2;

		// We are dealing with a quad... don't display the "middle" edge
	if (Tri == 1) {
		float3 eDists = input.Heights * invddHeights;
		if (eDists[1] < eDists[0])
		{
			order.xy = order.yx;
		}
		if (eDists[2] < eDists[order.y])
		{
			order.yz = order.zy;
		}
		if (eDists[2] < eDists[order.x])
		{
			order.xy = order.yx;
		}

		if (order.x == 2) pixHeights2.z = 1000;
	}

	dist = sqrt(min(min(pixHeights2.x, pixHeights2.y), pixHeights2.z));

	return dist;
}

//--------------------------------------------------------------------------------------
// Vertex Shader
//--------------------------------------------------------------------------------------
GS_INPUT VS(VS_INPUT input)
{
	GS_INPUT output;

	output.Pos = mul(input.position, wvp);
	output.PosV = mul(input.position, viewMatrix);
	return output;
}

[maxvertexcount(3)]
void GS(triangle GS_INPUT input[3],
	inout TriangleStream<PS_INPUT> outStream)
{
	PS_INPUT output;

	// Shade and colour face.
	output.Col = shadeFace(input[0].PosV, input[1].PosV, input[2].PosV);

	output.Pos = input[0].Pos;
	outStream.Append(output);

	output.Pos = input[1].Pos;
	outStream.Append(output);

	output.Pos = input[2].Pos;
	outStream.Append(output);

	outStream.RestartStrip();
}

[maxvertexcount(3)]
void GSSolidWire(triangle GS_INPUT input[3],
	inout TriangleStream<PS_INPUT_WIRE> outStream)
{
	PS_INPUT_WIRE output;

	// Shade and colour face.
	output.Col = shadeFace(input[0].PosV, input[1].PosV, input[2].PosV);

	// Emit the 3 vertices
	// The Height attribute is based on the constant
	output.Pos = input[0].Pos;
	output.Heights = float3(1, 0, 0);
	outStream.Append(output);

	output.Pos = input[1].Pos;
	output.Heights = float3(0, 1, 0);
	outStream.Append(output);

	output.Pos = input[2].Pos;
	output.Heights = float3(0, 0, 1);
	outStream.Append(output);

	outStream.RestartStrip();
}

float4 PSColor(PS_INPUT input) : SV_Target
{
	return input.Col;
}

float4 PSSolidWireFade(PS_INPUT_WIRE input) : SV_Target
{
	// Compute the shortest square distance between the fragment and the edges.
	float dist = evalMinDistanceToEdges(input);

	// Cull fragments too far from the edge.
	if (dist > 0.5*LineWidth + 1) discard;

	// Map the computed distance to the [0,2] range on the border of the line.
	dist = clamp((dist - (0.5*LineWidth - 1)), 0, 2);

	// Alpha is computed from the function exp2(-2(x)^2).
	dist *= dist;
	float alpha = exp2(-2 * dist);

	// Standard wire color but faded by distance
	// Dividing by pos.w, the depth in view space
	float fading = clamp(FadeDistance / input.Pos.w, 0, 1);

	float4 color = WireColor * fading;
		color.a *= alpha;
	return color;
}


technique11 Shader_SolidWireFade
{
	pass P0
	{
		SetDepthStencilState(DSSDepthWriteLess, 0);
		SetRasterizerState(RSFill);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
		SetVertexShader(CompileShader(vs_4_0, VS()));
		SetGeometryShader(CompileShader(gs_4_0, GS()));
		SetPixelShader(CompileShader(ps_4_0, PSColor()));
	}

	pass P1
	{
		SetDepthStencilState(DSSDepthLessEqual, 0);
		SetRasterizerState(RSFill);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
		SetVertexShader(CompileShader(vs_4_0, VS()));
		SetGeometryShader(CompileShader(gs_4_0, GSSolidWire()));
		SetPixelShader(CompileShader(ps_4_0, PSSolidWireFade()));
	}
}