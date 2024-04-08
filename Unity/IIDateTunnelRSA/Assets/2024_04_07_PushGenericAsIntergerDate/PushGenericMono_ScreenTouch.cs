using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PushGenericMono_ScreenTouch  : MonoBehaviour
{

    public UnityEvent<int> m_onIntegerPushed;

    public int m_startValue = 1900000500;
    public int m_lastValuePushed;

    public int m_previousCount;
    public int m_currentCount;

    private void Update()
    {
        m_previousCount = m_currentCount;
        m_currentCount= Input.touchCount;

        if(m_currentCount!= m_previousCount)
        {
            if (m_currentCount > m_previousCount) { 
                for (int i = m_previousCount; i <= m_currentCount; i++)
                {
                    PushButtonIndex1DAsPress(i);
                }
            }
            if (m_currentCount < m_previousCount) { 
                for (int i = m_currentCount; i <= m_previousCount; i++)
                {
                    PushButtonIndex1DAsRelease(i);  
                }
            }
            
        }
    }
    public void PushButtonIndex1DAsPress(int index)
    {
        PushButtonIndex1DAs(index, true);
    }
    public void PushButtonIndex1DAsRelease(int index)
    {
        PushButtonIndex1DAs(index, false);
    }
    public void PushButtonIndex1DAs(int index, bool isPress)
    {
       
        int value = (m_startValue + (Mathf.Clamp(index, 0, 20) ));
        if (isPress)
            value += 1000;
        value *= -1;
        m_lastValuePushed = value;
        m_onIntegerPushed.Invoke(value);
    }

}
