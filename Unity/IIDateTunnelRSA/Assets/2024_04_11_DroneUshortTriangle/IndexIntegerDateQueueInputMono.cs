using System.Collections.Generic;
using UnityEngine;

public class IndexIntegerDateQueueInputMono :MonoBehaviour{

    public IndexIntegerDateStruct m_last;
    public IndexIntegerDateStruct [] m_lasts= new IndexIntegerDateStruct[10];
    public Queue<IndexIntegerDateStruct> m_queue = new Queue<IndexIntegerDateStruct>();
    public NativeArrayMono_ArrayIndexToIndexInteger16K m_indexToIndexInteger16K;
    public void Enqueue(IndexIntegerDateStruct item)
    {
        m_queue.Enqueue(item);
    }

    public void Dequeue(out IndexIntegerDateStruct item)
    {
        item = m_queue.Dequeue();
    }
    public void Update()
    {
        if (m_queue.Count > 0)
        {
           
            //move in array to next position
            for (int i = m_lasts.Length - 1; i > 0; i--)
            {
                m_lasts[i] = m_lasts[i - 1];
            }


            Dequeue(out m_last);
            m_lasts[0] = m_last;


            m_indexToIndexInteger16K.GetFromIntegerIndex( m_last.index,
                out bool found, out int index);
            if(found)
            {
                m_indexToIndexInteger16K.SetIndex(index, m_last.value);
            }
            else
            {
                m_indexToIndexInteger16K.GetNextFree(out int indexFree);
                if (indexFree >= 0) { 
                    m_indexToIndexInteger16K.SetIndex(indexFree, m_last.value);
                }
            }
         
        }
    }
}
