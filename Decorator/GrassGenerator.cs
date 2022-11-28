using System.Collections.Generic;
using UnityEngine;

namespace Generador.LandGenerator
{

    public class GrassGenerator : MonoBehaviour, Decorator
    {
        private const int submeshIndex = 0;
        public bool reciveShadows = false;
        public UnityEngine.Rendering.ShadowCastingMode shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        public Mesh GrassMeshLOD0;
        public Material GrassMaterialLOD0;
        public Mesh GrassMeshLOD1;
        public Material GrassMaterialLOD1;
        public float InstancesPerMeter = 1;
        public float maxHeightToInstantiate = 5f;
        public float maxGrassHeight = .2f;
        public float grassScale = 1f;
        private Mesh landMesh;
        private ComputeBuffer perInstanceBuffer;
        [SerializeField]
        private ComputeShader compute;
        private List<InstanceData> perInstanceData;
        private Bounds bounds;

        static readonly int perInstanceDataID = Shader.PropertyToID("_PerInstanceData");

        private struct InstanceData
        {
            public Matrix4x4 Matrix;
            public Matrix4x4 MatrixInverse;

            public static int Size()
            {
                return sizeof(float) * 4 * 4
                    + sizeof(float) * 4 * 4;
            }
        }

        private void updateFunctionOnGPU()
        {
            compute.SetBuffer(0, perInstanceDataID, perInstanceBuffer);
            compute.Dispatch(0, perInstanceData.Count, 1, 1);
            GrassMaterialLOD0.SetBuffer(perInstanceDataID, perInstanceBuffer);
        }

        [ContextMenu("Generate")]
        public void Decorate(Mesh mesh)
        {

            bounds = new Bounds(transform.position, Vector3.one * 100 + Vector3.up * 500);
            landMesh = mesh;
            perInstanceData = new List<InstanceData>();

            Vector3[] vertices = landMesh.vertices;

            float vertexDistance = Vector3.Distance(vertices[0], vertices[1]);
            float usableDistance = vertexDistance / 2;
            int instancesPerLine = (int)(InstancesPerMeter * usableDistance);
            float instanceGap = usableDistance / InstancesPerMeter;


            foreach (Vector3 vertex in vertices)
            {
                if (vertex.y < maxHeightToInstantiate)
                {
                    addGrassMatrix(vertex);

                    Vector3 initialPosition = vertex - new Vector3(usableDistance, 0, usableDistance);
                    for (int x = 0; x < instancesPerLine; x++)
                        for (int z = 0; z < instancesPerLine; z++)
                            addGrassMatrix(initialPosition + new Vector3(x * instanceGap, 0, z * instanceGap));
                }
            }

            if (perInstanceBuffer != null)
                perInstanceBuffer.Dispose();

            perInstanceBuffer = new ComputeBuffer(perInstanceData.Count, InstanceData.Size());
            perInstanceBuffer.SetData(perInstanceData);
            updateFunctionOnGPU();
        }

        private void addGrassMatrix(Vector3 point)
        {
            float randX = Random.Range(-0.9f, .9f);
            float randZ = Random.Range(-0.9f, .9f);
            float randH = Mathf.PerlinNoise(point.x, point.y) * maxGrassHeight;
            float randR = Random.Range(0, 180f);

            Vector3 finalPosition = transform.position + new Vector3(point.x + randX, point.y + .5f, point.z + randZ);
            Quaternion rotation = Quaternion.identity * Quaternion.Euler(0, randR, 0);
            Vector3 scale = new Vector3(grassScale, 0.1f + randH, grassScale);

            InstanceData data = new InstanceData();
            data.Matrix = Matrix4x4.TRS(finalPosition, rotation, scale);

            perInstanceData.Add(data);
        }

        void Update()
        {
            renderBatches(GrassMeshLOD0, GrassMaterialLOD0);
        }

        private void renderBatches(Mesh GrassMesh, Material GrassMaterial)
        {
            Graphics.DrawMeshInstancedProcedural(GrassMeshLOD0, submeshIndex, GrassMaterialLOD0, bounds, perInstanceData.Count, new MaterialPropertyBlock());
        }
    }
}