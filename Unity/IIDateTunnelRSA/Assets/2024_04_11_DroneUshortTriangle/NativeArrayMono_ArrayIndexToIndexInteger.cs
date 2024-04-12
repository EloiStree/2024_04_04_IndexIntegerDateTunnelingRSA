using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class NativeArrayMono_ArrayIndexToIndexInteger16K :  NativeArrayMono_Generic16K<IndexToIndexInteger>
{
    public Dictionary<int, int> m_dicoIntegerIndexToIndex = new Dictionary<int, int>();

    private new  void Awake()
    {

        m_nativeArray.Create();
        ResetValue();
    }



    public void GetNextFree(out int indexFree) { 
        for (int i = 0; i < 128 * 128; i++)
        {
            if (m_nativeArray.m_indexToIndexInteger[i].m_value == 0)
            { 
            indexFree = i;
                return;
            }    
        }
        indexFree = -1;
    }


    public void RemoveIndex(int index)
    {
        IndexToIndexInteger value = m_nativeArray.m_indexToIndexInteger[index];
        m_dicoIntegerIndexToIndex.Remove(value.m_value);
        value.m_value = 0;
        m_nativeArray.m_indexToIndexInteger[index] = value;
    }
    public void RemoveInteger(int integer)
    {
        if(m_dicoIntegerIndexToIndex.ContainsKey(integer) == false)
        {
            return;
        }

        int index = m_dicoIntegerIndexToIndex[integer];
        IndexToIndexInteger value = m_nativeArray.m_indexToIndexInteger[index];
        value.m_value = 0;
        m_nativeArray.m_indexToIndexInteger[index] = value;
        m_dicoIntegerIndexToIndex.Remove(integer);
    }

   

    public void ResetValue()
    {
        m_dicoIntegerIndexToIndex.Clear();
        for (int i = 0; i < 128 * 128; i++)
        {
            IndexToIndexInteger v= new IndexToIndexInteger();
            v.m_index = i;
            m_nativeArray.m_indexToIndexInteger[i] = v;
        }
    }

    public void SetIndex(int index, int newIntegerValue)
    {
        IndexToIndexInteger value = m_nativeArray.m_indexToIndexInteger[index];
        value.m_value = newIntegerValue;
        m_nativeArray.m_indexToIndexInteger[index] = value;

        if (m_dicoIntegerIndexToIndex.ContainsKey(newIntegerValue))
        {
            m_dicoIntegerIndexToIndex[newIntegerValue] = index;
        }
        else
        {
            m_dicoIntegerIndexToIndex.Add(newIntegerValue, index);
        }
    }


    public void GetFromIntegerIndex(int integerIndex, out bool found, out int index)
    {
        found = m_dicoIntegerIndexToIndex.TryGetValue(integerIndex, out index);
    }
    public void GetAtIndexTheIntegerIndex(int index, out IndexToIndexInteger value)
    {
        value = m_nativeArray.m_indexToIndexInteger[index];
    }
    public void GetAtIndexTheIntegerIndex(int index, out int integerIndex)
    {

        integerIndex = m_nativeArray.m_indexToIndexInteger[index].m_value;
    }


    public int m_maxIndexReach;
    public void ComputeGetMaxPlayerReach() { 
        GetMaxPlayerReach(out m_maxIndexReach);
    }
    
    public bool ContaintsInteger(in int integer)
    {
        return m_dicoIntegerIndexToIndex.ContainsKey(integer);
    }
    
    public void Update()
    {
        //Should I ?
        ComputeGetMaxPlayerReach();
    }
    

    public void GetMaxPlayerReach(out int maxIndexReach)
    {
        for (int i = (128 * 128) - 1; i >= 0; i--)
        {
            if (m_nativeArray.m_indexToIndexInteger[i].m_value != 0)
            {
                maxIndexReach = i;
                return;
            }
        
        }
        maxIndexReach = 0;
    }
}


public class NativeArrayMono_ArrayIndexToTimestampAfkDetection : NativeArrayMono_Generic16K<IndexToUlong>
{
  
}


[System.Serializable]
public struct IndexToIndexInteger
{
    public int m_index;
    public int m_value;
}
[System.Serializable]
public struct IndexToUlong
{
    public int m_index;
    public ulong m_value;
}

public class NativeArray_ArrayIndexToIndexInteger16K : NativeArrayMono_Generic16K<IndexToIndexInteger>
{
    
    
}





public class NativeArrayMono_Generic16K<T>:MonoBehaviour where T : struct
{
    public T m_sample;
    public NativeArray_Generic16K<T> m_nativeArray = new NativeArray_Generic16K<T>();

    public void Awake()
    {
        m_nativeArray.Create();
    }

    public void Get(int index, out T shieldDroneAsUShort)
    {
        m_nativeArray.Get(index, out shieldDroneAsUShort);
    }
    public void Set(int index, T shieldDroneAsUShort)
    {
        Set(index, shieldDroneAsUShort);
    }
    public void OnDestroy()
    {
        m_nativeArray.Destroy();
    }
}
public class NativeArray_Generic16K<T>   where T: struct 
{
    public NativeArray<T> m_indexToIndexInteger;

    public void Create()
    {
        m_indexToIndexInteger = new NativeArray<T>(128 * 128, Allocator.Persistent);
    }

    public void Get(int index, out T shieldDroneAsUShort)
    {
        shieldDroneAsUShort = m_indexToIndexInteger[index];
    }
    public void Set(int index, T shieldDroneAsUShort)
    {
        m_indexToIndexInteger[index] = shieldDroneAsUShort;
    }
    public void Destroy()
    {
        m_indexToIndexInteger.Dispose();
    }
}