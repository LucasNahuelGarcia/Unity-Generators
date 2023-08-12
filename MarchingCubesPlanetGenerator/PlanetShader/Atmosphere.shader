Shader "Custom/Planet"
{
    Properties
    {
        _Density ("Density", float) = 1
        _BaseHeight ("TerrainHeight", float) = 1
    }
    SubShader
    {
            Tags
            {
                "LightMode" = "UniversalForward"
                "RenderType" = "Transparent"
                "Queue" = "Transparent"
            }
        Pass
        {

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            #pragma target 3.0

            #include "AtmosphereForwardLitPass.hlsl"
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}