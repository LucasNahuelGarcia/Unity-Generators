using UnityEngine;
using System.Collections.Generic;

namespace Generador
{
    public class ChunkHandler : MonoBehaviour
    {
        private static ChunkHandler _instance;
        private List<Chunk> chunks;
        private Vector3Int centroArr;
        // El pivot es el objeto alrededor del que se mueven
        // los chunks
        public bool IncludeYDirection;
        public GameObject Pivot;
        public GameObject GeneratedObject;
        public int chunkRadius = 1;
        public float chunkSize = 5;
        private Vector3 velocidadChunks;
        private Vector3Int pivotPosition;
        private Vector3Int prevPivotPosition;


        void Start()
        {
            centroArr = new Vector3Int(chunkRadius, chunkRadius, chunkRadius);
            this.Generar();
        }

        [ContextMenu("Generar")]
        private void Generar()
        {
            chunks = new List<Chunk>();

            if (IncludeYDirection)
                for (int x = -chunkRadius; x <= chunkRadius; x++)
                    for (int z = -chunkRadius; z <= chunkRadius; z++)
                        for (int y = -chunkRadius; y <= chunkRadius; y++)
                            AgregarChunk(x, y, z);
            else
                for (int x = -chunkRadius; x <= chunkRadius; x++)
                    for (int z = -chunkRadius; z <= chunkRadius; z++)
                        AgregarChunk(x, 0, z);
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
            chunk.objetoAGenerar = GeneratedObject;
            chunk.size = chunkSize;
            Vector3Int chunkIndex = new Vector3Int(x + chunkRadius, y + chunkRadius, z + chunkRadius);
            chunk.ChunkIndex = chunkIndex;
            chunk.ChunkHandler = this;
            chunks.Add(chunk);
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

        private void UpdateChunks()
        {
            //TODO: REFACTOR
            Debug.Log("Pivot en Chunk: " + pivotPosition);
            int centro = chunkRadius;
            int indiceAlFrente = (chunkRadius * 2 + 1);

            if (pivotPosition.x > centro)
            {
                foreach (Chunk chunk in chunks)
                {
                    if (chunk.ChunkIndex.x == 0)
                    {
                        chunk.transform.Translate(trunc(indiceAlFrente * chunkSize), 0, 0);
                        chunk.ChunkIndex.x = chunkRadius * 2;
                        chunk.Generar();
                    }
                    else
                        chunk.ChunkIndex.x = chunk.ChunkIndex.x - 1;
                }
            }
            else if (pivotPosition.x < centro)
            {
                foreach (Chunk chunk in chunks)
                {
                    if (chunk.ChunkIndex.x == chunkRadius * 2)
                    {
                        chunk.transform.Translate(trunc(-indiceAlFrente * chunkSize), 0, 0);
                        chunk.ChunkIndex.x = 0;
                        chunk.Generar();

                    }
                    else
                        chunk.ChunkIndex.x = chunk.ChunkIndex.x + 1;
                }
            }
            if (pivotPosition.y > centro)
            {
                foreach (Chunk chunk in chunks)
                {
                    if (chunk.ChunkIndex.y == 0)
                    {
                        chunk.transform.Translate(0, trunc(indiceAlFrente * chunkSize), 0);
                        chunk.Generar();

                        chunk.ChunkIndex.y = chunkRadius * 2;
                    }
                    else
                        chunk.ChunkIndex.y = chunk.ChunkIndex.y - 1;
                }
            }
            else if (pivotPosition.y < centro)
            {
                foreach (Chunk chunk in chunks)
                {
                    if (chunk.ChunkIndex.y == chunkRadius * 2)
                    {
                        chunk.transform.Translate(0, trunc(-indiceAlFrente * chunkSize), 0);
                        chunk.Generar();

                        chunk.ChunkIndex.y = 0;
                    }
                    else
                        chunk.ChunkIndex.y = chunk.ChunkIndex.y + 1;
                }
            }
            if (pivotPosition.z > centro)
            {
                foreach (Chunk chunk in chunks)
                {
                    if (chunk.ChunkIndex.z == 0)
                    {
                        chunk.transform.Translate(0, 0, trunc(indiceAlFrente * chunkSize));
                        chunk.Generar();

                        chunk.ChunkIndex.z = chunkRadius * 2;
                    }
                    else
                        chunk.ChunkIndex.z = chunk.ChunkIndex.z - 1;
                }
            }
            else if (pivotPosition.z < centro)
            {
                foreach (Chunk chunk in chunks)
                {
                    if (chunk.ChunkIndex.z == chunkRadius * 2)
                    {
                        chunk.transform.Translate(0, 0, trunc(-indiceAlFrente * chunkSize));
                        chunk.Generar();

                        chunk.ChunkIndex.z = 0;
                    }
                    else
                        chunk.ChunkIndex.z = chunk.ChunkIndex.z + 1;
                }
            }
        }

        private float trunc(float val) {
            int truncVal = (int) val;
            return truncVal;
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
