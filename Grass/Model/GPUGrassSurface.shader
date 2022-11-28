Shader "GPU/GPUGrassSurface" {

	Properties {
		_Smoothness ("Smoothness", Range(0,1)) = 0.5
	}
	
	SubShader {
		CGPROGRAM
		#pragma surface ConfigureSurface Standard fullforwardshadows addshadow
		#pragma instancing_options assumeuniformscaling procedural:ConfigureProcedural
		#pragma target 4.5

		void ConfigureProcedural () {

        }

		#include "PointGPU.hlsl"

		struct Input {
			float3 worldPos;
		};

		float _Smoothness;



		void ConfigureSurface (Input input, inout SurfaceOutputStandard surface) {
			surface.Albedo = saturate(input.worldPos * 0.5 + 0.5);
			surface.Smoothness = _Smoothness;
		}
		ENDCG
	}


	FallBack "Diffuse"
}