namespace Licenser.Encryption.Constants
{
    public static class EncryptionConstants
    {
        public const string EncryptionContainerName = "Licenser.Encryption.Container";
        public const string KeysDirectory = "Keys";

        public static readonly string PrivateKeyDirectory = $"{KeysDirectory}/pvtk_rsa.txt";
        public static readonly string PublicKeyDirectory = $"{KeysDirectory}/pbk_rsa.txt";
    }
}
