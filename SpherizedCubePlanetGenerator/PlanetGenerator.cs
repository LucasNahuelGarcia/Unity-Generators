using UnityEngine;


public class PlanetGenerator : MonoBehaviour, Generator
{
    public ShapeSettings shapeSettings;
    public Material Material;
    public bool autoUpdate;
    [Range(2, 256)] public int cantVertices = 1;
    [SerializeField, HideInInspector] private MeshFilter[] meshFilters;
    private Terrain[] caras;
    private ShapeGenerator shapeGenerator = new ShapeGenerator();

    void Initialize()
    {
        shapeGenerator.updateSettings(shapeSettings);
        Vector3[] direcciones = new Vector3[]
        {
            Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back
        };

        if (meshFilters == null || meshFilters.Length == 0)
            meshFilters = new MeshFilter[6];


        caras = new Terrain[6];
        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i] == null)
            {
                GameObject meshObject = new GameObject("mesh");
                meshObject.transform.parent = this.transform;

                MeshRenderer meshrenderer = meshObject.AddComponent<MeshRenderer>();
                meshrenderer.sharedMaterial = Material;
                meshFilters[i] = meshObject.AddComponent<MeshFilter>();
                Mesh mesh = new Mesh();
                meshFilters[i].sharedMesh = mesh;
                meshObject.AddComponent<MeshCollider>().sharedMesh = mesh;
            }

            TerrainConfig config = new TerrainConfig();
            config.meshBuilder = new MeshBuilder(meshFilters[i].sharedMesh);
            config.detalle = this.cantVertices;
            config.localUp = direcciones[i];
            config.shapeGenerator = shapeGenerator;

            caras[i] = new Terrain(config);
        }
    }

    public void onShapeSettingsUpdate()
    {
        Initialize();
        generateMesh();
    }

    public void Generar()
    {
        Initialize();
        generateMesh();
    }

    public void SetCalidadMesh(int cant)
    {
        this.cantVertices = cant;
    }

    public void generateMesh()
    {
        foreach (Terrain terr in caras)
        {
            terr.constructMesh().CrearMesh();
        }
    }
}