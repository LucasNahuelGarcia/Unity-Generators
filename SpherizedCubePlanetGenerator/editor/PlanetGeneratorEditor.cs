using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlanetGenerator))]
public class PlanetGeneratorEditor : Editor
{
    Editor shapeEditor;
    Editor colorEditor;
    public override void OnInspectorGUI()
    {
        PlanetGenerator astGen = (PlanetGenerator)target;

        if (GUILayout.Button("Generar") || DrawDefaultInspector() && astGen.autoUpdate)
            astGen.Generar();

        drawSettingsEditor(astGen.shapeSettings, astGen.onShapeSettingsUpdate, true, ref shapeEditor);
        drawSettingsEditor(astGen.colorSettings, astGen.onColorSettingsUpdate, true, ref colorEditor);
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
