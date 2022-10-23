using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Generador.LandGenerator
{
    [CustomEditor(typeof(MapGenerator))]
    public class MapGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            MapGenerator mapGen = (MapGenerator)target;

            if (DrawDefaultInspector())
                if (mapGen.autoUpdate)
                    mapGen.Generate();
            if (GUILayout.Button("Generar"))
                mapGen.Generate();

        }
    }
}