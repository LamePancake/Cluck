//reuse//code//Vertex//
uniform extern float4x4 ViewMatrix;
uniform extern float4x4 ProjectionMatrix;

void SkyboxVertexShader( float3 pos : POSITION0,
                         out float4 SkyPos : POSITION0,
                         out float3 SkyCoord : TEXCOORD0 )
{
    // Calculate rotation. Using a float3 result, so translation is ignored
    float3 rotatedPosition = mul(pos, ViewMatrix);           
    // Calculate projection, moving all vertices to the far clip plane 
    // (w and z both 1.0)
    SkyPos = mul(float4(rotatedPosition, 1), ProjectionMatrix).xyww;    

    SkyCoord = pos;
};
//reuse//code//Vertex//
//reuse//code//Pixel//
uniform extern texture SkyboxTexture;
uniform extern texture SkyboxTextureRed;

float IntensityBlue;

sampler SkyboxS = sampler_state
{
    Texture = <SkyboxTexture>;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    MipFilter = LINEAR;
    AddressU = CLAMP;
    AddressV = CLAMP;
};

sampler SkyboxSR = sampler_state
{
    Texture = <SkyboxTextureRed>;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    MipFilter = LINEAR;
    AddressU = CLAMP;
    AddressV = CLAMP;
};

float4 SkyboxPixelShader( float3 SkyCoord : TEXCOORD0 ) : COLOR
{
	float intensityRed = 1 - IntensityBlue;

    // grab the pixel color value from the skybox cube map
    return (texCUBE(SkyboxS, SkyCoord) * IntensityBlue) + (texCUBE(SkyboxSR, SkyCoord) * intensityRed);
};
//reuse//code//Pixel//
//reuse//code//Technique//
technique SkyboxTechnique
{
    pass P0
    {
        vertexShader = compile vs_2_0 SkyboxVertexShader();
        pixelShader = compile ps_2_0 SkyboxPixelShader();

        // We're drawing the inside of a model
        CullMode = None;  
        // We don't want it to obscure objects with a Z < 1
        ZWriteEnable = false; 
    }
}
//reuse//code//Technique//