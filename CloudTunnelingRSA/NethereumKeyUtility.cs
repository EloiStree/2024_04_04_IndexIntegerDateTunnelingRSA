using Nethereum.Signer;
using Nethereum.Hex.HexConvertors.Extensions;

public class NethereumKeyUtility {

    public void GenerateEthereumKey(out string privateKeyEthereum, out string publicKeyEthereum, out string address)
    {

        var ecKey = EthECKey.GenerateKey();
        privateKeyEthereum = ecKey.GetPrivateKeyAsBytes().ToHex();
        publicKeyEthereum = ecKey.GetPubKey().ToHex();
        address = ecKey.GetPublicAddress();
    }

}
