using CloudTunnelingLocalGateRSA;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;
using System;
using System.Drawing;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

class Program
{
    static async Task Main(string[] args)
    {

        string m_password = "Pasword12345!";

        //Override password manually for developer.
        //m_password = "Ordit";

        string m_salt = "Salt12345";

     
        if(args.Length > 0)
        {
            m_password = args[0];
        }



        bool sendMesssageToDebug = true;
        if(sendMesssageToDebug)
        {
            Timer timerDebug = new Timer((e) =>
            {
                Console.WriteLine("Action every 5 seconds");

                try {
                    byte[] bytes = new byte[12];

                    int valueRandom = new Random().Next(int.MinValue, int.MaxValue);
                    ulong date = (ulong)DateTimeOffset.Now.ToUnixTimeSeconds();

                    BitConverter.GetBytes(valueRandom).CopyTo(bytes, 0);
                    BitConverter.GetBytes(date).CopyTo(bytes, 4);
                    WebSocketClientToServerRSA.SentToServerAsBinary(bytes);
                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
               
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
        }
        

        if(!File.Exists("Salt/SaltUseToEncrypt.txt")){
            Directory.CreateDirectory("Salt");
            File.WriteAllText("Salt/SaltUseToEncrypt.txt", Guid.NewGuid().ToString());
            m_salt = File.ReadAllText("Salt/SaltUseToEncrypt.txt");
        }

        RSA rsa = RSA.Create();
        rsa.KeySize = 1024;
        //Read a file name PrivateKey.txt and PublicKey.txt
        string path = Directory.GetCurrentDirectory();
        Console.WriteLine("Current Path:" + path);
        string privateXmlKey="", publicXmlKey = "", privatePem = "", publicPem = "", privateKeyEthereum = "", publicKeyEthereum = "", address = "";

      
        if (!File.Exists("_RSA_PRIVATE_XML_ENCRYPTED.txt") )
        {
            PemToXmlConverter.Generate1024RsaKey(out privateXmlKey, out _, out _, out _);
            CreateEncryptedKeyFromPrivateXmlRSA(m_password, m_salt, privateXmlKey);
        }
        if (!File.Exists("_ETH_PRIVATE_ENCRYPTED.txt"))
        {
            EthereumKeyGenerator.GenerateEthereumKey(out privateKeyEthereum, out _, out _);
            CreateEncryptedKeyFromPrivateEthereum(m_password, m_salt, privateKeyEthereum);
        }





        byte[] encryptBytesXmlRead = File.ReadAllBytes("_RSA_PRIVATE_XML_ENCRYPTED.txt");
        EncryptionWithPassword.DecryptText(encryptBytesXmlRead, m_password, m_salt, out privateXmlKey);

        byte[] encryptBytesEthereumRead = File.ReadAllBytes("_ETH_PRIVATE_ENCRYPTED.txt");
        EncryptionWithPassword.DecryptText(encryptBytesEthereumRead, m_password, m_salt, out privateKeyEthereum);

        PemToXmlConverter.Generate1024RsaKey(privateXmlKey, out rsa, out publicXmlKey, out privatePem, out publicPem);
        EthereumKeyGenerator.GenerateEthereumKey(privateKeyEthereum, out publicKeyEthereum, out address);


        bool exportPassword = false;
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i]=="export")
                exportPassword = true;
        }
        if (exportPassword)
        {
            Directory.CreateDirectory("KeyPair");
            File.WriteAllText("KeyPair/RSA_PRIVATE_XML.txt", privateXmlKey);
            File.WriteAllText("KeyPair/RSA_PRIVATE_PEM.txt", privatePem);
            File.WriteAllText("KeyPair/ETH_PRIVATE.txt", privateKeyEthereum);


            File.WriteAllText("KeyPair/RSA_PUBLIC_XML.txt", publicXmlKey);
            File.WriteAllText("KeyPair/RSA_PUBLIC_PEM.txt", publicPem);
            File.WriteAllText("KeyPair/ETH_PUBLIC.txt", publicKeyEthereum);
            File.WriteAllText("KeyPair/ETH_ADDRESS.txt", address);
        }




        string fileTarget = "TargetServer/URITARGET.txt";
        if (!File.Exists(fileTarget))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fileTarget));
            File.WriteAllText(fileTarget, "ws://");
        }


        string fileIpbroadcastBytes = "Broadcast/UDPBytes.txt";
        if (!File.Exists(fileIpbroadcastBytes))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fileIpbroadcastBytes));
            File.WriteAllText(fileIpbroadcastBytes, "3618\n192.168.1.1:3618\n127.0.0.1:3618");
        }
        string[] lines = File.ReadAllLines(fileIpbroadcastBytes);
        BroadcastCallbackAsUDP.Instance.TryToAddByLines(lines);
        foreach (BroadcastCallbackAsUDP.TargetIp target in BroadcastCallbackAsUDP.Instance.m_ipPortUDP) { 
        
            Console.WriteLine("Broadcasting to: " + target.m_ipEndPoint.Address + ":" + target.m_ipEndPoint.Port);
        }
       
        string serverUri = File.ReadAllText(fileTarget);
        WebSocketClientToServerRSA.m_publicKey = publicXmlKey;
        WebSocketClientToServerRSA.m_privateKey = privateXmlKey;
        WebSocketClientToServerRSA.m_serverUri = serverUri;

        Console.WriteLine("PUBLIC KEY XML:");
        Console.WriteLine("---------------------");
        Console.WriteLine(publicXmlKey);
        Console.WriteLine("---------------------");
        Console.WriteLine("PUBLIC KEY PEM:");
        Console.WriteLine("---------------------");
        Console.WriteLine(publicPem);
        Console.WriteLine("---------------------");
        Console.WriteLine("PUBLIC ETHEREUM:");
        Console.WriteLine("---------------------");
        Console.WriteLine("https://etherscan.io/address/"+address);
        Console.WriteLine(publicKeyEthereum);
        Console.WriteLine("---------------------");
        Console.WriteLine("\n\n\n\n");

        Task.Run(async () => await ListenUDP_Binary.ListenUDP(5010));
        Task.Run(async () => await ListenUDP_Text.ListenUDP(5011));
        Task.Run(async () => await ListenAsLocalWebsocket.Start("http://localhost:5012/"));
        Task.Run( BroadcastCallbackAsUDP.Instance.Start);
        

        WebSocketClientToServerRSA client = new WebSocketClientToServerRSA();
        await client.ConnectAndRun();



        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    private static void CreateEncryptedKeyFromPrivateEthereum(string m_password, string m_salt, string s)
    {
        EncryptionWithPassword.EncryptText(s, m_password, m_salt, out byte[] encryptBytesEthereum);
        File.WriteAllBytes("_ETH_PRIVATE_ENCRYPTED.txt", encryptBytesEthereum);
    }

    private static void CreateEncryptedKeyFromPrivateXmlRSA(string m_password, string m_salt, string privateXmlKey)
    {
        EncryptionWithPassword.EncryptText(privateXmlKey, m_password, m_salt, out byte[] encryptBytesXml);
        File.WriteAllBytes("_RSA_PRIVATE_XML_ENCRYPTED.txt", encryptBytesXml);
    }
}



