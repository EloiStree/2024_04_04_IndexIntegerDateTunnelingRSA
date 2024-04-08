using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PushGenericMono_UnityKeyboard : MonoBehaviour
{

    public UnityEvent<int> m_onIntegerPushed;

    public int m_startValue = 1900000000;
    public int m_lastValuePushed;


    public List<KeyCode> m_keyCodes = new List<KeyCode>();
    public Dictionary<KeyCode, bool> m_keyCodeToValue = new Dictionary<KeyCode, bool>();


    [ContextMenu("Add Joysticks")]
    public void RemoveJoysticks() {
      
        m_keyCodes = m_keyCodes.Where(x => (int)x < 330).ToList();
    }

    private void Reset()
    {
        AddAllKeyCodeList();

    }

    private void AddAllKeyCodeList()
    {
        m_keyCodes.Clear();
        foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
        {
            m_keyCodes.Add(key);
        }
    }

    private void Start()
    {
        foreach (var key in m_keyCodes)
        {
            m_keyCodeToValue[key] = false;
        }
    }

    private void Update()
    {
        foreach (var key in m_keyCodes)
        {
            bool currentPrevious = m_keyCodeToValue[key];
            bool currentDown = Input.GetKey(key);
            if (currentPrevious != currentDown)
            {
                m_keyCodeToValue[key] = currentDown;
                if (currentDown)
                    PushButtonIndex1DAsPress(key);
                else
                    PushButtonIndex1DAsRelease(key);
            }
        }
    }

    public void PushButtonIndex1DAsPress(KeyCode index)
    {
        PushButtonIndex1DAs(index, true);
    }
    public void PushButtonIndex1DAsRelease(KeyCode index)
    {
        PushButtonIndex1DAs(index, false);
    }
    public int m_test;
    public void PushButtonIndex1DAs(KeyCode index, bool isPress)
    {
        int indexValue = (int)(index);
        m_test = indexValue;
        int value = Mathf.Abs(m_startValue + (Mathf.Clamp(indexValue, 0, 600) ));
        if (isPress)
            value += 1000;
        value *= -1;
        m_lastValuePushed = value;
        m_onIntegerPushed.Invoke(value);
    }

}
