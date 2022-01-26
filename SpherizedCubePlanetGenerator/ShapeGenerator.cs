using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeGenerator
{
  public MinMax elevationMinMax;
  private ShapeSettings settings;
  private NoiseFilter[] noiseFilters;

  public void updateSettings(ShapeSettings settings)
  {
    elevationMinMax = new MinMax();
    this.settings = settings;
    noiseFilters = new NoiseFilter[settings.noiseLayers.Length];
    for (int i = 0; i < noiseFilters.Length; i++)
    {
      noiseFilters[i] = new NoiseFilter(settings.noiseLayers[i].noiseSettings);
    }
  }

  public Vector3 calcularShapeOfPoint(Vector3 pointOnUnitSphere)
  {
    float elevation = 0;

    for (int i = 0; i < noiseFilters.Length; i++)
    {
      if (settings.noiseLayers[i].enabled)
        elevation += noiseFilters[i].Evaluate(pointOnUnitSphere);
    }

    elevation = settings.radio * (1 + elevation);

    elevationMinMax.addValue(elevation);

    return pointOnUnitSphere.normalized * elevation;
  }
}
