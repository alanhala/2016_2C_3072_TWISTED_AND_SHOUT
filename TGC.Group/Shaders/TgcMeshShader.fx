/*
* Shader generico para TgcMesh.
* Hay 3 Techniques, una para cada MeshRenderType:
*	- VERTEX_COLOR
*	- DIFFUSE_MAP
*	- DIFFUSE_MAP_AND_LIGHTMAP
*/

/**************************************************************************************/
/* Variables comunes */
/**************************************************************************************/

//Matrices de transformacion
float4x4 matWorld; //Matriz de transformacion World
float4x4 matWorldView; //Matriz World * View
float4x4 matWorldViewProj; //Matriz World * View * Projection
float4x4 matInverseTransposeWorld; //Matriz Transpose(Invert(World))

bool carDamaged;

//Textura para DiffuseMap
texture texDiffuseMap;
sampler2D diffuseMap = sampler_state
{
	Texture = (texDiffuseMap);
	ADDRESSU = WRAP;
	ADDRESSV = WRAP;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

//Textura para Lightmap
texture texLightMap;
sampler2D lightMap = sampler_state
{
	Texture = (texLightMap);
};

/**************************************************************************************/
/* VERTEX_COLOR */
/**************************************************************************************/

//Input del Vertex Shader
struct VS_INPUT_VERTEX_COLOR
{
	float4 Position : POSITION0;
	float3 Normal : NORMAL0;
	float4 Color : COLOR;
};

//Output del Vertex Shader
struct VS_OUTPUT_VERTEX_COLOR
{
	float4 Position : POSITION0;
	float4 Color : COLOR;
};

//Vertex Shader
VS_OUTPUT_VERTEX_COLOR vs_VertexColor(VS_INPUT_VERTEX_COLOR input)
{
	VS_OUTPUT_VERTEX_COLOR output;

	//Proyectar posicion
	output.Position = mul(input.Position, matWorldViewProj);

	//Enviar color directamente
	output.Color = input.Color;

	return output;
}

//Input del Pixel Shader
struct PS_INPUT_VERTEX_COLOR
{
	float4 Color : COLOR0;
};

//Pixel Shader
float4 ps_VertexColor(PS_INPUT_VERTEX_COLOR input) : COLOR0
{
	return input.Color;
}

/*
* Technique VERTEX_COLOR
*/
technique VERTEX_COLOR
{
	pass Pass_0
	{
		VertexShader = compile vs_3_0 vs_VertexColor();
		PixelShader = compile ps_3_0 ps_VertexColor();
	}
}

/**************************************************************************************/
/* DIFFUSE_MAP */
/**************************************************************************************/

//Input del Vertex Shader
struct VS_INPUT_DIFFUSE_MAP
{
	float4 Position : POSITION0;
	float3 Normal : NORMAL0;
	float4 Color : COLOR;
	float2 Texcoord : TEXCOORD0;
};

//Output del Vertex Shader
struct VS_OUTPUT_DIFFUSE_MAP
{
	float4 Position : POSITION0;
	float4 Color : COLOR;
	float2 Texcoord : TEXCOORD0;
};

//Vertex Shader
VS_OUTPUT_DIFFUSE_MAP vs_DiffuseMap(VS_INPUT_DIFFUSE_MAP input)
{
	VS_OUTPUT_DIFFUSE_MAP output;

	//Proyectar posicion
	output.Position = mul(input.Position, matWorldViewProj);

	//Enviar color directamente
	output.Color = input.Color;

	//Enviar Texcoord directamente
	output.Texcoord = input.Texcoord;

	return output;
}

//Input del Pixel Shader
struct PS_DIFFUSE_MAP
{
	float4 Color : COLOR;
	float2 Texcoord : TEXCOORD0;
	float4 Position : POSITION0;
};

//Pixel Shader
float4 ps_DiffuseMap(PS_DIFFUSE_MAP input) : COLOR0
{
	//Modular color de la textura por color del mesh
	return tex2D(diffuseMap, input.Texcoord) * input.Color;
}

/*
* Technique DIFFUSE_MAP
*/
technique DIFFUSE_MAP
{
	pass Pass_0
	{
		VertexShader = compile vs_3_0 vs_DiffuseMap();
		PixelShader = compile ps_3_0 ps_DiffuseMap();
	}
}

/**************************************************************************************/
/* DIFFUSE_MAP_AND_LIGHTMAP */
/**************************************************************************************/

//Input del Vertex Shader
struct VS_INPUT_DIFFUSE_MAP_AND_LIGHTMAP
{
	float4 Position : POSITION0;
	float3 Normal : NORMAL0;
	float4 Color : COLOR;
	float2 Texcoord : TEXCOORD0;
	float2 TexcoordLightmap : TEXCOORD1;
};

//Output del Vertex Shader
struct VS_OUTPUT_DIFFUSE_MAP_AND_LIGHTMAP
{
	float4 Position : POSITION0;
	float4 Color : COLOR;
	float2 Texcoord : TEXCOORD0;
	float2 TexcoordLightmap : TEXCOORD1;
};

//Vertex Shader
VS_OUTPUT_DIFFUSE_MAP_AND_LIGHTMAP vs_diffuseMapAndLightmap(VS_INPUT_DIFFUSE_MAP_AND_LIGHTMAP input)
{
	VS_OUTPUT_DIFFUSE_MAP_AND_LIGHTMAP output;

	//Proyectar posicion
	output.Position = mul(input.Position, matWorldViewProj);

	//Enviar color directamente
	output.Color = input.Color;

	//Enviar Texcoord directamente
	output.Texcoord = input.Texcoord;
	output.TexcoordLightmap = input.TexcoordLightmap;

	return output;
}

//Input del Pixel Shader
struct PS_INPUT_DIFFUSE_MAP_AND_LIGHTMAP
{
	float4 Color : COLOR;
	float2 Texcoord : TEXCOORD0;
	float2 TexcoordLightmap : TEXCOORD1;
};

//Pixel Shader
float4 ps_diffuseMapAndLightmap(PS_INPUT_DIFFUSE_MAP_AND_LIGHTMAP input) : COLOR0
{
	//Obtener color de diffuseMap y de Lightmap
	float4 albedo = tex2D(diffuseMap, input.Texcoord);
	float4 lightmapColor = tex2D(lightMap, input.TexcoordLightmap);

	//Modular ambos colores por color del mesh
	return albedo * lightmapColor * input.Color;
}

//technique DIFFUSE_MAP_AND_LIGHTMAP
technique DIFFUSE_MAP_AND_LIGHTMAP
{
	pass Pass_0
	{
		VertexShader = compile vs_3_0 vs_diffuseMapAndLightmap();
		PixelShader = compile ps_3_0 ps_diffuseMapAndLightmap();
	}
}



// ----------------------------------------------------------------------------------------- //
// -----------------------------------------------------------------------------------------//

float3 AmbientLightPosition = float3(1000, 10000, 0);
float3 AmbientLightColor = float3(1, 1, 1);
float AmbientLightIntensity = 500;
float AmbientLightAttenuation = 0.2f;

float4 CarLightPosition;
float CarLightAttenuation = 0.1f;
float CarLightIntensity = 150;
float3 SpotLightDir;
float SpotLightAngleCos;
float SpotLightExponent = 7;

float4 cameraPosition;

float3 materialEmissiveColor = float3(0, 0, 0);
float3 materialAmbientColor = float3(1, 1, 1);
float4 materialDiffuseColor = float4(1, 1, 1, 1);
float3 materialSpecularColor = float3(1, 1, 1);
float materialSpecularExp = 20;

float3 calculateAmbientLight(float intensity, float3 lightColor)
{
    return intensity * lightColor * materialAmbientColor;
}

float3 calculateDiffuseLight(float intensity, float3 lightColor, float nDotL)
{
    return intensity * lightColor * materialDiffuseColor.rgb * max(0.0, nDotL);
}

float3 calculateSpecularLight(float intensity, float3 ligthColor, float nDotL, float nDotH)
{
    return nDotL <= 0.0 ? float3(0.0, 0.0, 0.0) 
        : (intensity * ligthColor * materialSpecularColor * pow(max(0.0, nDotH), materialSpecularExp));
}

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Normal : NORMAL0;
    float4 Color : COLOR0;
    float4 Texcoord : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 Texcoord : TEXCOORD0;
    float3 WorldPosition : TEXCOORD1;
    float3 WorldNormal : TEXCOORD2;
    float3 LightVec : TEXCOORD3;
    float3 HalfwayVector : TEXCOORD4;
    float3 CarLigthVec : TEXCOORD5;
    float3 CarHalfwayVector : TEXCOORD6;
};

