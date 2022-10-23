using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generador.LandGenerator
{
    public struct Land
    {
        public GameObject gameObject;
        public MeshCollider meshCollider;
        public MeshFilter meshFilter;
        public MeshRenderer meshRenderer;
        public Renderer textureRenderer;
    }
}