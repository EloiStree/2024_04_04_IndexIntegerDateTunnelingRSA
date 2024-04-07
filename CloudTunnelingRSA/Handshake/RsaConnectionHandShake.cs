using CloudTunnelingRSA.Beans;
using System.Security.Cryptography;
using System.Text;



public class RsaConnectionHandShake
    {
        public RsaPublicKeyRef m_publicRsaKeyReceived;
        public string m_uniqueMessageSentToClient = Guid.NewGuid().ToString();
        public string m_signMessageReceived = "";
        public bool m_isValid = false;
        public DateTime m_lastActivity = DateTime.UtcNow;

    public RsaConnectionHandShake(RsaPublicKeyRef key)
        {
            m_publicRsaKeyReceived = key;
            m_uniqueMessageSentToClient = Guid.NewGuid().ToString();
            m_signMessageReceived = "";
            m_isValid = false;
        }
        public bool IsRsaPublicKeyGiven()
        {
            return m_publicRsaKeyReceived.m_publicKey.Length > 0;
        }
        public bool IsSignedReceived()
        {
            return m_uniqueMessageSentToClient.Length > 0;
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
            m_isValid= CryptoTools.VerifySignature(m_uniqueMessageSentToClient, m_signMessageReceived, m_publicRsaKeyReceived);
            return m_isValid;
        }

    public void UpdateLastActivity()
    {
        m_lastActivity = DateTime.UtcNow;
    }
    public void GetSecondsSinceLastActivity(out double seconds)
    {
        seconds = (DateTime.UtcNow - m_lastActivity).TotalSeconds;
    }

    public RsaPublicKeyRef GetPublicKey()
    {
        return m_publicRsaKeyReceived;
    }
}
