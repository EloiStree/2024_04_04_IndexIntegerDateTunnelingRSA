using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class Sleepy_TriangleMono : MonoBehaviour
{


    public NativeArray<DroneAsUShort> m_droneAsUShortArray;

    public GameObject m_prefab;
    public Transform m_parent;
     List<Transform> m_dronesTransform= new List<Transform>(); 

    public struct DroneAsUShort { 
    
        public ushort x;
        public ushort y;
        public ushort z;
        public ushort angleLR180;
    }
    void Start()
    {
        m_droneAsUShortArray = new NativeArray<DroneAsUShort>(128*128, Allocator.Persistent);
       
        

       

        for (int i = 0; i < m_droneAsUShortArray.Length; i++)
        {
            GameObject g = GameObject.Instantiate(m_prefab); 
            g.transform.parent = m_parent;
            m_dronesTransform.Add(g.transform);
            m_dronesTransform[i].localScale = new Vector3(m_height,m_height,m_height);
        }
    }
    private void OnDestroy()
    {
        m_droneAsUShortArray.Dispose();
    }

      public  float m_height = 0.05f;
    public bool m_useDebugDraw;
    void Update()
    {
        for (int i = 0; i < m_droneAsUShortArray.Length; i++)
        {
            //randomize m_droneAsUShortArray
            m_droneAsUShortArray[i] = new DroneAsUShort
            {
                x = (ushort)Random.Range(0, 128),
                y = (ushort)Random.Range(0, 128),
                z = (ushort)Random.Range(0, 128),
                angleLR180 = (ushort)Random.Range(0, 360)
            };

            Vector3 vector3 = new Vector3(m_droneAsUShortArray[i].x, m_droneAsUShortArray[i].y, m_droneAsUShortArray[i].z);
            if (m_useDebugDraw)
            {
                Debug.DrawLine(vector3,
                    new Vector3(m_droneAsUShortArray[i].x + m_height, m_droneAsUShortArray[i].y + m_height, m_droneAsUShortArray[i].z + m_height)
                    , Color.red);

            }

            if (i < m_dronesTransform.Count) {

                Quaternion rotation = Quaternion.Euler(0, m_droneAsUShortArray[i].angleLR180, 0);
                m_dronesTransform[i].position = vector3;
                m_dronesTransform[i].rotation = rotation;
            
            }
        }
        
    }

    //public void SetTriangleOfMeshBasedOnNativeArray() { 
    
    //    Mesh mesh = new Mesh();
    //    Vector3[] vertices = new Vector3[m_droneAsUShortArray.Length];
    //    int[] triangles = new int[m_droneAsUShortArray.Length];
    //    for (int i = 0; i < m_droneAsUShortArray.Length; i++)
    //    {
    //        vertices[i] = new Vector3(m_droneAsUShortArray[i].x, m_droneAsUShortArray[i].y, m_droneAsUShortArray[i].z);
    //        triangles[i] = i;
    //    }
    //    mesh.vertices = vertices;
    //    mesh.triangles = triangles;
    //    GetComponent<MeshFilter>().mesh = mesh;
    //}

}
