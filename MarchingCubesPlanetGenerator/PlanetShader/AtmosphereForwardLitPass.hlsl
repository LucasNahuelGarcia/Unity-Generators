#include <HLSLSupport.cginc>

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
#include "Noise.hlsl"

struct Attributes
{
    float3 positionOS: POSITION;
    float3 normalOS : NORMAL;
};

struct Interpolators
{
    float4 positionCS : SV_POSITION;
    float3 positionOS : TEXCOORD1;
    float3 positionWS : TEXCOORD2;
    float3 normalOS : TEXCOORD3;
};

float _BaseHeight;
float _Density;

Interpolators Vertex(Attributes input)
{
    Interpolators output;
    float3 p = input.positionOS;
    float3 pNormal = input.normalOS;

    VertexPositionInputs posInputs = GetVertexPositionInputs(p);
    output.positionCS = posInputs.positionCS;
    output.positionOS = input.positionOS;
    output.positionWS = posInputs.positionWS;
    output.normalOS = pNormal;

    return output;
}

float getSelfShadow(float3 lightDirectionWS, float3 normalWS)
{
    return (1 + dot(lightDirectionWS, normalWS)) / 2;
}

float getDepth(Interpolators input)
{
    return clamp(
        (SampleSceneDepth((input.positionCS.xy / _ScaledScreenParams.xy)) / (input.positionCS.z)) * _BaseHeight, 0, 1);
}

float4 Fragment(Interpolators input) : SV_Target
{
    float4 color = float4(GetMainLight().color, 1);
    float3 normalWS = normalize(TransformObjectToWorld(input.normalOS));
    float3 viewDirectionWS = normalize(GetWorldSpaceViewDir(input.positionWS));
    float3 lightDirectionWS = normalize(GetMainLight().direction);
    float rawDepth = getDepth(input);

    float sunsetFactor = 1 - (1 + dot(lightDirectionWS, viewDirectionWS)) / 2;
    sunsetFactor *= 1 - abs(dot(viewDirectionWS, normalWS));
    float density = clamp(dot(viewDirectionWS, normalWS), 0, 1);
    density *= _Density;

    color.rgb = lerp(float3(0, .2, 1), float3(1, .2, 0), sunsetFactor);

    color.a = 1 - rawDepth;
    color.a *= density * density * density;
    color.a *= getSelfShadow(lightDirectionWS, normalWS);

    return color;
}
