using UnityEngine;

[CreateAssetMenu()]
public class MarchingCubesPlanetShapeSettings : ScriptableObject
{
  public float escalaNoiseForma = 1f;
  public Vector3 shiftNoiseForma = new Vector3(0, 0, 0);
  public float radioForma = 1;
  [Range(0, 1)]
  public float valorMinimoForma = .5f;
  public float densidadCubos = 1f;
  public bool noiseSuperficie = false;
  public float esacalaNoiseSuperficie = 1f;
  public float fuerzaNoiseSuperficie = 1f;
  [Range(0, 1)]
  public float piso = 0f;
  public Vector3 shiftNoiseSuperficie = new Vector3(0, 0, 0);
  [Range(0, 1)]
  public float factorRadioSuavizado = .2f;
}