public class EncryptionWithPassword {


    public static void EncryptText(string plainText, string password, string saltText, out byte[] encryptBytes)
    {
        byte[] salt = Encoding.UTF8.GetBytes(saltText); // Salt should be unique and random

        using (Aes aesAlg = Aes.Create())
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(passwordBytes, salt, 1000);

            aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
            aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(msEncrypt, aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(cryptoStream))
                    {
                        swEncrypt.Write(plainText);
                    }
                }
                encryptBytes = msEncrypt.ToArray();
            }
        }
    }

    public static void DecryptText(byte[] encryptedData, string password, string saltText, out string textDecrypted)
    {
        byte[] salt = Encoding.UTF8.GetBytes(saltText); // Same salt used during encryption

        using (Aes aesAlg = Aes.Create())
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(passwordBytes, salt,1000);

            aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
            aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);

            using (MemoryStream msDecrypt = new MemoryStream(encryptedData))
            {
                using (CryptoStream cryptoStream = new CryptoStream(msDecrypt, aesAlg.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(cryptoStream))
                    {
                        textDecrypted= srDecrypt.ReadToEnd();
                        
                    }
                }
            }
        }
    }
}

public class EthereumKeyGenerator {

    public static void GenerateEthereumKey(out string privateKeyEthereum, out string publicKeyEthereum, out string address)
    {

        var ecKey = EthECKey.GenerateKey();
        privateKeyEthereum = ecKey.GetPrivateKeyAsBytes().ToHex();
        publicKeyEthereum = ecKey.GetPubKey().ToHex();
        address = ecKey.GetPublicAddress();
    }
    public static void GenerateEthereumKey( string privateKeyEthereum, out string publicKeyEthereum, out string address)
    {

        var ecKey = new EthECKey(privateKeyEthereum);
        publicKeyEthereum = ecKey.GetPubKey().ToHex();
        address = ecKey.GetPublicAddress();
    }


}


