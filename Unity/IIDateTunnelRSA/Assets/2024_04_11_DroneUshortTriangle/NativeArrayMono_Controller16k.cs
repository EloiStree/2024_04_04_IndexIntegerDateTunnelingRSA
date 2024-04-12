using Unity.Collections;
using UnityEngine;

public class NativeArrayMono_Controller16k : NativeArrayMono_Generic16K<DroneController>
{
   
}


[System.Serializable]
public struct DroneController
{
    public float m_leftRightRotation;
    public float m_downUpMove;
    public float m_leftRightMove;
    public float m_backForwardMove;
}
