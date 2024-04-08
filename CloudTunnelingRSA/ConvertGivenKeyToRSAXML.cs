using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudTunnelingRSA
{
    public class ConvertGivenKeyToRSAXML
    {
        public static void TryParse(string publicKey, out bool foundAndConvert, out string resultXmlKey)
        {
            foundAndConvert = false;
            if (publicKey.Contains("BEGIN") &&
                publicKey.Contains("PUBLIC") &&
                publicKey.Contains("KEY"))
            {
                Console.WriteLine("PUBLIC RSA KEY 1024 PEM");
                resultXmlKey = PemToXmlConverter.ConvertPublicKey(publicKey);

                foundAndConvert = true;
            }
            else if (publicKey.Contains("<RSAKeyValue>"))
            {

                Console.WriteLine("PUBLIC RSA KEY 1024 XML");
                resultXmlKey = publicKey;

                foundAndConvert = true;
            }
            else { 
            
                Console.WriteLine("PUBLIC RSA KEY 1024 NOT FOUND");
                resultXmlKey = "";
            }

            /***
             * 
             * PUBLIC RSA KEY 1024
            
            **https://devglan.com/online-tools/rsa-encryption-decryption
            * 515 bit
            MFwwDQYJKoZIhvcNAQEBBQADSwAwSAJBAJUS3kuUaBGbCt/gv+Pb4RcIGG2wbDMF2r5/3AOuBPap98lzpNgMhSQ76aglk5vhxYU2UBqzi8T6JRQIAeRGAA0CAwEAAQ==

            * 1024
    MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCT7QBirnqMjhSQjETyQmupObO4NU4BwEQdiETSaeNF3Lg4G+32EdOQJkPjm28KadW2WOzQN4v34LPKGI6WE7YIcO07GjynFZGOgMqZTI5vI1hEhwZAOJCxJ5xHhHyyGFAz5ZWq+j7/fuh4qkzH6hJn3tQ51ncKgmmtQChSz1mLgQIDAQAB

            * 2048
            MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAxy0YqO3EUMYhjNbPhDuvV+9bfidnmBiAdbgBFO7LD19hxXA/bAQjG2n2OL/NhSZKaR3xPIbRMVU4LnC2FZjxg9wlryEyTs0hPNCNuDazIud5+9TK1pEoCxpxCOlTpKs7BXmyHyeHprO53uHnpDY+1QrlU5Akuvhs4QMW123ClrtJVtOd9Q1btny7+AiW5TRcBcl0OlVV/ck76TgUnONwFAqvlEi54wyrjFuRJWG8eKEZ0siHs1Y/3WBsv/Ov1rvfakRbjnsKdB64H0B3WTZQOtPFumnQVw2OKIxBK9Je+Oc2IdxZhRR25/cwP1wPwxrGtqSGXsFf3Ygg7Lysgo/uvwIDAQAB


    * 1024
-----BEGIN PUBLIC KEY-----
MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQC9B3RVRDebYMNENvTDX4aBU7et
V6gHdA+/QEpb3b0dR9b75pqwVKHJVh3wwumO2e+1RJ8fRutR8yh111Uuv/syjiIT
R6p1Do80aWXPL6W0BcU6/AtosLq5/cWiaMA0ND18s5DnoZ69bdg/JRqnZH3Trk2o
iegucz4oP+dwgCQuvQIDAQAB
-----END PUBLIC KEY-----
    
            *PUBLIC RSA XML KEY 1024

    <RSAKeyValue><Modulus>28hcojmUPKGJAp5+TeocYBZ6YYSiG85+BaRcrQ0c8Lnraa2czWZfZDmguC4VoeJ44JmO7RMGmOUWie+QAJwLRLwlN19J3+sy/LzgYQL9VtrEM7Eyx1KnGWN08ZablH/Vrg7+/K/97+Q9neeimpufTtOCoym7sTcHHvlsa98IytE=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>

            
            *Public ETHEREUM

    04c2f751a057c8edcec6f195a55474c97cdb1206cc883315373de580e3f08153eba367136450c731e2fe6c89a0b06cf0f3c422c6b5302e5b2900bcca88ae757432             


           *Metamaks account
address_eth= "0x7DBca59f14d3300EadC5985d038C55867739E386"
private_eth= "7aa703e35c97dd7cd40c3b55c88fa093b0c1ac67ffb19f4b7ce26b64e6cc46e3"


                 */


        }
    }
}
