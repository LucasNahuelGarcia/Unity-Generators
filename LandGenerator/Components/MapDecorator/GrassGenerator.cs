using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Generador.LandGenerator;
using System.Linq;

public class ObjData
{
    public Vector3 Position;
    public Vector3 Scale;
    public Quaternion Rotation;
    public Matrix4x4 matrix
    {
        get
        {
            return Matrix4x4.TRS(Position, Rotation, Scale);
        }
    }

    public ObjData(Vector3 position, Vector3 scale, Quaternion rotation)
    {
        this.Position = position;
        this.Scale = scale;
        this.Rotation = rotation;
    }
}

public class GrassGenerator : MonoBehaviour, Decorator
{
    private ComputeBuffer landVertexComputeBuffer;
    private const int _BatchesLimit = 1000;
    public Mesh GrassMesh;
    public Material GrassMaterial;
    public int Instances;
    public Mesh Surface;
    private List<List<ObjData>> batches = new List<List<ObjData>>();

    ComputeBuffer meshTriangles;
    ComputeBuffer meshPositions;
    ComputeBuffer resultBuffer;

    private void Start()
    {
    }

    [ContextMenu("Generate")]
    public void Decorate(ComputeBuffer buffer)
    {

        int[] triangles = GrassMesh.triangles;
        meshTriangles = new ComputeBuffer(triangles.Length, sizeof(int));
        meshTriangles.SetData(triangles);

        Vector3[] positions = Surface.vertices;
        meshPositions = new ComputeBuffer(positions.Length, sizeof(float) * 3);
        meshPositions.SetData(positions);
        landVertexComputeBuffer = buffer;
        renderBatches();
    }

    void Update()
    {
        if (landVertexComputeBuffer != null)
            renderBatches();
    }

    private void renderBatches()
    {
        // foreach (var batch in batches)
        {
            Graphics.DrawMeshInstancedIndirect(GrassMesh, 0, GrassMaterial, new Bounds(Vector3.zero, new Vector3(100.0f, 100.0f, 100.0f)), landVertexComputeBuffer);
        }
    }
    void OnDestroy()
    {
        resultBuffer.Dispose();
        meshTriangles.Dispose();
        meshPositions.Dispose();
    }
}
