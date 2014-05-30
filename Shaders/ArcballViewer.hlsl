cbuffer MatrixBuffer
{
	matrix worldMatrix;
	matrix viewMatrix;
	matrix projectionMatrix;
	matrix wvp;

	matrix rotation;
};   
 
DepthStencilState DSSDepthWriteLess
{
	DepthEnable = true;
	DepthWriteMask = All;
	DepthFunc = Less;
};  

struct VertexInputType
{
	float4 position : POSITION;
	float4 color: COLOR;

};
           
struct PixelInputType
{
	float4 position : SV_POSITION;
	float4 color: COLOR;
}; 

PixelInputType VS(VertexInputType input)
{
	PixelInputType output;

	output.position = mul(input.position, rotation);
	output.position = mul(output.position, wvp);

	float4 threshold = mul(float4(0, 0, 0, 1), wvp);
	output.color = input.color;

	if (output.position.z < threshold.z) output.color.xyz *= 0.5;

	return output;
} 
  
float4 PS(PixelInputType input) : SV_TARGET
{
	return input.color;
} 
                  
RasterizerState rsStandard { FillMode = Wireframe; CullMode = None; AntialiasedLineEnable = true; MultisampleEnable = true; };

technique10 ShaderStd
{
	pass P0
	{
		SetRasterizerState(rsStandard);
		SetDepthStencilState(DSSDepthWriteLess, 0);
		SetVertexShader(CompileShader(vs_5_0, VS()));
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_5_0, PS()));
	}
}

