Shader "Custom/Planet"
{
    Properties
    {
        _GroundColor ("Ground Color", Color) = (1,1,1,1)
        _CliffsColor ("Cliffs Color", Color) = (1,1,1,1)
        _WaterColor ("Water Color", Color) = (1,1,1,1)
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _WaterLevel ("Water Level", float) = 0
        _PolesRadious ("Poles Radious", float) = 0
        _Coldness ("Coldness", float) = 0
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "Queue" = "Geometry"
        }
        Pass
        {
            Name "ForwardLit"
            Tags
            {
                "LightMode" = "UniversalForward"
            }

            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            #pragma target 3.0
            #pragma Lambert addshadow

            #include "PlanetForwardLitPass.hlsl"
            ENDHLSL
        }
        Pass
        {
            Tags
            {
                "LightMode"="DepthOnly"
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #include "UnityCG.cginc"

            struct v2f
            {
                V2F_SHADOW_CASTER;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDHLSL
        }
    }
}