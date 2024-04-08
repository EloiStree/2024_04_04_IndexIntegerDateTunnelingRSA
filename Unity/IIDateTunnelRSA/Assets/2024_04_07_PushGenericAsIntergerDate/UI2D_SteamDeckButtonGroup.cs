using UnityEngine;

public class UI2D_SteamDeckButtonGroup : MonoBehaviour
{

    public PushGenericMono_SteamDeck m_steamDeckPusher;
    public UI2D_SteamDeckButton[] m_buttons;
    public bool[] m_buttonsIsOn;
    void Awake()
    {
        if (m_steamDeckPusher == null)
            return;

        FetchButtonInChildrens();
    }

    [ContextMenu("Fretch button in childrens")]
    private void FetchButtonInChildrens()
    {
        m_buttons = transform.GetComponentsInChildren<UI2D_SteamDeckButton>();
        m_buttonsIsOn = new bool[m_buttons.Length];
        for (int i = 0; i < m_buttons.Length; i++)
        {
            m_buttons[i].SetButtonIndex(i);
        }
    }

    private void Update()
    {
        if (m_steamDeckPusher == null)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            for (int i = 0; i < m_buttons.Length; i++)
            {
               bool isPressed= m_buttons[i].IsMouseOverPanelPixel(Input.mousePosition);
                if (m_buttonsIsOn[i]!=isPressed)
                {
                    m_buttonsIsOn[i]=true;
                   m_steamDeckPusher.PushButtonIndex1DAsPress(i);
               }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            for (int i = 0; i < m_buttons.Length; i++)
            {
                if (m_buttonsIsOn[i])
                {
                    m_buttonsIsOn[i]=false;
                    m_steamDeckPusher.PushButtonIndex1DAsRelease(i);
                }
            }
        }
    }
}
