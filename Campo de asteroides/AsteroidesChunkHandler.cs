using UnityEngine;
using System.Collections.Generic;

namespace Generador
{
    public class AsteroidesChunkHandler : MonoBehaviour
    {
        public static AsteroidesChunkHandler Instance
        {
            get { return _instance; }
        }
        private static AsteroidesChunkHandler _instance;
        private List<ChunkCampoAsteroides> chunks;
        public GameObject CuboEjemplo;
        public Generator generador;
        public int chunkRadius = 1;
        public float chunkSize = 5;
        private Vector3 velocidadChunks;

        void Start()
        {
            this.Generar();
            if (_instance == null)
                _instance = this;
            else
                Debug.LogError("Existen dos o mas instancias de AsteroideChunkHandler");
        }

        void Update()
        {
            this.MoverChunks(velocidadChunks * Time.deltaTime);
        }

        [ContextMenu("Generar")]
        private void Generar()
        {
            chunks = new List<ChunkCampoAsteroides>();
            for (int x = -chunkRadius; x <= chunkRadius; x++)
                for (int z = -chunkRadius; z <= chunkRadius; z++)
                    for (int y = -chunkRadius; y <= chunkRadius; y++)
                        AgregarChunk(x, y, z);
        }

        private GameObject AgregarChunk(int x, int y, int z)
        {
            GameObject generado = new GameObject("chunk:" + x + " : " + y + " : " + z);
            generado.transform.parent = this.transform;
            generado.transform.position = new Vector3(
                transform.position.x + chunkSize * x,
                transform.position.y + chunkSize * y,
                transform.position.z + chunkSize * z
            );
            ChunkCampoAsteroides chunk = generado.AddComponent<ChunkCampoAsteroides>();
            BoxCollider trigger = generado.AddComponent<BoxCollider>();
            generado.tag = "Chunk";

            // Esto hay que cambiarlo
            // El chunk se tiene que hacer cargo de qué va a generar
            chunk.objetoAGenerar = CuboEjemplo;
            chunk.size = chunkSize;
            chunks.Add(chunk);
            chunk.Generar();

            trigger.isTrigger = true;
            trigger.size = new Vector3(chunkSize, chunkSize, chunkSize);

            return generado;
        }

        public void MoverChunks(Vector3 direccion)
        {
            foreach (ChunkCampoAsteroides chunk in chunks)
            {
                chunk.transform.Translate(direccion);
            }
        }

        public void SetVelocidadChunks(Vector3 velocidad)
        {
            this.velocidadChunks = velocidad;
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
