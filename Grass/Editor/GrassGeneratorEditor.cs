using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GrassGenerator))]
public class GrassGeneratorEditor : Editor
{
  public override void OnInspectorGUI()
  {
    GrassGenerator pastoGen = (GrassGenerator)target;

    DrawDefaultInspector();
    if (GUILayout.Button("Generar"))
      pastoGen.generateMesh();
  }
}