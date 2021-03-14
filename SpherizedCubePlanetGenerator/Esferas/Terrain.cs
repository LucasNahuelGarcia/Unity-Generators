using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terrain
{
  private MeshBuilder meshBuilder;
  private Vector3 localUp;
  private Vector3 ejeA;
  private Vector3 ejeB;
  private int detalle;
  private float radio;
  private ShapeGenerator shapeGenerator;
  public Terrain(TerrainConfig config)
  {
    shapeGenerator = config.shapeGenerator;
    meshBuilder = config.meshBuilder;
    localUp = config.localUp;
    ejeA = new Vector3(localUp.y, localUp.z, localUp.x);
    ejeB = Vector3.Cross(localUp, ejeA);
    detalle = config.detalle;
  }

  public MeshBuilder constructMesh()
  {
    int cantVertices = 0;
    for (int y = 0; y < detalle; y++)
      for (int x = 0; x < detalle; x++)
      {
        Vector2 porcentaje = new Vector2(x, y) / (detalle - 1);
        Vector3 pointOnUnitCUbe = localUp + (porcentaje.x - 0.5f) * 2 * ejeA + (porcentaje.y - 0.5f) * 2 * ejeB;
        Vector3 pointOnSphereProj = pointOnUnitCUbe.normalized;

        meshBuilder.Vertices.Add(shapeGenerator.calcularShapeOfPoint(pointOnSphereProj));

        if (x < detalle - 1 && y < detalle - 1)
        {
          meshBuilder.addTriangle(cantVertices, cantVertices + 1, cantVertices + detalle + 1);
          meshBuilder.addTriangle(cantVertices, cantVertices + detalle + 1, cantVertices + detalle);
        }

        cantVertices++;
      }

    return meshBuilder;
  }

}