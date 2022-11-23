using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Generador.LandGenerator
{
    [CustomEditor(typeof(LandGenerator))]
    public class MapGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            LandGenerator mapGen = (LandGenerator)target;

            if (DrawDefaultInspector())
                if (mapGen.autoUpdate)
                    mapGen.Generar();
            if (GUILayout.Button("Generar"))
                mapGen.Generar();

        }
    }
}