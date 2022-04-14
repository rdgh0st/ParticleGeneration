float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 InverseCamera; //Inverse Camera Matrix 
texture2D Texture;
float4 AmbientColor;
float AmbientIntensity;
float4 DiffuseColor;
float DiffuseIntensity;
float3 CameraPosition;
float3 LightPosition;
float Shininess;
float4 SpecularColor;
float SpecularIntensity = 1;

sampler ParticleSampler: register(s0) = sampler_state {
	texture = <Texture>;
	magfilter = LINEAR; // None, POINT, LINEAR, Anisotropic 
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = Wrap; // Clamp, Mirror, MirrorOnce, Wrap, Border 
	AddressV = Wrap;
};

struct VertexShaderInput
{
	float4 Position: POSITION;
	float2 TexCoord: TEXCOORD0;
	float4 ParticlePosition: POSITION1;
	float4 ParticleParamater: POSITION2; // x: Scale x/y: Color 
};

struct VertexShaderOutput
{
	float4 Position: POSITION;
	float4 ParticlePosition: POSITION1;
	float2 TexCoord: TEXCOORD0;
	float4 Color: COLOR0;
};

VertexShaderOutput ParticleVertexShader(VertexShaderInput input)
{
	VertexShaderOutput output;
	float4 worldPosition = mul(input.Position, InverseCamera);
	worldPosition.xyz = worldPosition.xyz * sqrt(input.ParticleParamater.x);
	worldPosition += input.ParticlePosition;

	output.Position = mul(mul(mul(worldPosition, World), View), Projection);
	output.TexCoord = input.TexCoord;
	output.Color = 1 - input.ParticleParamater.x / input.ParticleParamater.y;
	return output;
}

float4 ParticlePixelShader(VertexShaderOutput input) : COLOR
{
	float4 color = tex2D(ParticleSampler, input.TexCoord);
	color *= input.Color;
	return color;
}

float4 PhongPixelShader(VertexShaderOutput input) : COLOR
{
	float4 color = float4(0, 1, 0, 1);
	color *= input.Color;

	return color;
}

technique Phong {
	pass Pass1 {
		VertexShader = compile vs_4_0 ParticleVertexShader();
		PixelShader = compile ps_4_0 PhongPixelShader();
	}
}

technique ParticleTechnique {
	pass Pass1 {
		VertexShader = compile vs_4_0 ParticleVertexShader();
		PixelShader = compile ps_4_0 ParticlePixelShader();
	}
}