cbuffer MatrixBuffer
{
	matrix worldMatrix;
	matrix viewMatrix;
	matrix projectionMatrix;
	matrix wvp;
};

struct VertexInputType
{
	float3 position : POSITION;
	float3 color : COLOR;
};

struct PixelInputType
{
	float4 position : SV_POSITION;
	float3 color : COLOR;
};

PixelInputType VS(VertexInputType input)
{
	PixelInputType output;

	output.position = mul(float4(input.position, 1), wvp);
	output.color = input.color;
	return output;
}

float4 PS(PixelInputType input) : SV_TARGET
{
	return float4(input.color, 1);
}

RasterizerState rsStandard { FillMode = Solid; CullMode = Back; AntialiasedLineEnable = true; MultisampleEnable = true; };


technique10 ShaderStd
{
	pass P0
	{
		SetRasterizerState(rsStandard);
		SetVertexShader(CompileShader(vs_5_0, VS()));
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_5_0, PS()));
	}
}

