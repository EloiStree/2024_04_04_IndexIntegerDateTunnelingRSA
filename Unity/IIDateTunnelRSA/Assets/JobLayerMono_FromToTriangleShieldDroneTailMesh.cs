
//public class JobLayerMono_FromToTriangleTailMesh : MonoBehaviour
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using DroneIMMO;
using UnityEditor.Rendering;

public class JobLayerMono_FromToTriangleShieldDroneTailMesh : MonoBehaviour
{
    public Mesh m_meshObject;

    public SNAM16K_ShieldDroneAsUnityFloatValue m_previousPosition;
    public SNAM16K_ShieldDroneAsUnityFloatValue m_currentPosition;


    //[SerializeField]
    //private MeshRenderer m_renderer;
    [SerializeField]
    private SkinnedMeshRenderer m_filter;

    public NativeArray<Vector3> m_vertices;
    public NativeArray<int> m_trianglesOrder;

    public int m_verticesCount = 0;
    public int m_trianglesOrderCount = 0;
    public float m_triangleSize = 0.05f;
    public float m_minDistance = 0.1f;
    private void Awake()
    {
        m_vertices = new NativeArray<Vector3>(IMMO16K.ARRAY_MAX_SIZE * 3, Allocator.Persistent);
        m_trianglesOrder = new NativeArray<int>(IMMO16K.ARRAY_MAX_SIZE * 6, Allocator.Persistent);
        m_meshObject = new Mesh();
        m_filter.sharedMesh = m_meshObject;

    }

    private void SetTriangle()
    {
        for (int index = 0; index < IMMO16K.ARRAY_MAX_SIZE; index++)
        {
            int t1, t2, t3, t4, t5, t6;
            int i1, i2, i3;

            i1 = index * 3;
            i2 = index * 3 + 1;
            i3 = index * 3 + 2;

            t1 = index * 6 + 0;
            t2 = index * 6 + 1;
            t3 = index * 6 + 2;
            t4 = index * 6 + 3;
            t5 = index * 6 + 4;
            t6 = index * 6 + 5;
            m_trianglesOrder[t1] = i1;
            m_trianglesOrder[t2] = i2;
            m_trianglesOrder[t3] = i3;
            m_trianglesOrder[t4] = i1;
            m_trianglesOrder[t5] = i3;
            m_trianglesOrder[t6] = i2;
        }
        m_meshObject.SetTriangles(m_trianglesOrder.ToArray(), 0);

    }

    private void OnDestroy()
    {
        m_vertices.Dispose();
        m_trianglesOrder.Dispose();
    }
    private bool m_firstTime = true;
    private void Update()
    {
        Job_SetTriangleTailBetweenPreviousAndDrone job = new Job_SetTriangleTailBetweenPreviousAndDrone
        {
            m_currentPosition = m_currentPosition.GetNativeArray(),
            m_previousPosition = m_previousPosition.GetNativeArray(),
            m_triangleSize = m_triangleSize,
            m_vertices = m_vertices
        };
        job.Schedule(IMMO16K.ARRAY_MAX_SIZE, 64).Complete();

        m_meshObject.SetVertices(m_vertices);
        if (m_firstTime)
        {
            m_firstTime = false;

            SetTriangle();
        }
    }
    private void Reset()
    {

        m_verticesCount = IMMO16K.ARRAY_MAX_SIZE * 3;
        m_trianglesOrderCount = IMMO16K.ARRAY_MAX_SIZE * 6;
    }

}
[BurstCompile]
public struct Job_SetTriangleTailBetweenPreviousAndDrone : IJobParallelFor
{


    [ReadOnly]
    public NativeArray<STRUCT_ShieldDroneAsUnity> m_previousPosition;
    [ReadOnly]
    public NativeArray<STRUCT_ShieldDroneAsUnity> m_currentPosition;

    public float m_triangleSize;

    [NativeDisableParallelForRestriction]
    [WriteOnly]
    public NativeArray<Vector3> m_vertices;

    public Vector3 m_hidePosition;

    public void Execute(int index)
    {
        STRUCT_ShieldDroneAsUnity current = m_currentPosition[index];
        STRUCT_ShieldDroneAsUnity previous = m_previousPosition[index];

        Vector3 from = previous.m_position;
        Vector3 to = current.m_position;
        Quaternion direction = current.m_rotation;
        
        float distance = Vector3.Distance(from, to);
        if (distance < 0.01)
        {

            m_vertices[index * 3] = Vector3.forward * 0.001f; 
            m_vertices[index * 3 + 1] = Vector3.right * 0.001f; ;
            m_vertices[index * 3 + 2] = -Vector3.right * 0.001f; ;
        }
        else {

            Quaternion right = direction * Quaternion.Euler(0, 90, 0);
            Vector3 rightDir = (right * (Vector3.forward * m_triangleSize));

            m_vertices[index * 3] = from + (direction * (Vector3.forward * distance));
            m_vertices[index * 3 + 1] = from + rightDir;
            m_vertices[index * 3 + 2] = from + -rightDir;
        }



    }
}



