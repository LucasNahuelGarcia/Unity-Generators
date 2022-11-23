using UnityEngine;
using Unity;

namespace Generador.LandGenerator
{
    public interface Decorator
    {
        void Decorate(ComputeBuffer buffer);
    }
}