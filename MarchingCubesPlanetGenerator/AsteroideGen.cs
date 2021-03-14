using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class AsteroideGen : MonoBehaviour
{
  public bool autoUpdate = false;
  public MarchingCubesPlanetShapeSettings shapeSettings;
  private MeshBuilder mesh;
  private Noise noise;
  public Vector3 rotacion;
  public void build()
  {
    mesh = new MeshBuilder();
    noise = new Noise();
    crearMesh();
  }
  void crearMesh()
  {
    float radioForma = shapeSettings.radioForma;
    MarchingCubes marching = new MarchingCubes(shapeSettings.densidadCubos, mesh, shapeSettings.valorMinimoForma, valuePoint);
    
    for (float z = -radioForma; z < 2 * radioForma; z += shapeSettings.densidadCubos)
      for (float x = -radioForma; x < 2 * radioForma; x += shapeSettings.densidadCubos)
        for (float y = -radioForma; y < 2 * radioForma; y += shapeSettings.densidadCubos)
        {
          Vector3 punto = new Vector3(x, y, z);
          marching.analizarMarchingCube(punto);
        }

    Mesh meshResultado = mesh.crearMesh();
    GetComponent<MeshFilter>().sharedMesh = meshResultado;
    GetComponent<MeshCollider>().sharedMesh = meshResultado;
  }
  
  private float valuePoint(Vector3 punto)
  {
    float val = 0;
    float noiseRadio = shapeSettings.radioForma;

    if (shapeSettings.noiseSuperficie)
    {
      Vector3 puntoRadio = punto * shapeSettings.esacalaNoiseSuperficie + shapeSettings.shiftNoiseSuperficie;
      noiseRadio += noise.Evaluate(puntoRadio) * shapeSettings.fuerzaNoiseSuperficie - 1;
      noiseRadio = Mathf.Clamp(noiseRadio, 0, shapeSettings.radioForma);
    }

    if (punto.magnitude < noiseRadio * shapeSettings.piso)
    {
      val = Mathf.InverseLerp(noiseRadio * shapeSettings.piso, noiseRadio * shapeSettings.piso * shapeSettings.factorRadioSuavizado, punto.magnitude);
      val = (1f - shapeSettings.valorMinimoForma) * val + shapeSettings.valorMinimoForma;
    }
    else if (punto.magnitude < noiseRadio)
      val = Mathf.InverseLerp(-1, 1, noise.Evaluate(punto * shapeSettings.escalaNoiseForma + shapeSettings.shiftNoiseForma));

    if (val >= shapeSettings.valorMinimoForma && punto.magnitude < noiseRadio && punto.magnitude > noiseRadio * shapeSettings.factorRadioSuavizado)
      val = Mathf.InverseLerp(noiseRadio, noiseRadio * shapeSettings.factorRadioSuavizado, punto.magnitude);

    return val;
  }

  void Update() {
    this.transform.Rotate(rotacion * Time.deltaTime);
  }
}
