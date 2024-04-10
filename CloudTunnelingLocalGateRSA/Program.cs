using System;
using System.Drawing;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

class Program
{
    public static string[] m_ports = new string[] { "5505", "5506" };
    static async Task Main(string[] args)
    {



        //Read a file name PrivateKey.txt and PublicKey.txt
        string path = Directory.GetCurrentDirectory();
        Console.WriteLine("Current Path:" + path);
        if (!File.Exists("KeyPair/RSA_PRIVATE_XML.txt"))
        {
            Directory.CreateDirectory("KeyPair");
            using (RSA rsa = RSA.Create())
            {
                rsa.KeySize = 1024;
                File.WriteAllText("KeyPair/RSA_PRIVATE_XML.txt", rsa.ToXmlString(true));
                File.WriteAllText("KeyPair/RSA_PUBLIC_XML.txt", rsa.ToXmlString(false));



                Console.WriteLine("Not Private key found. Random Generated for you in PrivateKey.txt.");
                Console.WriteLine("Never share your private key.");
            }
        }

        string privateKey = File.ReadAllText("KeyPair/RSA_PRIVATE_XML.txt");
        string publicKey = File.ReadAllText("KeyPair/RSA_PUBLIC_XML.txt");

        string fileTarget = "TargetServer/URITARGET.txt";
        if (!File.Exists(fileTarget))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fileTarget));
            File.WriteAllText(fileTarget, "ws://");
        }
        string fileTargetPorts = "Broadcasting/BROADCAST_PORT.txt";
        if (!File.Exists(fileTargetPorts))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fileTargetPorts));
            File.WriteAllText(fileTargetPorts, "5505\n5506");
        }

        m_ports = File.ReadAllLines(fileTargetPorts).Where(k => !string.IsNullOrEmpty(k) && int.TryParse(k, out _)).ToArray();
        foreach (var item in m_ports)
        {
            if (int.TryParse(item, out int port))
            {
                Console.WriteLine($"Broadcasting to port:{port}");
                WebSocketClientToServerRSA.m_broadcastPort.Add(port);
            }
        }
        string serverUri = File.ReadAllText(fileTarget);
        WebSocketClientToServerRSA.m_publicKey = publicKey;
        WebSocketClientToServerRSA.m_privateKey = privateKey;
        WebSocketClientToServerRSA.m_serverUri = serverUri;

        Console.WriteLine("PUBLIC KEY USE:");
        Console.WriteLine("---------------------");
        Console.WriteLine(publicKey);
        Console.WriteLine("---------------------");

        //example of sending data to local ports when broadcast all.

        Task.Run(async () =>
        {
            while (true)
            {
                await Task.Delay(30000);
                WebSocketClientToServerRSA.m_toSendToTheServerUTF8.Enqueue($"CLIENTTIME:{DateTime.UtcNow.ToString("yyyyMMddhhmmss")}");
            }
        });
        WebSocketClientToServerRSA client = new WebSocketClientToServerRSA();
        await client.ConnectAndRun();



        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}



public class PemToXmlConverter
{


    public void Generate1024RsaKey(out string privateXmlKey, out string publicXmlKey, out string privatePem, out string publicPem)
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