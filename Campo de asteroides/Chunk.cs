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
            Vector3 generadoPosition = Random.insideUnitSphere;
            Vector3 generadoRotation = Random.insideUnitSphere;

            BoxCollider col = this.gameObject.AddComponent<BoxCollider>();
            col.isTrigger = true;
            GameObject generado = Instantiate(objetoAGenerar, transform.position + generadoPosition * size, Quaternion.Euler(generadoRotation));
            RockGeneratorVoronoi voronoi = generado.GetComponent<RockGeneratorVoronoi>();
            voronoi.Generar();
            generado.transform.parent = this.transform;
        }

        private void OnTriggerEnter(Collider col)
        {
            if (GameObject.ReferenceEquals(col.gameObject, ChunkHandler.Pivot))
                ChunkHandler.UpdatePivotPosition(this);
        }
    }
}
