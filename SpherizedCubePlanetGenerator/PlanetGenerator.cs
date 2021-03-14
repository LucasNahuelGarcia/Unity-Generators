using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlanetGenerator : MonoBehaviour
{
  public ShapeSettings shapeSettings;
  public ColorSettings colorSettings;
  public bool autoUpdate;
  [Range(2, 256)]
  public int cantVertices = 1;
  [SerializeField, HideInInspector]
  private MeshFilter[] meshFilters;
  private Terrain[] caras;
  private ShapeGenerator shapeGenerator = new ShapeGenerator();
  private ColourGenerator colourGenerator = new ColourGenerator();
  void Initialize()
  {
    shapeGenerator.updateSettings(shapeSettings);
    colourGenerator.updateSettings(colorSettings);

    Vector3[] direcciones = new Vector3[] {
      Vector3.up,Vector3.down,Vector3.left,Vector3.right,Vector3.forward, Vector3.back
    };

    if (meshFilters == null || meshFilters.Length == 0)
      meshFilters = new MeshFilter[6];


    caras = new Terrain[6];
    for (int i = 0; i < 6; i++)
    {
      if (meshFilters[i] == null)
      {
        GameObject meshObject = new GameObject("mesh");
        meshObject.transform.parent = this.transform;

        MeshRenderer meshrenderer = meshObject.AddComponent<MeshRenderer>();
        meshrenderer.sharedMaterial = colorSettings.material;
        meshFilters[i] = meshObject.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        meshFilters[i].sharedMesh = mesh;
        meshObject.AddComponent<MeshCollider>().sharedMesh = mesh;
      }

      TerrainConfig config = new TerrainConfig();
      config.meshBuilder = new MeshBuilder(meshFilters[i].sharedMesh);
      config.detalle = this.cantVertices;
      config.localUp = direcciones[i];
      config.shapeGenerator = shapeGenerator;

      caras[i] = new Terrain(config);
    }
  }

  public void onColorSettingsUpdate()
  {
    Initialize();
    generateColor();
  }

  public void onShapeSettingsUpdate()
  {
    Initialize();
    generateMesh();
  }

  public void generarAsteroide()
  {
    Initialize();
    generateMesh();
    generateColor();
  }

  public void generateMesh()
  {
    foreach (Terrain terr in caras)
    {
      terr.constructMesh().crearMesh();
    }

    colourGenerator.updateElevation(shapeGenerator.elevationMinMax);
  }

  public void generateColor()
  {
    colourGenerator.updateColours();
    Debug.Log("GenerandoColor");
  }
}