using UnityEngine;

public class NoiseFilter
{
  NoiseSettings noiseSettings;
  Noise noise = new Noise();

  public NoiseFilter(NoiseSettings noiseSettings)
  {
    this.noiseSettings = noiseSettings;
  }

  public float Evaluate(Vector3 point)
  {
    float noiseValue = 0;
    float frecuencia = noiseSettings.baseRoughness;
    float amplitud = 1;

    for (int i = 0; i < noiseSettings.numLayers; i++){
        float v = noise.Evaluate(point * frecuencia + noiseSettings.centre);
        noiseValue += (v+1) * .5f * amplitud;

        frecuencia *= noiseSettings.roughness;
        amplitud *= noiseSettings.persistencia;
    }

    noiseValue = Mathf.Max(0, noiseValue - noiseSettings.minValue);

    return noiseValue * noiseSettings.strength;
  }
}