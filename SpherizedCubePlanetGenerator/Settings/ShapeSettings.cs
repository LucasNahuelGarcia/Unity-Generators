using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ShapeSettings: ScriptableObject 
{
  public float radio;
  public NoiseLayer[] noiseLayers;
  [System.Serializable]
  public class NoiseLayer {
    public bool enabled = true;
    public NoiseSettings noiseSettings;
  }
}
