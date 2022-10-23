using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface noiseMap
{
    void configNoiseMap(NoiseMapConfig config);
    float[,] GenerateNoiseMap();
    float[,] GenerateNoiseMap(NoiseMapConfig config);
    List<float[,]> GenerateNoiseMaps(NoiseMapConfig[] config);

}