struct PixelShaderInput
{
    float2 Texcoord : TEXCOORD0;
    float3 WorldPosition : TEXCOORD1;
    float3 WorldNormal : TEXCOORD2;
    float3 LightVec : TEXCOORD3;
    float3 HalfwayVector : TEXCOORD4;
    float3 CarLigthVec : TEXCOORD5;
    float3 CarHalfwayVector : TEXCOORD6;
};

VertexShaderOutput vs_main(VertexShaderInput input)
{
    VertexShaderOutput output;

    output.Position = mul(input.Position, matWorldViewProj);
    output.Texcoord = input.Texcoord;
    output.WorldPosition = mul(input.Position, matWorld);
    output.WorldNormal = mul(input.Normal, matInverseTransposeWorld).xyz;
    output.LightVec = AmbientLightPosition - output.WorldPosition;
    output.CarLigthVec = CarLightPosition.xyz - output.WorldPosition;

    float3 viewVector = cameraPosition.xyz - output.WorldPosition;
    output.HalfwayVector = viewVector + output.LightVec;
    output.CarHalfwayVector = viewVector + output.CarLigthVec;

    return output;
}

float4 ps_ambient_light(PixelShaderInput input) : COLOR
{
    float3 nNormal = normalize(input.WorldNormal);
    float3 nLightVec = normalize(input.LightVec);
    float3 nCarLigthVec = normalize(input.CarLigthVec);
    float3 nHalfwayVector = normalize(input.HalfwayVector);
    float3 nCarHalfwayVector = normalize(input.CarHalfwayVector);

    float ambientDistAtten = length(AmbientLightPosition - input.WorldPosition) * AmbientLightAttenuation;
    float ambientIntensity = AmbientLightIntensity / ambientDistAtten;

    float spotAtten = dot(-SpotLightDir, nCarLigthVec);
    spotAtten = (spotAtten > SpotLightAngleCos) ? pow(spotAtten, SpotLightExponent) : 0.0;
    float carDistAtten = length(CarLightPosition.xyz - input.WorldPosition) * CarLightAttenuation;
    float carIntensity = CarLightIntensity * spotAtten / carDistAtten;

    float4 texelColor = tex2D(diffuseMap, input.Texcoord);

    float3 ambientLight = calculateAmbientLight(ambientIntensity, AmbientLightColor);
    float3 carLightAmbientLight = calculateAmbientLight(carIntensity, AmbientLightColor);

    float ambientNDotL = dot(nNormal, nLightVec);
    float carNDotL = dot(nNormal, nCarLigthVec);
    float3 ambientDiffuseLight = calculateDiffuseLight(ambientIntensity, AmbientLightColor, ambientNDotL);
    float3 carDiffuseLight = calculateDiffuseLight(carIntensity, AmbientLightColor, carNDotL);

    float ambientNDotH = dot(nNormal, nHalfwayVector);
    float carNDotH = dot(nNormal, nCarHalfwayVector);
    float3 ambientSpecularLight = calculateSpecularLight(ambientIntensity, AmbientLightColor, ambientNDotL, ambientNDotH);
    float3 carSpecularLight = calculateSpecularLight(carIntensity, AmbientLightColor, carNDotL, carNDotH);

    float3 finalAmbientLight = (ambientLight + carLightAmbientLight) / 2;
    float3 finalDiffuseLight = (ambientDiffuseLight + carDiffuseLight) / 2;
    float3 finalSpecularLight = (ambientSpecularLight + carSpecularLight) / 2;

    return float4(saturate(materialEmissiveColor + finalAmbientLight + finalDiffuseLight) * texelColor + finalSpecularLight, materialDiffuseColor.a);
}

