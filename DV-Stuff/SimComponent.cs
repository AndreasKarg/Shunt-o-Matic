// Decompiled with JetBrains decompiler
// Type: SimComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9379D573-50CB-4E58-8F83-33F4D448217F
// Assembly location: E:\SteamLibrary\steamapps\common\Derail Valley\DerailValley_Data\Managed\Assembly-CSharp.dll

using Newtonsoft.Json.Linq;
using UnityEngine;

public class SimComponent
{
  public string name;
  public float value;
  public float nextValue;
  public float min;
  public float max;
  public float valueStep;

  public SimComponent(string name, float min = 0.0f, float max = 1f, float valueStep = 1f, float value = 0.0f)
  {
    this.name = name;
    this.value = value;
    this.min = min;
    this.max = max;
    this.valueStep = valueStep;
  }

  public void AddValue(float val)
  {
    this.value = Mathf.Clamp(this.value + val, this.min, this.max);
  }

  public void AddNextValue(float val)
  {
    this.nextValue = Mathf.Clamp(this.nextValue + val, this.min, this.max);
  }

  public void SetValue(float val)
  {
    this.value = Mathf.Clamp(val, this.min, this.max);
  }

  public void SetNextValue(float val)
  {
    this.nextValue = Mathf.Clamp(val, this.min, this.max);
  }

  public void PassValueTo(SimComponent destinationComp, float valueToPass)
  {
    if ((double) valueToPass > (double) this.value)
      valueToPass = this.value;
    float num = destinationComp.max - destinationComp.value;
    if ((double) valueToPass > (double) num)
      valueToPass = num;
    this.AddValue(-valueToPass);
    destinationComp.AddValue(valueToPass);
  }

  public void PassValueToNext(SimComponent destinationComp, float valueToPass)
  {
    if ((double) this.nextValue < (double) valueToPass)
      valueToPass = this.nextValue;
    float num = destinationComp.max - destinationComp.nextValue;
    if ((double) valueToPass > (double) num)
      valueToPass = num;
    this.AddNextValue(-valueToPass);
    destinationComp.AddNextValue(valueToPass);
  }
}
