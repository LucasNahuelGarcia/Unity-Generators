using UnityEngine;
using System.Collections.Generic;

namespace Generador
{
    public class ChunkHandler : MonoBehaviour
    {
        private static ChunkHandler _instance;
        private Chunk[,,] chunks;
        private Vector3Int centroArr;
        public GameObject Pivot;
        public GameObject CuboEjemplo;
        public Generator generador;
        public int chunkRadius = 1;
        public float chunkSize = 5;
        private Vector3 velocidadChunks;
        private Vector3Int pivotPosition;
        private Vector3Int prevPivotPosition;


        void Start()
        {
            int chunkDiameter = chunkRadius * 2 + 1;
            centroArr = new Vector3Int(chunkRadius, chunkRadius, chunkRadius);
            chunks = new Chunk[chunkDiameter, chunkDiameter, chunkDiameter];
            this.Generar();
        }

        void Update()
        {
        }

        [ContextMenu("Generar")]
        private void Generar()
        {
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
            Chunk chunk = generado.AddComponent<Chunk>();
            BoxCollider trigger = generado.AddComponent<BoxCollider>();
            generado.tag = "Chunk";

            // Esto hay que cambiarlo
            // El chunk se tiene que hacer cargo de qué va a generar
            chunk.objetoAGenerar = CuboEjemplo;
            chunk.size = chunkSize;
            Vector3Int chunkIndex = new Vector3Int(x + chunkRadius, y + chunkRadius, z + chunkRadius);
            chunk.ChunkIndex = chunkIndex;
            chunk.ChunkHandler = this;
            chunks[chunkIndex.x, chunkIndex.y, chunkIndex.z] = chunk;
            chunk.Generar();

            trigger.isTrigger = true;
            trigger.size = new Vector3(chunkSize, chunkSize, chunkSize);

            return generado;
        }

        public void UpdatePivotPosition(Chunk chunk)
        {
            if (prevPivotPosition == null)
                prevPivotPosition = pivotPosition;
            pivotPosition = chunk.ChunkIndex;
            UpdateChunks();
        }

        // En algunos casos el pivot puede entrar y salir de un chunk sin
        // dejar el anterior, confunde al sistema de chunks.
        // Para corregirlo también aviso si el pivot deja un chunk.
        public void RestorePivotPosition(Chunk chunk)
        {
            Vector3Int restoringChunkPosition = chunk.ChunkIndex;
            if (restoringChunkPosition == prevPivotPosition)
                pivotPosition = prevPivotPosition;
            UpdateChunks();
        }

        private void UpdateChunks()
        {
            Debug.Log("Pivot en Chunk: " + pivotPosition);
            int centro = chunkRadius;
            if (pivotPosition.x != centro)
            {

            }
            if (pivotPosition.y != centro)
            {

            }
            if (pivotPosition.z != centro)
            {

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
