using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoise : noiseMap
{
  private static int _RANGO_RANDOM = 100000;
  private float maxNoiseHeight, minNoiseHeight;
  private int ancho, alto, octavas;
  private float escala, persistencia, lacunaridad;
  private int seed;
  private Vector2 offset;
  public void configNoiseMap(NoiseMapConfig config)
  {
    ancho = config.ancho;
    alto = config.alto;
    octavas = config.octavas;
    escala = config.size <= 0 ? 0.000001f : config.size;
    persistencia = config.persistencia;
    lacunaridad = config.lacunaridad;
    seed = config.seed;
    offset = config.offset;
  }
  public float[,] GenerateNoiseMap()
  {
    minNoiseHeight = float.MaxValue;
    maxNoiseHeight = float.MinValue;
    float[,] perlinNoiseMap = calcularMapa();
    normalizarMapa(perlinNoiseMap, maxNoiseHeight, minNoiseHeight);

    return perlinNoiseMap;
  }

  private float[,] calcularMapa()
  {
    float[,] perlinNoiseMap = new float[ancho, alto];


    for (int x = 0; x < ancho; x++)
      for (int y = 0; y < alto; y++)
        perlinNoiseMap[x, y] = calcularSumaDeOctavasEnPunto(x, y);

    return perlinNoiseMap;
  }

  private float calcularSumaDeOctavasEnPunto(int x, int y)
  {
    float amplitud = 1;
    float frecuencia = 1;
    float noiseHeight = 0;
    float centroX = (x - ancho / 2f) / escala + offset.x;
    float centroY = (y - alto / 2f) / escala + offset.y;

    System.Random pseudoRandom = new System.Random(seed);

    for (int i = 0; i < octavas; i++)
    {
      //float xTent = centroX * frecuencia + pseudoRandom.Next(-_RANGO_RANDOM, _RANGO_RANDOM) + offset.x;
      //float yTent = centroY * frecuencia + pseudoRandom.Next(-_RANGO_RANDOM, _RANGO_RANDOM) + offset.y;

      float perlinVal = Mathf.PerlinNoise(centroX, centroY) * 2 - 1;

      noiseHeight += perlinVal * amplitud;

      //Cada octava tiene menos amplitud
      //(Menos influencia en el resultado)
      amplitud = amplitud * persistencia;

      //Cada octava tiene mas frecuencia
      frecuencia = frecuencia * lacunaridad;
    }

    actualizarMaximoYMinimo(noiseHeight);

    return noiseHeight;
  }

  private void actualizarMaximoYMinimo(float noiseHeight)
  {
    if (noiseHeight > maxNoiseHeight)
      maxNoiseHeight = noiseHeight;
    else if (noiseHeight < minNoiseHeight)
      minNoiseHeight = noiseHeight;
  }

  private static void normalizarMapa(float[,] map, float min, float max)
  {
    for (int x = 0; x < map.GetLength(0); x++)
      for (int y = 0; y < map.GetLength(1); y++)
        map[x, y] = Mathf.InverseLerp(max, min, map[x, y]);
  }
}