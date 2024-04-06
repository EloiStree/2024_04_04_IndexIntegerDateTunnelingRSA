using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudTunnelingRSA.Beans;

namespace CloudTunnelingRSA
{



    public class RsaConnectionState
    {
        public RsaPublicKeyRef m_givenPublicKey=null;
        public int m_index = int.MinValue;
        public int m_indexValue = 0;
        public long m_timeStampUtc = 0;

    }
}
