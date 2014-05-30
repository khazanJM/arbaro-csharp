cbuffer MatrixBuffer
{
	matrix worldMatrix;
	matrix viewMatrix;
	matrix projectionMatrix;
	matrix wvp;
	
	matrix rotation;
};
  
struct VertexInputType
{
	float3 position : POSITION;
};

struct PixelInputType
{
	float4 position : SV_POSITION;
};

PixelInputType VS(VertexInputType input)
{
	PixelInputType output;

	output.position = mul(float4(input.position, 1), rotation);
	output.position = mul(output.position, wvp);
	return output;
}    

float4 PS(PixelInputType input) : SV_TARGET
{
	return float4(1,1,1, 1);
}

RasterizerState rsStandard { FillMode = Wireframe; CullMode = Back; AntialiasedLineEnable = true; MultisampleEnable = true; };


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

