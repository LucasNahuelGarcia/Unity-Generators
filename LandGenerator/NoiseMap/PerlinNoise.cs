using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generador.LandGenerator
{
    public class PerlinNoise : noiseMap
    {
        private static int _RANGO_RANDOM = 100000;
        private float maxNoiseHeight, minNoiseHeight;
        private int ancho, alto;
        private int seed;
        private Vector2 offset;
        public void configNoiseMap(NoiseMapConfig config)
        {
            ancho = config.ancho;
            alto = config.alto;
            seed = config.seed;
            offset = config.offset;
        }

        public float[,] GenerateNoiseMap(List<PerlinOctave> octaves)
        {
            minNoiseHeight = float.MaxValue;
            maxNoiseHeight = float.MinValue;

            float[,] perlinNoiseMap = calcularMapa(octaves);
            normailizeMap(perlinNoiseMap, maxNoiseHeight, minNoiseHeight);

            return perlinNoiseMap;
        }
        public List<float[,]> GenerateNoiseMaps(NoiseMapConfig[] config)
        {
            return new List<float[,]>();
        }

        private float[,] calcularMapa(List<PerlinOctave> octaves)
        {
            float[,] perlinNoiseMap = new float[ancho, alto];

            for (int x = 0; x < ancho; x++)
                for (int y = 0; y < alto; y++)
                    perlinNoiseMap[x, y] = calculatePointHeight(x, y, octaves);

            return perlinNoiseMap;
        }

        private float calculatePointHeight(int x, int y, List<PerlinOctave> octaves)
        {
            float noiseHeight = 0;
            float centroX = (x - ancho / 2f);
            float centroY = (y - alto / 2f);

            System.Random pseudoRandom = new System.Random(seed);

            foreach (PerlinOctave octave in octaves)
            {
                float octaveX = (centroX + offset.x + octave.offset.x) / octave.heightMapZoom;
                float octaveY = (centroY + offset.y + octave.offset.y) / octave.heightMapZoom;
                float octaveHeightAtPoint = (Mathf.PerlinNoise(octaveX, octaveY) * 2 - 1);

                octaveHeightAtPoint = octave.curveHeightMultiplier.Evaluate(octaveHeightAtPoint);
                octaveHeightAtPoint *= octave.weight;

                noiseHeight += octaveHeightAtPoint;
                Debug.Log(octave.weight);
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

        private static void normailizeMap(float[,] map, float min, float max)
        {
            for (int x = 0; x < map.GetLength(0); x++)
                for (int y = 0; y < map.GetLength(1); y++)
                    map[x, y] = Mathf.InverseLerp(max, min, map[x, y]);
        }
    }
}