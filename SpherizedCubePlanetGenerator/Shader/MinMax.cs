using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinMax
{
  public float min {get; private set;}
  public float max {get; private set;}

  public MinMax(){
    min = float.MaxValue;
    max = float.MinValue;
  }

  public void addValue(float val){
    if(val > max)
      max = val;
    if(val < min)
      min = val;
  }
}
