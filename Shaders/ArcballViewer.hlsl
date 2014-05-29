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

	output.position = mul(input.position, wvp);
	output.position = input.position;
	output.color = input.color;

	//output.color = float4(1,1,1,1);
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

