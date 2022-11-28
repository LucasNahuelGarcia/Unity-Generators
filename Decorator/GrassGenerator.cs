using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Generador.LandGenerator
{

    public class GrassGenerator : MonoBehaviour, Decorator
    {
        public float GrassLODDistance = 130f;
        public float MaxGrassDistance = 230f;
        [Space()]
        public Mesh GrassMeshLOD0;
        public Material GrassMaterialLOD0;

        [Space()]
        public Mesh GrassMeshLOD1;
        public Material GrassMaterialLOD1;

        [Space()]
        public float InstancesPerMeter = 1;
        public float maxHeightToInstantiate = 5f;
        public float maxGrassHeight = .2f;
        public float grassScale = 1f;
        private Mesh landMesh;
        [SerializeField]
        private GPUInstancingBatches instancingBatches;
        [SerializeField]
        private const int submeshIndex = 0;

        private void OnEnable()
        {
        }

        [ContextMenu("Generate")]
        public void Decorate(Mesh mesh)
        {
            landMesh = mesh;
            instancingBatches = new GPUInstancingBatches();

            StartCoroutine(decorateAsync(mesh));
        }
        IEnumerator decorateAsync(Mesh mesh)
        {
            Vector3[] vertices = landMesh.vertices;

            float vertexDistance = Vector3.Distance(vertices[0], vertices[1]);
            float usableDistance = vertexDistance / 2;
            int instancesPerLine = (int)(InstancesPerMeter * usableDistance);
            float instanceGap = usableDistance / InstancesPerMeter;

            int yieldCounter = 0;
            int yieldTreshold = 700;

            foreach (Vector3 vertex in vertices)
            {
                yieldCounter++;
                if (vertex.y < maxHeightToInstantiate)
                {
                    addGrassMatrix(vertex);

                    Vector3 initialPosition = vertex - new Vector3(usableDistance, 0, usableDistance);
                    for (int x = 0; x < instancesPerLine; x++)
                        for (int z = 0; z < instancesPerLine; z++)
                            addGrassMatrix(initialPosition + new Vector3(x * instanceGap, 0, z * instanceGap));
                }
                if (yieldCounter >= yieldTreshold)
                {
                    yieldCounter = 0;
                    yield return null;
                }
            }
        }

        private void addGrassMatrix(Vector3 point)
        {
            float randX = Random.Range(-0.9f, .9f);
            float randZ = Random.Range(-0.9f, .9f);
            float randH = Mathf.PerlinNoise(point.x, point.y) * maxGrassHeight;
            float randR = Random.Range(0, 180f);

            Vector3 finalPosition = transform.position + new Vector3(point.x + randX, point.y + .5f, point.z + randZ);
            if (Physics.Raycast(finalPosition + Vector3.up, Vector3.down, out RaycastHit raycasthit) && Vector3.Angle(raycasthit.point, Vector3.up) > .2)
                finalPosition = raycasthit.point;
            Quaternion rotation = Quaternion.identity * Quaternion.Euler(0, randR, 0);
            Vector3 scale = new Vector3(grassScale, 0.02f + randH, grassScale);

            instancingBatches.AddMatrix(Matrix4x4.TRS(finalPosition, rotation, scale));
        }

        void Update()
        {
            float distanceToCamera = Vector3.Distance(Camera.main.transform.position, this.transform.position);
            if (distanceToCamera <= GrassLODDistance)
                renderBatches(GrassMeshLOD0, GrassMaterialLOD0);
            else if (distanceToCamera <= MaxGrassDistance)
                renderBatches(GrassMeshLOD1, GrassMaterialLOD1);
        }

        private void renderBatches(Mesh GrassMesh, Material GrassMaterial)
        {
            foreach (List<Matrix4x4> batch in instancingBatches.Batches)
                Graphics.DrawMeshInstanced(GrassMesh, 0, GrassMaterial, batch);
        }
    }
}