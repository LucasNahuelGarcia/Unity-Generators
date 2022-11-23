using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ProceduralGrassGenerator))]
public class GrassGeneratorEditor : Editor
{
  public override void OnInspectorGUI()
  {
    ProceduralGrassGenerator pastoGen = (ProceduralGrassGenerator)target;

    DrawDefaultInspector();
    if (GUILayout.Button("Generar"))
      pastoGen.generateMesh();
  }
}