using UnityEngine;

[CreateAssetMenu()]
public class RockVoronoiShapeSettings : ScriptableObject
{
  public float radio = 1;
  public float densidadCubos = 1f;
  public float escalaVoronoi = .1f;
  public Vector3 shiftVoronoi = new Vector3(0, 0, 0);
  [Range(0, 1)]
  public float treshold = .5f;
  public bool noiseSuperficie = false;
  public float esacalaNoiseSuperficie = .1f;
  public float fuerzaNoiseSuperficie = .5f;
  [Range(0, 1)]
  public float porcentajeLimiteInterior = 0f;
  public Vector3 shiftNoiseSuperficie = new Vector3(0, 0, 0);
  [Range(0, 1)]
  public float factorSuavizado = .2f;

  public void random() {
        escalaVoronoi = 0.05f;
        shiftVoronoi = new Vector3(Random.value * 10, Random.value * 10, Random.value * 10);
        radio = 11 + Random.value * 10;
        treshold = .19f;
        noiseSuperficie = true;
        esacalaNoiseSuperficie = .1f;
        fuerzaNoiseSuperficie = 1 + Random.value ;
        porcentajeLimiteInterior = 0.05f;
        shiftNoiseSuperficie = new Vector3(Random.value * 10, Random.value * 10, Random.value * 10);
    }
}
