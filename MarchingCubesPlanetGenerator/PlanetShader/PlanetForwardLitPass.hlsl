#include <HLSLSupport.cginc>

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
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

float4 _GroundColor;
float4 _CliffsColor;
float4 _WaterColor;
float _Glossiness;
float _WaterLevel;
float _PolesRadious;
float _Coldness;

Interpolators Vertex(Attributes input)
{
    Interpolators output;
    float3 p = input.positionOS;
    float3 pNormal = input.normalOS;

    if (length(input.positionOS) <= _WaterLevel)
    {
        float3 upDirection = normalize(p);
        pNormal = upDirection;
        p = pNormal * _WaterLevel;
    }

    VertexPositionInputs posInputs = GetVertexPositionInputs(p);
    output.positionCS = posInputs.positionCS;
    output.positionOS = input.positionOS;
    output.positionWS = posInputs.positionWS;
    output.normalOS = pNormal;

    return output;
}

float4 addSelfShadow(float3 positionOS, float3 positionWS, float4 currentColor)
{
    float4 color = currentColor;
    float3 lightDirection = GetMainLight().direction;
    float3 planetOrigin = mul(unity_ObjectToWorld, float4(0.0, 0.0, 0.0, 1.0));
    float3 wsFromCenter = normalize(positionWS - planetOrigin);

    float selfShadow = clamp(dot(lightDirection, wsFromCenter), 0, 1);
    color.xyz *= selfShadow;
    return color;
}

float2 ProjectToSphere(float3 p)
{
    float2 proj;
    float3 normalizedP = normalize(p);

    proj.x = (dot(normalizedP, float3(0, 0, 1)) + 1) / 2;
    proj.y = (dot(normalizedP, float3(0, 1, 0)) + 1) / 2;

    return proj;
}

float calculatePoles(float3 position)
{
    float color = 0;
    if (abs(position.y) < _PolesRadious)
        color = (position.y * _Coldness) * (position.y * _Coldness);
    else
        color = 1;
    return color;
}

float4 Fragment(Interpolators input) : SV_Target
{
    float4 color;
    InputData lightingInput = (InputData)0;
    SurfaceData surfaceInput = (SurfaceData)0;

    float3 upVector = normalize(input.positionOS);
    float upFacing = abs(dot(input.normalOS, upVector));
    color = lerp(_CliffsColor, _GroundColor, upFacing);

    float frozenFactor = calculatePoles(input.positionOS);
    color = lerp(color, float4(1, 1, 1, 1), frozenFactor);

    lightingInput.positionWS = input.positionWS;
    lightingInput.positionCS = input.positionCS;
    lightingInput.normalWS = normalize(mul(unity_ObjectToWorld, input.normalOS));
    lightingInput.viewDirectionWS = GetWorldSpaceNormalizeViewDir(input.positionWS);
    lightingInput.shadowCoord = TransformWorldToShadowCoord(input.positionWS);
    surfaceInput.smoothness = _Glossiness;
    surfaceInput.specular = .25;
    surfaceInput.albedo = color.xyz;
    surfaceInput.alpha = color.w;
    float3 reflectionVector = -lightingInput.viewDirectionWS + 2 *
        dot(
            lightingInput.viewDirectionWS, lightingInput.normalWS
        ) * lightingInput.normalWS;

    if (length(input.positionOS) <= _WaterLevel - .1)
    {
        float3 p = input.positionOS;
        const float distortionAmplitude = .006;
        const float distortionSize = 45;
        float distortion = snoise(ProjectToSphere(p) * distortionSize + _Time.x);
        lightingInput.normalWS = lightingInput.normalWS + float3(1, 1, 1) * distortion * distortionAmplitude;
        float fresnel = 1 - abs(dot(lightingInput.normalWS, lightingInput.viewDirectionWS));
        half4 skyData = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, reflectionVector);

        surfaceInput.smoothness = .8;

        surfaceInput.albedo = lerp(_WaterColor, skyData, fresnel * fresnel);
    }


    color = UniversalFragmentPBR(lightingInput, surfaceInput);
    color = addSelfShadow(input.positionOS, input.positionWS, color);

    return color;
}
