using CloudTunnelingRSA.Beans;
using System.Security.Cryptography;
using System.Text;



public class RsaConnectionHandShake
    {
        public RsaPublicKeyRef m_publicRsaKeyReceived;
        public string m_messageSentToClient = Guid.NewGuid().ToString();
        public string m_signMessageReceived = "";
        public bool m_isValid = false;

        public RsaConnectionHandShake(RsaPublicKeyRef key)
        {
            m_publicRsaKeyReceived = key;
            m_messageSentToClient = Guid.NewGuid().ToString();
            m_signMessageReceived = "";
            m_isValid = false;
        }
        public bool IsRsaPublicKeyGiven()
        {
            return m_publicRsaKeyReceived.m_publicKey.Length > 0;
        }
        public bool IsSignedReceived()
        {
            return m_messageSentToClient.Length > 0;
        }
        public bool WasReceivedValide()
        {
            return m_isValid;
        }
        public void SetValide(bool isValide)
        {
            m_isValid = isValide;
        }

        public bool ComputeCurrentValidityThenReturnIsValide()
        {
            m_isValid= CryptoTools.VerifySignature(m_messageSentToClient, m_signMessageReceived, m_publicRsaKeyReceived);
            return m_isValid;
        }
    }
