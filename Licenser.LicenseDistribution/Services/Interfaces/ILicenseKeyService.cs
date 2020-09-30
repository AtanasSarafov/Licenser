using System.Threading.Tasks;

namespace Licenser.LicenseDistribution.Services
{
    public interface ILicenseKeyService
    {
        void GeneratePublicPrivateKeyPair();

        Task<string> GetPublicKey();

        Task<string> GetPrivateKey();
    }
}
