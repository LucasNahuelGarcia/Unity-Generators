using UnityEngine;

[System.Serializable]
public class NoiseSettings
{
  public float strength = 1;
  public float baseRoughness = 1;
  public float roughness = 1;
  [Range(1,8)]
  public int numLayers = 1;
  [Range(0,2)]
  public float persistencia = .5f;
  public Vector3 centre;

public float minValue = 0;
}