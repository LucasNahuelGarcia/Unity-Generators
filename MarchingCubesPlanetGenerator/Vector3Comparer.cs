using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Vector3Comparer : IComparer<Vector3>
{
  private float treshold;
  public Vector3Comparer(float treshold)
  {
    this.treshold = treshold;
  }
  public int Compare(Vector3 x, Vector3 y)
  {
    int res = 0;
    float diferencia = (x - y).magnitude;

    if (diferencia > treshold)
      res = 1;
    else if (diferencia < -treshold)
      res = -1;

    return res;
  }
}