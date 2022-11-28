#ifndef GRASS_SHADER
#define GRASS_SHADER



#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)

	struct InstanceData {
		float4x4 worldMatrix;
		float4x4 worldMatrixInverse;
	};
	// StructuredBuffer<float3> _Positions;
	StructuredBuffer<InstanceData> _PerInstanceData;

#endif


void ConfigureProcedural()
{
    #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
		// float3 position = _Positions[unity_InstanceID];
		unity_ObjectToWorld = mul(unity_ObjectToWorld, _PerInstanceData[unity_InstanceID].worldMatrix);
		unity_WorldToObject = mul(unity_WorldToObject, _PerInstanceData[unity_InstanceID].worldMatrixInverse);
		// unity_ObjectToWorld = 0.0;
		// unity_ObjectToWorld._m03_m13_m23_m33 = float4(position, 1.0);
		// unity_ObjectToWorld._m00_m11_m22 = .5;//grass quad scale
	#endif
}


void Setup_float (float3 In, out float3 Out) {
	Out = In;
}

void Setup_half (half3 In, out half3 Out) {
	Out = In;
}

#endif