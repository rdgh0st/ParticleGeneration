float4x4 World;
float4x4 View;
float4x4 Projection;
float3 CameraPosition;
Texture SkyBoxTexture;

samplerCUBE SkyBoxSampler = sampler_state {
	texture = <SkyBoxTexture>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = Mirror;
	AddressV = Mirror;
};

struct VertexShaderInput {
	float4 Position: POSITION0;
};

struct VertexShaderOutput {
	float4 Position: POSITION;
	float3 TextureCoordinate: TEXCOORD;
};

VertexShaderOutput VertexFunction(VertexShaderInput input) {
	VertexShaderOutput output;
	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	output.TextureCoordinate = worldPosition.xyz - CameraPosition;
	return output;
}

float4 PixelFunction(VertexShaderOutput input) : COLOR {
	return texCUBE(SkyBoxSampler, normalize(input.TextureCoordinate));
}

technique Skybox {
	pass Pass1 {
		VertexShader = compile vs_4_0 VertexFunction();
		PixelShader = compile ps_4_0 PixelFunction();
	}
}