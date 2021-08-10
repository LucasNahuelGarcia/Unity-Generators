using UnityEngine;

namespace Generador
{
    public class ArraySpawner : MonoBehaviour
    {
        public int ancho = 10;
        public int alto = 10;
        public int profundidad = 10;
        public float espacio = 10;
        public float random = 2;
        public Material material;
        // Start is called before the first frame update
        void Start()
        {
            this.Generar();
        }

        [ContextMenu("Generar")]
        public void Generar()
        {
            for (int x = 0; x < ancho; x++)
                for (int y = 0; y < alto; y++)
                    for (int z = 0; z < profundidad; z++)
                    {
                        GameObject generado = new GameObject(x + " : " + y + " : " + z);
                        generado.transform.position = new Vector3(x * espacio - Random.value * random, y * espacio - Random.value * random, z * espacio - Random.value * random);

                        generado.AddComponent<MeshCollider>();
                        RockGeneratorVoronoi gen = generado.AddComponent<RockGeneratorVoronoi>();
                        MeshRenderer meshRender = generado.GetComponent<MeshRenderer>();
                        RockVoronoiShapeSettings shape = new RockVoronoiShapeSettings();

                        shape.random();
                        meshRender.sharedMaterial = material;
                        gen.shapeSettings = shape;

                        generado.transform.parent = this.transform;

                        gen.Generar();
                    }
        }

        [ContextMenu("Limpiar")]
        public void Limpiar()
        {
            while (transform.childCount != 0)
            {
                Transform child = transform.GetChild(0);
                DestroyImmediate(child.gameObject);
            }
        }
    }
}