
using System;
using System.IO;
using System.Security.Cryptography;
using System.Xml;
using UnityEngine;
using UnityEngine.Events;

public class GenerateAndStoreKeyPairInUnityMono : MonoBehaviour
{


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
        RSA rsa = RSA.Create();
        rsa.KeySize = 1024;
        m_privateXmlKey = rsa.ToXmlString(true);
        m_publicXmlKey = rsa.ToXmlString(false);

        string path = Path.Combine(Application.persistentDataPath, "KeyPair");
        string pathPublic = Path.Combine(path, "RSA_PUBLIC_XML.txt");
        string pathPrivate = Path.Combine(path, "RSA_PRIVATE_XML.txt");
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            System.IO.File.WriteAllText(pathPublic, m_publicXmlKey);
            System.IO.File.WriteAllText(pathPrivate, m_privateXmlKey);
            Debug.Log("Public and private keys saved in:\n " + path);
        }
       

        m_publicXmlKey = System.IO.File.ReadAllText(pathPublic );
        m_privateXmlKey= System.IO.File.ReadAllText(pathPrivate );
        m_onPublicXmlLoaded.Invoke(m_publicXmlKey);
        m_onPrivateXmlLoaded.Invoke(m_privateXmlKey);
        Debug.Log("Public and private keys Stored in:\n " + path);
        m_onKeyPairLoaded.Invoke();



    }


}

