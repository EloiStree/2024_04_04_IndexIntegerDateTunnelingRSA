using System;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;

public class GenerateAndStoreDeviceKeyPairInUnityMono : MonoBehaviour
{
    public string m_secretPassphrase = "0123456789";
    public string deviceIdentifier;
    [TextArea(0, 10)]
    public string m_publicXmlKey;
    [TextArea(0, 10)]
    public string m_privateXmlKey;

    public UnityEvent<string> m_onPublicXmlLoaded;
    public UnityEvent<string> m_onPrivateXmlLoaded;
    public UnityEvent m_onKeyPairLoaded;

    void Start()
    {
        GeneratePrivatePublicRsaKey();
    }

    [ContextMenu("Generate Random Public Private RSA Key")]
    private void GeneratePrivatePublicRsaKey()
    {
        deviceIdentifier = SystemInfo.deviceUniqueIdentifier;

        RSAParameters privateKeyParams = GeneratePrivateKey(deviceIdentifier);
        RSAParameters publicKeyParams = GetPublicKeyFromPrivateKey(privateKeyParams);

        // Convert RSA parameters to XML strings
        m_privateXmlKey = ToXmlString(privateKeyParams);
        m_publicXmlKey = ToXmlString(publicKeyParams);
        m_onPublicXmlLoaded.Invoke(m_publicXmlKey);
        m_onPrivateXmlLoaded.Invoke(m_privateXmlKey);

        // Output keys
        Debug.Log("Private Key (XML): " + m_privateXmlKey);
        Debug.Log("Public Key (XML): " + m_publicXmlKey);
    }

    // Generate RSA private key parameters using device identifier as part of the salt
    private RSAParameters GeneratePrivateKey(string deviceIdentifier)
    {
        byte[] salt = System.Text.Encoding.UTF8.GetBytes(deviceIdentifier); // Use device identifier as part of the salt
        int iterations = 1000; // Number of iterations for key derivation

        using (Rfc2898DeriveBytes derivedBytes = new Rfc2898DeriveBytes(m_secretPassphrase, salt, iterations))
        {
            byte[] keyBytes = derivedBytes.GetBytes(1024 / 8); // Changed key size to 1024 bits
            RSAParameters privateKeyParams = new RSAParameters();
            privateKeyParams.Modulus = keyBytes;

            return privateKeyParams;
        }
    }

    private RSAParameters GetPublicKeyFromPrivateKey(RSAParameters privateKeyParams)
    {
        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
        {
            rsa.ImportParameters(privateKeyParams);

            RSAParameters publicKeyParams = new RSAParameters();
            publicKeyParams.Modulus = rsa.ExportParameters(false).Modulus;
            publicKeyParams.Exponent = rsa.ExportParameters(false).Exponent;

            return publicKeyParams;
        }
    }

    // Convert RSA parameters to XML string
    private string ToXmlString(RSAParameters rsaParams)
    {
        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
        {
            rsa.ImportParameters(rsaParams);
            return rsa.ToXmlString(false); // Do not include private key information
        }
    }
}
