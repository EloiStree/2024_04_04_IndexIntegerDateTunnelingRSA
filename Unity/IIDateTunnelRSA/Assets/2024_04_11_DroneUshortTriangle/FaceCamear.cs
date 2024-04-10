using UnityEngine;

public class FaceCamear : MonoBehaviour
{

    void Update()
    {
        
        transform.LookAt(Camera.main.transform);
    }
}
