using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UI2D_SteamDeckButton : MonoBehaviour
{
    
    public int m_buttonIndex;

    

    public bool IsMouseOverPanelPixel(Vector2 pixelPosition)
    {
        
        RectTransform panelRectTransform = GetComponent<RectTransform>();
        Vector2 mousePosition = pixelPosition;
        return RectTransformUtility.RectangleContainsScreenPoint(panelRectTransform, mousePosition);
    }

    public void SetButtonIndex(int index)
    {
        m_buttonIndex = index;
    }
}
