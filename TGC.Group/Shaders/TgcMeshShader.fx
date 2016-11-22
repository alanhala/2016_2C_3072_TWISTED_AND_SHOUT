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

float3 LightPosition = float3(1000, 10000, 0);
float3 LightColor = float3(1, 1, 1);
float LightIntensity = 500;
float LightAttenuation = 0.2f;

float4 cameraPosition;

float4 carLightPosition;
float3 spotLightDir;
float spotLightAngleCos;

float3 materialEmissiveColor = float3(0, 0, 0);
float3 materialAmbientColor = float3(1, 1, 1);
float4 materialDiffuseColor = float4(1, 1, 1, 1);
float3 materialSpecularColor = float3(1, 1, 1);
float materialSpecularExp = 20;

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
};

struct PixelShaderInput
{
    float2 Texcoord : TEXCOORD0;
    float3 WorldPosition : TEXCOORD1;
    float3 WorldNormal : TEXCOORD2;
    float3 LightVec : TEXCOORD3;
    float3 HalfwayVector : TEXCOORD4;
};

VertexShaderOutput vs_main(VertexShaderInput input)
{
    VertexShaderOutput output;

    output.Position = mul(input.Position, matWorldViewProj);
    output.Texcoord = input.Texcoord;
    output.WorldPosition = mul(input.Position, matWorld);
    output.WorldNormal = mul(input.Normal, matInverseTransposeWorld).xyz;
    output.LightVec = LightPosition.xyz - output.WorldPosition;

    float3 viewVector = cameraPosition.xyz - output.WorldPosition;
    output.HalfwayVector = viewVector + output.LightVec;

    return output;
}

float4 ps_ambient_light(PixelShaderInput input) : COLOR
{
    float3 Nn = normalize(input.WorldNormal);
    float3 Ln = normalize(input.LightVec);
    float3 Hn = normalize(input.HalfwayVector);

    float distAtten = length(LightPosition - input.WorldPosition) * LightAttenuation;
    float intensity = LightIntensity / distAtten;

    float4 texelColor = tex2D(diffuseMap, input.Texcoord);

    float3 ambientLight = intensity * LightColor * materialAmbientColor;

    float3 n_dot_l = dot(Nn, Ln);
    float3 diffuseLight = intensity * LightColor * materialDiffuseColor.rgb * max(0.0, n_dot_l);

    float3 n_dot_h = dot(Nn, Hn);
    float3 specularLight = n_dot_l <= 0.0
			? float3(0.0, 0.0, 0.0)
			: (intensity * LightColor * materialSpecularColor * pow(max(0.0, n_dot_h), materialSpecularExp));

    return float4(saturate(materialEmissiveColor + ambientLight + diffuseLight) * texelColor + specularLight, materialDiffuseColor.a);
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
    output.LightVec = LightPosition.xyz - output.WorldPosition;

    float3 viewVector = cameraPosition.xyz - output.WorldPosition;
    output.HalfwayVector = viewVector + output.LightVec;

    return output;
}

float4 ps_car_light(CarPixelShaderInput input) : COLOR
{
    float3 Nn = normalize(input.WorldNormal);
    float3 Ln = normalize(input.LightVec);
    float3 Hn = normalize(input.HalfwayVector);

    float distAtten = length(LightPosition - input.WorldPosition) * LightAttenuation;
    float intensity = LightIntensity / distAtten;

    float4 texelColor = tex2D(diffuseMap, input.Texcoord);

    float3 ambientLight = intensity * LightColor * materialAmbientColor;

    float3 n_dot_l = dot(Nn, Ln);
    float3 diffuseLight = intensity * LightColor * materialDiffuseColor.rgb * max(0.0, n_dot_l);

    float3 n_dot_h = dot(Nn, Hn);
    float3 specularLight = n_dot_l <= 0.0
			? float3(0.0, 0.0, 0.0)
			: (intensity * LightColor * materialSpecularColor * pow(max(0.0, n_dot_h), materialSpecularExp));

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
