using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AsteroideGen))]
public class AsteroideGenEditor : Editor
{
  private Editor shapeSettingsEditor;
  public override void OnInspectorGUI()
  {
    AsteroideGen astGen = (AsteroideGen)target;
    if (GUILayout.Button("Generar") || DrawDefaultInspector() && astGen.autoUpdate)
      astGen.build();

    drawSettingsEditor(astGen.shapeSettings, astGen.build, true, ref shapeSettingsEditor);
  }
  private void drawSettingsEditor(Object settings, System.Action onSettingsUpdated, bool fold, ref Editor editor)
  {
    fold = EditorGUILayout.InspectorTitlebar(fold, settings);
    using (var check = new EditorGUI.ChangeCheckScope())
    {

      if (fold)
      {
        CreateCachedEditor(settings, null, ref editor);
        editor.OnInspectorGUI();

        if (check.changed && onSettingsUpdated != null)
        {
          onSettingsUpdated();
        }
      }
    }
  }
}