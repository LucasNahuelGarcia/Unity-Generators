using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassGenerator : MonoBehaviour
{

  public struct Pasto
  {
    public Vector3 a;
    public Vector3 b;
    public Vector3 c;
    public Vector3 d;
  }
  public ComputeShader compute;
  public GameObject grassObject;
  [Range(0, 1)]
  public float anguloMaximo = .5f;
  [Range(1, 50)]
  public int pastoPorMetro = 1;
  [Range(0.001f,0.5f)]
  public float radioTallo = .1f;
  [Min(0)]
  public float alturaMinima = .2f;
  [Min(0)]
  public float alturaMaxima = .4f;
  [Range(0, .1f)]
  public float jitterAmount = .1f;
  [Range(0, 2)]
  private MeshBuilder mesh;
  public void generateMesh()
  {
    mesh = new MeshBuilder();

    gpu_generateGrassOnObject(grassObject);

    GetComponent<MeshFilter>().sharedMesh = mesh.CrearMesh();
  }
  private void gpu_generateGrassOnObject(GameObject superficie)
  {
    Mesh meshSuperficie = superficie.GetComponent<MeshFilter>().sharedMesh;

    ComputeBuffer i_trisBuffer = new ComputeBuffer(meshSuperficie.triangles.Length, sizeof(int), ComputeBufferType.Structured);
    ComputeBuffer i_verticesBuffer = new ComputeBuffer(meshSuperficie.vertices.Length, sizeof(float) * 3, ComputeBufferType.Structured);
    ComputeBuffer o_pastoBuffer = new ComputeBuffer((meshSuperficie.triangles.Length / 3) * pastoPorMetro * 2, sizeof(float) * 3 * 4, ComputeBufferType.Append);

    int kernel = compute.FindKernel("crearPastito");
    uint threadGroupSize;
    int dispatchSize;

    i_verticesBuffer.SetData(meshSuperficie.vertices);
    i_trisBuffer.SetData(meshSuperficie.triangles);

    configComputeShader(i_trisBuffer, i_verticesBuffer, o_pastoBuffer, kernel);

    compute.GetKernelThreadGroupSizes(kernel, out threadGroupSize, out _, out _);
    dispatchSize = Mathf.CeilToInt(((float)meshSuperficie.triangles.Length) / threadGroupSize);
    Debug.Log("dispatchSize: " + dispatchSize);
    compute.Dispatch(kernel, dispatchSize, 1, 1);

    Pasto[] pastos = new Pasto[o_pastoBuffer.count];
    o_pastoBuffer.GetData(pastos, 0, 0, o_pastoBuffer.count);

    foreach (Pasto pasto in pastos)
      if (!float.IsNaN(pasto.a.x))
        agregarPastoAMesh(pasto);

    Debug.Log("Cantidad de pasto: " + pastos.Length);
  }

  private void configComputeShader(ComputeBuffer i_trisBuffer, ComputeBuffer i_verticesBuffer, ComputeBuffer o_pastoBuffer, int kernel)
  {
    compute.SetBuffer(kernel, "_triangulos", i_trisBuffer);
    compute.SetBuffer(kernel, "_vertices", i_verticesBuffer);
    compute.SetBuffer(kernel, "_outPasto", o_pastoBuffer);
    compute.SetFloat("radioTallo", radioTallo);
    compute.SetFloat("alturaMinima", alturaMinima);
    compute.SetFloat("alturaMaxima", alturaMaxima);
    compute.SetFloat("anguloMaximo", anguloMaximo);
    compute.SetInt("pastoPorMetro", pastoPorMetro);
  }

  private void agregarPastoAMesh(Pasto pasto)
  {
    int iA = mesh.addVertice(pasto.a);
    int iB = mesh.addVertice(pasto.b);
    int iC = mesh.addVertice(pasto.c);
    int iD = mesh.addVertice(pasto.d);

    mesh.addTriangle(iA, iB, iD);
    mesh.addTriangle(iB, iC, iD);
    mesh.addTriangle(iC, iA, iD);
  }
}
