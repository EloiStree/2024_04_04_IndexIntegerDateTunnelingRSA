using CloudTunnelingRSA.Beans;
using System.Security.Cryptography;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class CryptoTools {

    public static bool VerifySignature(byte[] data, byte[] signature, RsaPublicKeyRef rsaRef)
    {
        using (RSA a = RSA.Create())
        {
            a.KeySize = 1024;
            a.FromXmlString(rsaRef.GetPublicKey());
            return a.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }
    }//Pkcs1 SHA256 KeySize 1024

    public static bool VerifySignature(string dataAsUTF8, string signatureAsB64, RsaPublicKeyRef rsaRef)
    {
        using (RSA a = RSA.Create())
        {
            a.KeySize = 1024;
            a.FromXmlString(rsaRef.GetPublicKey());
            byte[] bytes = Encoding.UTF8.GetBytes(dataAsUTF8);
            byte[] signature = Convert.FromBase64String(signatureAsB64);
            return a.VerifyData(bytes, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }
    }
}