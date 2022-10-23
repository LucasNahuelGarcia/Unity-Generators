using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcosphereCreator : GeneradorDeEsferas
{
  private MeshBuilder meshBuilder;
  private EsferaConfig config;
  public Mesh generarEsfera(EsferaConfig esferaConfig)
  {
    meshBuilder = new MeshBuilder();
    this.config = esferaConfig;

    crearOctahedroTruncado();

    for (int r = 0; r < config.detalle; r++)
      refinarEsfera();

    return meshBuilder.CreateMesh();
  }


  private void crearOctahedroTruncado()
  {
    float gr = (0.0f + Mathf.Sqrt(5.0f)) / 2f;

    agregarVerticeNormalizado(-1, gr, 0);
    agregarVerticeNormalizado(1, gr, 0);
    agregarVerticeNormalizado(-1, -gr, 0);
    agregarVerticeNormalizado(1, -gr, 0);

    agregarVerticeNormalizado(0, -1, gr);
    agregarVerticeNormalizado(0, 1, gr);
    agregarVerticeNormalizado(0, -1, -gr);
    agregarVerticeNormalizado(0, 1, -gr);

    agregarVerticeNormalizado(gr, 0, -1);
    agregarVerticeNormalizado(gr, 0, 1);
    agregarVerticeNormalizado(-gr, 0, -1);
    agregarVerticeNormalizado(-gr, 0, 1);

    meshBuilder.addTriangle(0, 11, 5);
    meshBuilder.addTriangle(0, 5, 1);
    meshBuilder.addTriangle(0, 1, 7);
    meshBuilder.addTriangle(0, 7, 10);
    meshBuilder.addTriangle(0, 10, 11);

    meshBuilder.addTriangle(1, 5, 9);
    meshBuilder.addTriangle(5, 11, 4);
    meshBuilder.addTriangle(11, 10, 2);
    meshBuilder.addTriangle(10, 7, 6);
    meshBuilder.addTriangle(7, 1, 8);

    meshBuilder.addTriangle(3, 9, 4);
    meshBuilder.addTriangle(3, 4, 2);
    meshBuilder.addTriangle(3, 2, 6);
    meshBuilder.addTriangle(3, 6, 8);
    meshBuilder.addTriangle(3, 8, 9);

    meshBuilder.addTriangle(4, 9, 5);
    meshBuilder.addTriangle(2, 4, 11);
    meshBuilder.addTriangle(6, 2, 10);
    meshBuilder.addTriangle(8, 6, 7);
    meshBuilder.addTriangle(9, 8, 1);
  }
  private int agregarVerticeNormalizado(float a, float b, float c)
  {

    Vector3 nuevoVertice = new Vector3(a, b, c).normalized;
    int indice = meshBuilder.Vertices.Count;

    float transformacion = getTransformacion(a, b, c) * config.fuerzaTransformacion;
    float radioPunto = config.radio + transformacion;

    meshBuilder.Vertices.Add(nuevoVertice * radioPunto);
    meshBuilder.Normals.Add(new Vector3(a, b, c).normalized);

    return indice;
  }
  private int agregarVerticeNormalizado(Vector3 vector)
  {
    return agregarVerticeNormalizado(vector.x, vector.y, vector.z);
  }

 private float getTransformacion(float x, float y, float z)
  {
    return Mathf.PerlinNoise(Mathf.Sin(x)+ config.shift.x, Mathf.Cos(y)+ config.shift.y);
  }

  private void refinarEsfera()
  {
    List<int> indices = meshBuilder.triangulosIndice;

    int cantIndicesAntesDeRefinar = indices.Count;
    for (int i = 0; i < cantIndicesAntesDeRefinar; i += 3)
    {
      int ia = indices[i];
      Vector3 a = meshBuilder.Vertices[ia];
      int ib = indices[i + 1];
      Vector3 b = meshBuilder.Vertices[ib];
      int ic = indices[i + 2];
      Vector3 c = meshBuilder.Vertices[ic];

      int iab = agregarVerticeNormalizado(MeshBuilder.centro2Vectores(a, b));
      int iac = agregarVerticeNormalizado(MeshBuilder.centro2Vectores(a, c));
      int ibc = agregarVerticeNormalizado(MeshBuilder.centro2Vectores(b, c));

      indices[i + 1] = iab;
      indices[i + 2] = iac;

      meshBuilder.addTriangle(iab, ib, ibc);
      meshBuilder.addTriangle(ibc, ic, iac);
      meshBuilder.addTriangle(iac, iab, ibc);
    }
  }
}
