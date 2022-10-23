using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generador.LandGenerator
{
    public interface noiseMap
    {
        void configNoiseMap(NoiseMapConfig config);
        float[,] GenerateNoiseMap(List<PerlinOctave> octaves);

    }
}