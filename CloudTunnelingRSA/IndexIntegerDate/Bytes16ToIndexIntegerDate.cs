using CloudTunnelingRSA.Beans;
using CloudTunnelingRSA.Dico;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WebSocketServer;

namespace CloudTunnelingRSA.IndexIntegerDate
{
    public class Bytes16ToIndexIntegerDate
    {

        public void PushBytes(byte[] bytes)
        {
            if (bytes[15] == 0 && bytes[14] == 0 && bytes[13] == 0 && bytes[12] == 0
                && bytes[11] == 0 && bytes[10] == 0 && bytes[9] == 0 && bytes[8] == 0)
            {
                int index = BitConverter.ToInt32(bytes, 0);
                int value = BitConverter.ToInt32(bytes, 4);
            }
            if (bytes.Length >= 16)
            {
                int index = BitConverter.ToInt32(bytes, 0);
                int value = BitConverter.ToInt32(bytes, 4);
                ulong timeStampUtc = BitConverter.ToUInt64(bytes, 8);
            }
        }

    }



    public class RsaFilterDicoIndexIntegerDate
    {

        public static bool CheckIfPushable(in byte[] target, in int indexLock)
        {

            int index = BitConverter.ToInt32(target, 0);
            if(index!=indexLock)
                return false;

            int value = BitConverter.ToInt32(target, 4);
            ulong timeStampUtc = BitConverter.ToUInt64(target, 8);

            DicoIndexIntegerDate.Instance.Set(
                index, value, timeStampUtc, out bool changeDetected);
            if (changeDetected) {
                ServerConsole.WriteLine($"IID {index}  {value}  {timeStampUtc} ");
                return true;
            }
            return false;
        }

        public static bool CheckIfPushable(in byte[] target, RsaPublicKeyRef key)
        {
            if (key == null)
                return false;

            int index= BitConverter.ToInt32(target, 0);
            if (DicoIntegerIndexToStringPublicKey.Instance.ContainsKey(index)) { 
                RsaPublicKeyRef inRegister = DicoIntegerIndexToStringPublicKey.Instance.Get(index);
                
                if (inRegister == null) { 
                    return false;
                }

                if (AreEquals(in key.m_publicKey, in inRegister.m_publicKey)) {
                    int value= BitConverter.ToInt32(target, 4);
                    ulong timeStampUtc= BitConverter.ToUInt64(target, 8);
                    DicoIndexIntegerDate.Instance.Set(index,
                        value,
                        timeStampUtc,
                        out bool changeDetected);
                    if (changeDetected) { 
                        ServerConsole.WriteLine($"IID {index}  {value}  {timeStampUtc} ");
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool AreEquals(in string a, in string b)
        {
            if(a.Length!=b.Length)
                return false;
            for (int i = 0; i < a.Length; i++) {
                if (a[i] != b[i]) return false;
            }
            return true;
        }
    }


    public class DicoIndexIntegerDate
    {

        public static DicoIndexIntegerDate Instance = new DicoIndexIntegerDate();
        public Dictionary<int, IndexIntegerDate> m_dicoIndexIntegerDate = new Dictionary<int, IndexIntegerDate>();
        public void Set(int index, int value, out bool changeDetected)
        { 
            Set( index, value, (ulong)(DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond), out changeDetected);
        }
        public void Set(int index, int value, ulong timeStampUtc, out bool changeDetected)
        {
            changeDetected = false;
            if (m_dicoIndexIntegerDate.ContainsKey(index))
            {
                ServerConsole.WriteLine($"???? {m_dicoIndexIntegerDate[index].value}  {value}");

                if (m_dicoIndexIntegerDate[index].value != value)
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

            ServerConsole.WriteLine($"Attempt to change {changeDetected}: {index}  {value}  {timeStampUtc} ");
        }

    }


    public class IndexIntegerDate
    {

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
            timeStampUtc = (ulong)DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
        }
    }



}