technique Light
{
    pass Pass_0
    {
        VertexShader = compile vs_3_0 vs_main();
        PixelShader = compile ps_3_0 ps_ambient_light();
    }
}

// -----------------------------------------------

struct CarVertexShaderOutput
{
    float4 Position : POSITION0;
    float2 Texcoord : TEXCOORD0;
    float3 WorldPosition : TEXCOORD1;
    float3 WorldNormal : TEXCOORD2;
    float3 LightVec : TEXCOORD3;
    float3 HalfwayVector : TEXCOORD4;
};

struct CarPixelShaderInput
{
    float2 Texcoord : TEXCOORD0;
    float3 WorldPosition : TEXCOORD1;
    float3 WorldNormal : TEXCOORD2;
    float3 LightVec : TEXCOORD3;
    float3 HalfwayVector : TEXCOORD4;
};

CarVertexShaderOutput vs_car(VertexShaderInput input)
{
    CarVertexShaderOutput output;

    if (carDamaged && input.Position.y > 20)
    {
        input.Position.y = input.Position.y * 0.9;
        input.Position.x = input.Position.x * 0.8;
        input.Position.z = input.Position.z * 0.8;
    }

    if (carDamaged && input.Position.y < 20 && input.Position.z < 10 && input.Position.z > -10)
    {
        input.Position.x = input.Position.x * 0.8;
    }

    output.Position = mul(input.Position, matWorldViewProj);
    output.Texcoord = input.Texcoord;
    output.WorldPosition = mul(input.Position, matWorld);
    output.WorldNormal = mul(input.Normal, matInverseTransposeWorld).xyz;
    output.LightVec = AmbientLightPosition.xyz - output.WorldPosition;

    float3 viewVector = cameraPosition.xyz - output.WorldPosition;
    output.HalfwayVector = viewVector + output.LightVec;

    return output;
}

float4 ps_car_light(CarPixelShaderInput input) : COLOR
{
    float3 Nn = normalize(input.WorldNormal);
    float3 Ln = normalize(input.LightVec);
    float3 Hn = normalize(input.HalfwayVector);

    float distAtten = length(AmbientLightPosition - input.WorldPosition) * AmbientLightAttenuation;
    float intensity = AmbientLightIntensity / distAtten;

    float4 texelColor = tex2D(diffuseMap, input.Texcoord);
    
    float3 ambientLight = calculateAmbientLight(intensity, AmbientLightColor);

    float n_dot_l = dot(Nn, Ln);
    float3 diffuseLight = calculateDiffuseLight(intensity, AmbientLightColor, n_dot_l);

    float n_dot_h = dot(Nn, Hn);
    float3 specularLight = calculateSpecularLight(intensity, AmbientLightColor, n_dot_l, n_dot_h);

    return float4(saturate(materialEmissiveColor + ambientLight + diffuseLight) * texelColor + specularLight, materialDiffuseColor.a);
}


technique ColissionAndLight
{
	pass Pass_1
	{
		VertexShader = compile vs_3_0 vs_car();
		PixelShader = compile ps_3_0 ps_car_light();
	}
};
