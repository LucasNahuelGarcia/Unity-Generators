using System.Collections.Generic;
using UnityEngine;

namespace Generador
{
    public class GPUInstancingBatches
    {
        public Vector3 BatchCenter;
        private int size;
        public List<List<Matrix4x4>> Batches
        {
            get
            {
                return batches;
            }
        }
        public int Size
        {
            get
            {
                return size;
            }
        }
        private List<List<Matrix4x4>> batches;
        private const int _BatchesLimit = 1000;
        private List<Matrix4x4> currentMatrix;

        public GPUInstancingBatches()
        {
            size = 0;
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
            size++;
        }

    }
}