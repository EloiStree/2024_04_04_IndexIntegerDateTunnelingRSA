using UnityEngine;

public class NativeArrayMono_DemoSetGetShieldDrone16K : MonoBehaviour
{
    public const ushort m_quadrantSize = ushort.MaxValue;
    public const ushort m_quadrantSizeAsMM = ushort.MaxValue/1000;
    public NativeArrayMono_ShieldDrone16K m_nativeArray_ShieldDrone16K;
    public bool m_useOnQuadrant=true;
    public void SetPositionFromPercent(int index, float x, float y, float z) {

        if(index>=128*128)
            throw new System.Exception("index out of range");

        if (m_useOnQuadrant == false)
            throw new System.NotImplementedException();

        m_nativeArray_ShieldDrone16K.Get(index, out ShieldDroneAsUShort drone);
        drone.m_quadrantX = (ushort)(m_quadrantSizeAsMM * x);
        drone.m_quadranty = (ushort)(m_quadrantSizeAsMM * y);
        drone.m_quadrantz = (ushort)(m_quadrantSizeAsMM * z);
        drone.m_quadrantX = (ushort)(m_quadrantSizeAsMM * x);

    }

}


