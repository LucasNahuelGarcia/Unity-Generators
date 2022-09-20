using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generador
{
    public class Chunk : MonoBehaviour
    {
        public GameObject objetoAGenerar;
        public ChunkHandler ChunkHandler;
        public Vector3Int ChunkIndex;
        public float size;

        [ContextMenu("Generar")]
        public void Generar()
        {
            BoxCollider col = this.gameObject.AddComponent<BoxCollider>();
            col.isTrigger = true;
            GameObject generado = Instantiate(objetoAGenerar, transform.position, transform.rotation);
            generado.transform.parent = this.transform;
        }

        private void OnTriggerEnter(Collider col)
        {
            if (GameObject.ReferenceEquals(col.gameObject, ChunkHandler.Pivot))
                ChunkHandler.UpdatePivotPosition(this);
        }
        private void OnTriggerExit(Collider col)
        {
            if (GameObject.ReferenceEquals(col.gameObject, ChunkHandler.Pivot))
                ChunkHandler.RestorePivotPosition(this);
        }
    }
}
