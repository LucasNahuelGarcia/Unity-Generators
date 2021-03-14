using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshBuilder
{
  private List<Vector3> m_Vertices = new List<Vector3>();
  private List<Vector3> m_Normals = new List<Vector3>();
  private List<Vector2> m_UVs = new List<Vector2>();
  private List<int> m_Indices = new List<int>();

  public List<Vector3> Vertices { get { return m_Vertices; } }
  public List<Vector3> Normals { get { return m_Normals; } }
  public List<Vector2> UVs { get { return m_UVs; } }
  public List<int> triangulosIndice { get { return m_Indices; } }
  private Mesh mesh;

  public MeshBuilder(Mesh mesh)
  {
    this.mesh = mesh;
  }

  public MeshBuilder()
  {
    this.mesh = new Mesh();
  }
  public void addTriangle(int a, int b, int c)
  {
    m_Indices.Add(a);
    m_Indices.Add(b);
    m_Indices.Add(c);
  }
  public int addIfNotFound(Vector3 punto)
  {
    int indice = this.Vertices.Count;

    int indiceEncontrado = this.Vertices.BinarySearch(punto, new Vector3Comparer(.01f));
    if (indiceEncontrado >= 0)
      indice = indiceEncontrado;
    else
      this.Vertices.Add(punto);

    return indice;
  }
  public int addVertice(Vector3 punto)
  {
    int indice = this.Vertices.Count;
    this.Vertices.Add(punto);

    return indice;
  }
  public Mesh crearMesh()
  {
    mesh.Clear();
    if (m_Vertices.Count >= 65000)
    {
      mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
      Debug.Log("Mas de 65000, pasando a indice de 32 bits");
    }

    mesh.vertices = m_Vertices.ToArray();
    mesh.triangles = m_Indices.ToArray();

    if (m_Normals.Count == m_Vertices.Count)
      mesh.normals = m_Normals.ToArray();
    else
      mesh.RecalculateNormals();

    if (m_UVs.Count == m_Vertices.Count)
      mesh.uv = m_UVs.ToArray();

    mesh.RecalculateBounds();

    return mesh;
  }
  public static Vector3 centro2Vectores(Vector3 a, Vector3 b)
  {
    return (a + b) / 2;
  }
}
