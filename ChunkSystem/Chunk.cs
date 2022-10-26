using UnityEngine;

namespace Generador
{
    public class Chunk : MonoBehaviour
    {
        public GameObject objetoAGenerar;
        private GameObject objetoGenerado;
        private Generator objetoGeneradoGenerator;
        public ChunkHandler ChunkHandler;
        public Vector3Int ChunkIndex;
        public float size;

        [ContextMenu("Generar")]
        public void Generar()
        {

            if (objetoGenerado == null)
                objetoGenerado = Instantiate(objetoAGenerar, transform.position, Quaternion.identity);

            objetoGenerado.transform.parent = this.transform;
            Generator gen = objetoGenerado.GetComponent<Generator>();
            if (gen != null)
                gen.Generar();
        }

        private void OnTriggerEnter(Collider col)
        {
            if (GameObject.ReferenceEquals(col.gameObject, ChunkHandler.Pivot)){
                ChunkHandler.UpdatePivotPosition(this);
            }
        }
    }
}
