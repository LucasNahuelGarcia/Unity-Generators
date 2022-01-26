using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RockGeneratorVoronoi))]
public class AsteroideGenEditor : Editor
{
    private Editor shapeSettingsEditor;
    private RockGeneratorVoronoi astGen;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Generar"))
            astGen.Generar();
        astGen = (RockGeneratorVoronoi)target;
        DrawSettingsEditor(astGen.shapeSettings, SettingsUpdated, true, ref shapeSettingsEditor);
    }

    private void SettingsUpdated() {
        if ((DrawDefaultInspector() && astGen.updateOnChange))
            astGen.Generar();
    }
    
    private void DrawSettingsEditor(Object settings, System.Action onSettingsUpdated, bool fold, ref Editor editor)
    {
        // fold = EditorGUILayout.InspectorTitlebar(fold, settings);
        using (var check = new EditorGUI.ChangeCheckScope())
        {

            // if (fold)
            // {
                CreateCachedEditor(settings, null, ref editor);
                editor.OnInspectorGUI();

                if (check.changed && onSettingsUpdated != null)
                {
                    onSettingsUpdated();
                }
            // }
        }
    }
}
