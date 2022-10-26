using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generador.LandGenerator
{
    public class MapGenerator : MonoBehaviour, Generator
    {
        public Material baseMaterial;
        public noiseMap noiseMapGenerator;
        public bool autoUpdate = true;
        [Range(2, 70)] //La cantidad de  vertices que hay en el ancho del mapa
        [Tooltip("How many vertex along width. VertexResolution*2=TotalVertex")]
        public int VerticesPerLine = 2;
        public int ChunkSize = 241;
        public float generalHeightMultiplier;
        public AnimationCurve curveHeightMultiplier;
        public int seed = 1;
        public PerlinOctave[] Octaves;
        public bool offsetIsPosition = true;
        public Vector2 offset;
        [NonReorderable]
        public Bioma[] biomas;
        public bool flatShading = false;
        private Land land;
        [SerializeField]
        private ComputeShader perlinShader;

        public void Generar()
        {
            if (land.gameObject == null)
                createLand();

            if (noiseMapGenerator == null)
                noiseMapGenerator = new PerlinNoise();

            noiseMapSetUp();
            float[,] map = noiseMapGenerator.GenerateNoiseMap(configOctaves());
            GenerateMapMeshAndTexture(map);
        }

        private List<PerlinOctave> configOctaves()
        {
            List<PerlinOctave> listOctaves = new List<PerlinOctave>();
            foreach (PerlinOctave octave in Octaves)
            {
                PerlinOctave newOctave = new PerlinOctave();
                newOctave.heightMapZoom = (1 / octave.heightMapZoom) * VerticesPerLine;
                newOctave.weight = octave.weight;
                newOctave.curveHeightMultiplier = octave.curveHeightMultiplier;
                newOctave.offset = octave.offset;

                listOctaves.Add(newOctave);
            }
            return listOctaves;
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

        private void GenerateMapMeshAndTexture(float[,] map)
        {
            Color[] biomaMap = calcularColourMapDeBiomas(map);
            Mesh meshTerreno = generateTerrainMesh(map).CreateMesh();
            Texture2D texture = TextureGenerator.textureFromColourMap(biomaMap, VerticesPerLine, VerticesPerLine);
            Material mat = new Material(baseMaterial);

            land.meshFilter.sharedMesh = meshTerreno;
            land.meshCollider.sharedMesh = meshTerreno;
            land.meshRenderer.sharedMaterial = mat;
            land.meshRenderer.sharedMaterial.mainTexture = texture;
        }

        private Color[] calcularColourMapDeBiomas(float[,] map)
        {
            Color[] colorMap = new Color[VerticesPerLine * VerticesPerLine];

            for (int y = 0; y < VerticesPerLine; y++)
                for (int x = 0; x < VerticesPerLine; x++)
                {
                    Bioma bioma = encontrarBiomaDeAltura(map[x, y]);
                    colorMap[y * VerticesPerLine + x] = bioma.color;
                }

            return colorMap;
        }

        private Bioma encontrarBiomaDeAltura(float altura)
        {
            Bioma minimoBioma = new Bioma();
            minimoBioma.color = Color.red;
            float minimaAltura = float.MaxValue;

            foreach (Bioma bioma in biomas)
            {
                if (altura <= bioma.altura && bioma.altura < minimaAltura)
                {
                    minimoBioma = bioma;
                    minimaAltura = bioma.altura;
                }
            }

            return minimoBioma;
        }

        private void noiseMapSetUp()
        {
            NoiseMapConfig noiseMapConfig = new NoiseMapConfig();
            noiseMapConfig.alto = VerticesPerLine;
            noiseMapConfig.ancho = VerticesPerLine;
            noiseMapConfig.seed = seed;
            noiseMapConfig.offset = offsetIsPosition ? new Vector2(transform.position.x, transform.position.z) : offset;

            noiseMapGenerator.configNoiseMap(noiseMapConfig);
        }

        private MeshBuilder generateTerrainMesh(float[,] heightMap)
        {
            MeshBuilder meshData = new MeshBuilder();

            addVertices(meshData, heightMap);
            createFaces(meshData);

            Debug.Log("Triangulos: " + meshData.Vertices.Count / 3);
            Debug.Log("Vertices: " + meshData.Vertices.Count);

            return meshData;
        }

        struct vertex
        {
            public float x;
            public float y;
            public float z;
        }
        private void addVertices(MeshBuilder meshData, float[,] heightMap)
        {
            vertex[] Vertices = calcularVertices();
            for (int i = 0; i < Vertices.Length; i++)
            {
                vertex vertex = Vertices[i];
                meshData.UVs.Add(new Vector2(vertex.x, vertex.y) / VerticesPerLine);
                Vector3 newVertex = new Vector3(vertex.x, vertex.y, vertex.z);
                meshData.AddVertice(newVertex);
            }
        }

        private vertex[] calcularVertices()
        {
            Vector3 initialPosition = transform.localPosition - (new Vector3(ChunkSize / 2, 0, ChunkSize / 2));
            float vertexDistance = ChunkSize / (float)(VerticesPerLine - 1);
            ComputeBuffer o_VertexBuffer = new ComputeBuffer(VerticesPerLine * VerticesPerLine, sizeof(float) * 3, ComputeBufferType.Default);

            perlinShader.SetInt("vertexPerLine", VerticesPerLine);
            perlinShader.SetFloat("vertexDistance", vertexDistance);
            perlinShader.SetFloat("generalMultiplier", generalHeightMultiplier);
            perlinShader.SetFloats("initialPosition", new float[] { initialPosition.x, initialPosition.y, initialPosition.z });
            perlinShader.SetFloats("offset", new float[] { transform.position.x, transform.position.z });

            int kernel = perlinShader.FindKernel("calculatePerlin");
            perlinShader.SetBuffer(kernel, "OutVertex", o_VertexBuffer);

            uint xGroupSize = 64;

            perlinShader.GetKernelThreadGroupSizes(kernel, out xGroupSize, out _, out _);
            int xGroups = Mathf.CeilToInt(VerticesPerLine * VerticesPerLine / (float)xGroupSize);
            if (xGroups == 0)
                xGroups = 1;
            perlinShader.Dispatch(kernel, xGroups, 1, 1);
            vertex[] Vertices = new vertex[o_VertexBuffer.count];
            o_VertexBuffer.GetData(Vertices, 0, 0, o_VertexBuffer.count);

            o_VertexBuffer.Dispose();
            return Vertices;
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
    }
}
