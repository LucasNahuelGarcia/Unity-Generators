using System.Collections.Generic;
using UnityEngine;

namespace Generador
{
    public class GPUInstancingBatches
    {
        public Vector3 BatchCenter;
        public List<List<Matrix4x4>> Batches
        {
            get
            {
                return batches;
            }
        }
        private List<List<Matrix4x4>> batches;
        private const int _BatchesLimit = 1000;
        private List<Matrix4x4> currentMatrix;

        public GPUInstancingBatches()
        {
            batches = new List<List<Matrix4x4>>();
            currentMatrix = new List<Matrix4x4>();
            batches.Add(currentMatrix);
        }

        public void AddMatrix(Matrix4x4 matrix)
        {
            currentMatrix.Add(matrix);
            if (currentMatrix.Count >= _BatchesLimit)
            {
                    currentMatrix = new List<Matrix4x4>();
                    batches.Add(currentMatrix);
            }
        }
    }
}