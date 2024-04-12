using Unity.Collections;
using UnityEngine;

public class NativeArrayMono_PercentPosition3D16K : NativeArrayMono_Generic16K<PercentPosition>
{
   
}


[System.Serializable]
public struct PercentPosition
{
    public float x;
    public float y;
    public float z;
    public float angle;
}
