using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Generador;

namespace Generador.LandGenerator
{
    public class LandGenerator : MonoBehaviour, Generator
    {
        public Material baseMaterial;
        public bool autoUpdate = true;
        [Range(2, 100)]
        [Tooltip("How many vertex along width. VertexResolution*2=TotalVertex")]
        public int VerticesPerLine = 2;
        public int ChunkSize = 241;
        public float zoom = 1;
        public float generalHeightMultiplier;
        public int seed = 1;
        public bool offsetIsPosition = true;
        public Vector2 offset;
        private Land land;
        [SerializeField]
        private ComputeShader mapGeneratorShader;
        private vertex[] chunk;
        private Decorator[] decorators;

        public void Generar()
        {
            decorators = GetComponents<Decorator>();
            if (land.gameObject == null)
                createLand();

            noiseMapSetUp();
            GenerateMapMeshAndTexture();
        }

        public void SetCalidadMesh(int calidad)
        {
            this.VerticesPerLine = calidad;
        }

        private void createLand()
        {
            land.gameObject = new GameObject("Land");
            land.meshCollider = land.gameObject.AddComponent<MeshCollider>();
            land.meshFilter = land.gameObject.AddComponent<MeshFilter>();
            land.meshRenderer = land.gameObject.AddComponent<MeshRenderer>();
            land.textureRenderer = land.gameObject.AddComponent<Renderer>();

            land.gameObject.transform.position = this.transform.position;
            land.gameObject.transform.SetParent(this.transform);
        }

        private void GenerateMapMeshAndTexture()
        {
            Mesh meshTerreno = generateTerrainMesh().CreateMesh();
            Material mat = new Material(baseMaterial);

            land.meshFilter.sharedMesh = meshTerreno;
            land.meshCollider.sharedMesh = meshTerreno;
            land.meshRenderer.sharedMaterial = mat;
        }


        private void noiseMapSetUp()
        {
            NoiseMapConfig noiseMapConfig = new NoiseMapConfig();
            noiseMapConfig.alto = VerticesPerLine;
            noiseMapConfig.ancho = VerticesPerLine;
            noiseMapConfig.seed = seed;
            noiseMapConfig.offset = offsetIsPosition ? new Vector2(transform.position.x, transform.position.z) : offset;
        }

        private MeshBuilder generateTerrainMesh()
        {
            MeshBuilder meshData = new MeshBuilder();

            addVertices(meshData);
            createFaces(meshData);

            return meshData;
        }

        private void addVertices(MeshBuilder meshData)
        {
            Vector2 topLeftPosition = new Vector2(-ChunkSize / 2, -ChunkSize / 2);
            vertex[] Vertices = calcularChunk();
            for (int i = 0; i < Vertices.Length; i++)
            {
                vertex vertex = Vertices[i];
                meshData.UVs.Add((new Vector2(vertex.x, vertex.z) + topLeftPosition) / ChunkSize);
                Vector3 newVertex = new Vector3(vertex.x, vertex.y, vertex.z);
                meshData.AddVertice(newVertex);
            }
        }

        private void createFaces(MeshBuilder meshData)
        {
            int squaresPerLine = (VerticesPerLine - 1);
            int squaresAmount = squaresPerLine * squaresPerLine;
            int rowsDone = 0;
            for (int i = 0; i < squaresAmount; i++)
            {
                if ((i + rowsDone + 1) % VerticesPerLine == 0)
                    //Si es múltiplo del ancho de vertices del terreno, entonces terminamos una fila.
                    rowsDone++;

                /*
                 *  a - b
                 *    \
                 *  c - d
                 */
                int a = i + rowsDone;
                int b = a + 1;
                int c = a + VerticesPerLine;
                int d = c + 1;
                meshData.addTriangle(a, d, b);
                meshData.addTriangle(a, c, d);
            }
        }

        private vertex[] calcularChunk()
        {
            Vector3 initialPosition =  - (new Vector3(ChunkSize / 2, 0, ChunkSize / 2));
            float vertexDistance = ChunkSize / (float)(VerticesPerLine - 1);
            float[] offset = { this.offset.x, this.offset.y };
            if(offsetIsPosition) {
                offset[0] = transform.position.x;
                offset[1] = transform.position.z;
            }
            ComputeBuffer o_VertexBuffer = new ComputeBuffer(VerticesPerLine * VerticesPerLine, sizeof(float) * 3, ComputeBufferType.Default);

            mapGeneratorShader.SetInt("vertexPerLine", VerticesPerLine);
            mapGeneratorShader.SetFloat("vertexDistance", vertexDistance);
            mapGeneratorShader.SetFloat("generalMultiplier", generalHeightMultiplier);
            mapGeneratorShader.SetFloat("zoom", 1 / zoom);
            mapGeneratorShader.SetFloats("initialPosition", new float[] { initialPosition.x, initialPosition.y, initialPosition.z });
            mapGeneratorShader.SetFloats("offset", offset);

            int kernel = mapGeneratorShader.FindKernel("generateTerrain");
            mapGeneratorShader.SetBuffer(kernel, "OutVertex", o_VertexBuffer);

            uint xGroupSize = 64;

            mapGeneratorShader.GetKernelThreadGroupSizes(kernel, out xGroupSize, out _, out _);
            int xGroups = Mathf.CeilToInt(VerticesPerLine * VerticesPerLine / (float)xGroupSize);
            if (xGroups == 0)
                xGroups = 1;
            mapGeneratorShader.Dispatch(kernel, xGroups, 1, 1);
            vertex[] Vertices = new vertex[o_VertexBuffer.count];
            o_VertexBuffer.GetData(Vertices, 0, 0, o_VertexBuffer.count);

            if (decorators.Length > 0)
                passDataToDecorators(o_VertexBuffer);
            else
                o_VertexBuffer.Dispose();

            this.chunk = Vertices;

            return Vertices;
        }

        private void passDataToDecorators(ComputeBuffer vertexBuffer)
        {
            foreach (Decorator decorator in decorators)
            {
                decorator.Decorate(vertexBuffer);
            }
        }
    }

    // private void OnDestroy() {

    // }
}
