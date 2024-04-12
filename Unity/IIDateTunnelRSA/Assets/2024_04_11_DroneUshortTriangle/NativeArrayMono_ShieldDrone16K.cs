using Unity.Collections;
using UnityEngine;

public class NativeArrayMono_ShieldDrone16K : NativeArrayMono_Generic16K<ShieldDroneAsUShort> 
{

   
}

[System.Serializable]
public struct ShieldDroneAsUShort
{
    public byte m_quadrantByte;
    public ushort m_quadrantX;
    public ushort m_quadranty;
    public ushort m_quadrantz;
    public ushort m_angleLR180;
    public ushort m_percentShield;
}


