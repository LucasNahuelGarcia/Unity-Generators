using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generador.LandGenerator
{
    [System.Serializable]
    public struct PerlinOctave
    {
        public float heightMapZoom;
        [Range(0.0001f, 1.0f)]
        public float weight;
        public float generalHeightMultiplier;
        public AnimationCurve curveHeightMultiplier;
        public Vector2 offset;
    }
}