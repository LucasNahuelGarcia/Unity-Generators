using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generador.LandGenerator
{
    public class MapGenerator : MonoBehaviour
    {
        void Start()
        {
            Generate();
        }

        public Material baseMaterial;
        public noiseMap noiseMapGenerator;
        public bool autoUpdate = true;
        [Range(2, 1000)] //La cantidad de  vertices que hay en el ancho del mapa
        [Tooltip("How many vertex along width. VertexResolution*2=TotalVertex")]
        public int VerticesPerLine = 2;
        public int ChunkSize = 241;
        public float heightMapZoom = 0.3f;
        public int octavas = 3;
        [Range(0.0001f, 1.0f)]
        public float persistencia = 0.5f;
        [Range(0f, 8f)]
        public float lacunaridad = 2;
        public int seed = 1;
        public Vector2 offset;
        public float generalHeightMultiplier;
        public AnimationCurve curveHeightMultiplier;
        public Bioma[] biomas;
        public bool flatShading = false;
        private Land land;

        public void Generate()
        {
            if (land.gameObject == null)
                createLand();

            if (noiseMapGenerator == null)
                noiseMapGenerator = new PerlinNoise();

            noiseMapSetUp();
            float[,] map = noiseMapGenerator.generarNoiseMap();
            GenerateMapMeshAndTexture(map);
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

            land.meshFilter.sharedMesh = meshTerreno;
            land.meshCollider.sharedMesh = meshTerreno;
            land.meshRenderer.sharedMaterial = baseMaterial;
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
            noiseMapConfig.size = (1 / heightMapZoom) * VerticesPerLine;
            noiseMapConfig.octavas = octavas;
            noiseMapConfig.persistencia = persistencia;
            noiseMapConfig.lacunaridad = lacunaridad;
            noiseMapConfig.seed = seed;
            noiseMapConfig.offset = offset;

            noiseMapGenerator.configNoiseMap(noiseMapConfig);
        }

        void OnValidate()
        {
            octavas = octavas <= 0 ? 1 : octavas;
            heightMapZoom = heightMapZoom <= 0 ? 1 : heightMapZoom;
            lacunaridad = lacunaridad <= 0 ? 1 : lacunaridad;
            persistencia = persistencia < 0 ? 0 : persistencia;
            persistencia = persistencia > 1 ? 1 : persistencia;
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

        private void addVertices(MeshBuilder meshData, float[,] heightMap)
        {
            Vector3 initialPosition = Vector3.zero;
            float vertexDistance = ChunkSize / (float)(VerticesPerLine - 1);

            for (int z = 0; z < VerticesPerLine; z++)
                for (int x = 0; x < VerticesPerLine; x++)
                {
                    float vertex_X = initialPosition.x + (x * vertexDistance);
                    float vertex_Z = initialPosition.z + (z * vertexDistance);
                    float vertexHeight = curveHeightMultiplier.Evaluate(heightMap[x, z]) * generalHeightMultiplier;

                    Vector3 newVertex = new Vector3(vertex_X, vertexHeight, vertex_Z);

                    meshData.UVs.Add(new Vector2(x, z) / VerticesPerLine);
                    meshData.AddVertice(newVertex);
                }
            Debug.Log("Vertex distance = " + vertexDistance);
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