public class PemToXmlConverter
{


    public static void Generate1024RsaKey(string privateKey, out RSA rsa, out string publicXmlKey, out string privatePEM, out string publicPEM)
    {
        rsa = RSA.Create();
        rsa.KeySize = 1024;
        rsa.FromXmlString(privateKey);
        publicXmlKey = rsa.ToXmlString(false);
        privatePEM = rsa.ExportRSAPrivateKeyPem();
        publicPEM = rsa.ExportRSAPublicKeyPem();


    }
    public static void Generate1024RsaKey(out string privateXmlKey, out string publicXmlKey, out string privatePem, out string publicPem)
    {
        using (RSA rsa = RSA.Create())
        {
            rsa.KeySize = 1024;
            privateXmlKey = rsa.ToXmlString(true);
            publicXmlKey = rsa.ToXmlString(false);
            privatePem = rsa.ExportRSAPrivateKeyPem();
            publicPem = rsa.ExportRSAPublicKeyPem();
        }
    }
    public static void GenerateRsaKey(RSA rsa ,out string privateXmlKey, out string publicXmlKey, out string privatePem, out string publicPem)
    {
            privateXmlKey = rsa.ToXmlString(true);
            publicXmlKey = rsa.ToXmlString(false);
            privatePem = rsa.ExportRSAPrivateKeyPem();
            publicPem = rsa.ExportRSAPublicKeyPem();
        
    }



    public static string ConvertPrivateKey(string pemPrivateKey)
    {
        RSA rsa = RSA.Create();
        rsa.ImportFromPem(pemPrivateKey);
        RSAParameters parameters = rsa.ExportParameters(true);

        XmlDocument xmlDoc = new XmlDocument();
        XmlElement root = xmlDoc.CreateElement("RSAKeyValue");
        xmlDoc.AppendChild(root);

        XmlElement modulus = xmlDoc.CreateElement("Modulus");
        modulus.InnerText = Convert.ToBase64String(parameters.Modulus);
        root.AppendChild(modulus);

        XmlElement exponent = xmlDoc.CreateElement("Exponent");
        exponent.InnerText = Convert.ToBase64String(parameters.Exponent);
        root.AppendChild(exponent);

        XmlElement p = xmlDoc.CreateElement("P");
        p.InnerText = Convert.ToBase64String(parameters.P);
        root.AppendChild(p);

        XmlElement q = xmlDoc.CreateElement("Q");
        q.InnerText = Convert.ToBase64String(parameters.Q);
        root.AppendChild(q);

        XmlElement dp = xmlDoc.CreateElement("DP");
        dp.InnerText = Convert.ToBase64String(parameters.DP);
        root.AppendChild(dp);

        XmlElement dq = xmlDoc.CreateElement("DQ");
        dq.InnerText = Convert.ToBase64String(parameters.DQ);
        root.AppendChild(dq);

        XmlElement inverseQ = xmlDoc.CreateElement("InverseQ");
        inverseQ.InnerText = Convert.ToBase64String(parameters.InverseQ);
        root.AppendChild(inverseQ);

        XmlElement d = xmlDoc.CreateElement("D");
        d.InnerText = Convert.ToBase64String(parameters.D);
        root.AppendChild(d);

        return xmlDoc.OuterXml;
    }

    public static string ConvertPublicKey(string pemPublicKey)
    {
        RSA rsa = RSA.Create();
        rsa.ImportFromPem(pemPublicKey);
        RSAParameters parameters = rsa.ExportParameters(false);

        XmlDocument xmlDoc = new XmlDocument();
        XmlElement root = xmlDoc.CreateElement("RSAKeyValue");
        xmlDoc.AppendChild(root);

        XmlElement modulus = xmlDoc.CreateElement("Modulus");
        modulus.InnerText = Convert.ToBase64String(parameters.Modulus);
        root.AppendChild(modulus);

        XmlElement exponent = xmlDoc.CreateElement("Exponent");
        exponent.InnerText = Convert.ToBase64String(parameters.Exponent);
        root.AppendChild(exponent);

        return xmlDoc.OuterXml;
    }

}