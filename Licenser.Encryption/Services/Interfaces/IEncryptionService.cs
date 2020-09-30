namespace Licenser.Encryption.Services
{
    public interface IEncryptionService
    {
        IRSAKeyService RSAKeyService { get; }

        string Encrypt(string dataToEncrypt);

        string Decrypt(string dataToDecrypt);
    }
}
