using Licenser.LicenseDistribution.Constants;
using Standard.Licensing.Security.Cryptography;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Licenser.LicenseDistribution.Services
{
    public class LicenseKeyService : ILicenseKeyService
    {
        public void GeneratePublicPrivateKeyPair()
        {
            Directory.CreateDirectory(LicenseConstants.KeysDirectory);

            var keyGenerator = KeyGenerator.Create();
            var keyPair = keyGenerator.GenerateKeyPair();

            if (File.Exists(LicenseConstants.PrivateKeyDirectory) || File.Exists(LicenseConstants.PublicKeyDirectory))
            {
                return;
            }

            File.WriteAllText(LicenseConstants.PrivateKeyDirectory, keyPair.ToEncryptedPrivateKeyString(LicenseConstants.PassPhrase), Encoding.UTF8);
            File.WriteAllText(LicenseConstants.PublicKeyDirectory, keyPair.ToPublicKeyString(), Encoding.UTF8);
        }

        public async Task<string> GetPublicKey()
        {
            if (!File.Exists(LicenseConstants.PublicKeyDirectory))
            {
                new FileNotFoundException(LicenseConstants.PublicKeyDirectory);
            }

            return await File.ReadAllTextAsync(LicenseConstants.PublicKeyDirectory);
        }

        public async Task<string> GetPrivateKey()
        {
            if (!File.Exists(LicenseConstants.PrivateKeyDirectory))
            {
                new FileNotFoundException(LicenseConstants.PrivateKeyDirectory);
            }

            return await File.ReadAllTextAsync(LicenseConstants.PrivateKeyDirectory);
        }
    }
}
