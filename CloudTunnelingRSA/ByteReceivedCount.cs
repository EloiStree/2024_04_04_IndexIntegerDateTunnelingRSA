using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudTunnelingRSA
{
    public class ByteReceivedCount
    {
        public static ByteReceivedCount Instance = new ByteReceivedCount();
        public ulong m_byteReceivedCount = 0;

        public ulong GetByteCount()
        {
            return m_byteReceivedCount;
        }
        public string GetByteAsMegaByte()
        {
            return (m_byteReceivedCount / 1024 / 1024).ToString();
        }
        public string GetByteAsGigaByte()
        {
            return (m_byteReceivedCount / 1024 / 1024 / 1024).ToString();
        }

        //add byte count in the context of multi threads
        public void AddByteCount(ulong byteCount)
        {
            lock (this)
            {
                m_byteReceivedCount += byteCount;
            }
        }
        public void AddByteCount(int byteCount)
        {
            lock (this)
            {
                m_byteReceivedCount += (ulong) byteCount;
            }
        }
    }
}
