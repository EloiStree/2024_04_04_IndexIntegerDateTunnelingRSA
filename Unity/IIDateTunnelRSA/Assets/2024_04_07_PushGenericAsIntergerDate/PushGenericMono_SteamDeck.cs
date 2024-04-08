using UnityEngine;
using UnityEngine.Events;

public class PushGenericMono_SteamDeck : MonoBehaviour
{

    public UnityEvent<int> m_onIntegerPushed;

    public int m_startValue = 1900000000;
    public int m_lastValuePushed;

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
        int value = ( m_startValue + (Mathf.Clamp(index, 0, 999) * 10000));
        if (isPress)
            value += 10000000;
        value *= -1;
        m_lastValuePushed = value;
        m_onIntegerPushed.Invoke(value);
    }

}
