using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudTunnelingRSA
{
    public class Bytes16ToIndexIntegerDate
    {

        public void PushBytes(byte[] bytes) {
            if (bytes[15] == 0 && bytes[14] == 0 && bytes[13] == 0 && bytes[12] == 0
                && bytes[11] == 0 && bytes[10] == 0 && bytes[9] == 0 && bytes[8] == 0 )
            {
                int index = BitConverter.ToInt32(bytes, 0);
                int value = BitConverter.ToInt32(bytes, 4);
            }
            if(bytes.Length >= 16)
            {
                int index = BitConverter.ToInt32(bytes, 0);
                int value = BitConverter.ToInt32(bytes, 4);
                ulong timeStampUtc = BitConverter.ToUInt64(bytes, 8);
            }
        }

    }



    public class RsaFilterDicoIndexIntegerDate { 
    
    
    }


    public class DicoIndexIntegerDate { 
    
        public static DicoIndexIntegerDate Instance = new DicoIndexIntegerDate();
        public Dictionary<int, IndexIntegerDate> m_dicoIndexIntegerDate = new Dictionary<int, IndexIntegerDate>();
        
        public void Set(int index, int value, ulong timeStampUtc, out bool changeDetected)
        {
            changeDetected = false;
            if (m_dicoIndexIntegerDate.ContainsKey(index))
            {
                if (m_dicoIndexIntegerDate[index].timeStampUtc > timeStampUtc ||
                    m_dicoIndexIntegerDate[index].value != value)
                {
                    m_dicoIndexIntegerDate[index].Set(value, timeStampUtc);
                    changeDetected = true;
                }
            }
            else
            {
                m_dicoIndexIntegerDate.Add(index, new IndexIntegerDate(index, value, timeStampUtc));
                changeDetected = true;
            }
        }
    
    }


    public struct IndexIntegerDate { 
    
        public int index;
        public int value;
        public ulong timeStampUtc;

        public IndexIntegerDate(int index, int value, ulong timeStampUtc)
        {
            this.index = index;
            this.value = value;
            this.timeStampUtc = timeStampUtc;
        }
        public void Set(int value, ulong timeStampUtc)
        {
            this.value = value;
            this.timeStampUtc = timeStampUtc;
        }
        public void Set(int value)
        {
            this.value = value;
            this.timeStampUtc = (ulong)DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
        }
    }



}
