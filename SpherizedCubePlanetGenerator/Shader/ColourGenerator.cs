using UnityEngine;
using System.IO;
using System.Collections;

public class ColourGenerator
{
  ColorSettings colorSettings;
  Texture2D texture;
  const int resolution = 15;

  public void updateSettings(ColorSettings colorSettings)
  {
    this.colorSettings = colorSettings;
    if (texture == null)
      texture = new Texture2D(resolution, 1);
  }

  public void updateElevation(MinMax elevacionMinMax)
  {
    colorSettings.material.SetVector("_elevationMinMax", new Vector4(elevacionMinMax.min, elevacionMinMax.max));
    Debug.Log("updated: " + elevacionMinMax.min + " , " + elevacionMinMax.max);
  }

  public void updateColours()
  {
    Color[] colours = new Color[resolution];
    for (int i = 0; i < resolution; i++)
    {
      colours[i] = colorSettings.color.Evaluate(i / (resolution - 1f));
    }
    Debug.Log("generando textura." + texture);
    texture.SetPixels(colours);

    string localPath = colorSettings.material.name + ".png";
    string path = Application.dataPath + "/Resources/" + localPath;

    Debug.Log("path: " + path);

    texture.Apply(false,false);
    File.WriteAllBytes(path, texture.EncodeToPNG());
    texture = Resources.Load<Texture2D>(colorSettings.material.name);
    Debug.Log("textura cargada == null ::  " + texture == null);
    Debug.Log("textura nomre ::  " + texture.name );

    colorSettings.material.SetTexture("_texture", texture);
  }
}