using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Licenser.Encryption.Services
{
    public interface IRSAKeyService
    {
        void GeneratePublicPrivateKeyPair();

        Task<string> GetPublicKey();


        Task<string> GetPrivateKey();

        RSACryptoServiceProvider GetRSAProviderFromContainer(string containerName);

        void DeleteKeyFromContainer(string containerName);
    }
}
