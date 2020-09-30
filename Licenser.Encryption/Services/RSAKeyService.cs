using Licenser.Encryption.Constants;
using Licenser.Encryption.Entities;
using Licenser.Encryption.Extensions;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Licenser.Encryption.Services
{
    public class RSAKeyService : IRSAKeyService
    {
        public void GeneratePublicPrivateKeyPair()
        {
            Directory.CreateDirectory(EncryptionConstants.KeysDirectory);

            var keyPair = GenerateKeys(RSAKeySize.Key2048);

            if (File.Exists(EncryptionConstants.PrivateKeyDirectory) || File.Exists(EncryptionConstants.PublicKeyDirectory))
            {
                return;
            }

            File.WriteAllText(EncryptionConstants.PrivateKeyDirectory, keyPair.PrivateKey, Encoding.UTF8);
            File.WriteAllText(EncryptionConstants.PublicKeyDirectory, keyPair.PublicKey, Encoding.UTF8);
        }

        public async Task<string> GetPublicKey()
        {
            if (!File.Exists(EncryptionConstants.PublicKeyDirectory))
            {
                new FileNotFoundException(EncryptionConstants.PublicKeyDirectory);
            }

            return await File.ReadAllTextAsync(EncryptionConstants.PublicKeyDirectory);
        }

        public async Task<string> GetPrivateKey()
        {
            if (!File.Exists(EncryptionConstants.PrivateKeyDirectory))
            {
                new FileNotFoundException(EncryptionConstants.PrivateKeyDirectory);
            }

            return await File.ReadAllTextAsync(EncryptionConstants.PrivateKeyDirectory);
        }

        public RSACryptoServiceProvider GetRSAProviderFromContainer(string containerName)
        {
            var cp = new CspParameters
            {
                KeyContainerName = containerName
            };

            return new RSACryptoServiceProvider(cp);
        }

        public void DeleteKeyFromContainer(string containerName)
        {
            var cp = new CspParameters
            {
                KeyContainerName = containerName
            };

            var rsa = new RSACryptoServiceProvider(cp)
            {
                PersistKeyInCsp = false
            };

            rsa.Clear();
        }

        private RSAKeysPair GenerateKeys(RSAKeySize rsaKeySize)
        {
            int keySize = (int)rsaKeySize;
            if (keySize % 2 != 0 || keySize < 512)
            {
                throw new Exception("Key should be multiple of two and greater than 512.");
            }

            var rsaKeysTypes = new RSAKeysPair();

            using (var provider = new RSACryptoServiceProvider(keySize))
            {
                var publicKey = RSAKeyExtensions.ToXmlString(provider, false);
                var privateKey = RSAKeyExtensions.ToXmlString(provider, true);

                var publicKeyWithSize = IncludeKeyInEncryptionString(publicKey, keySize);
                var privateKeyWithSize = IncludeKeyInEncryptionString(privateKey, keySize);

                rsaKeysTypes.PublicKey = publicKeyWithSize;
                rsaKeysTypes.PrivateKey = privateKeyWithSize;
            }

            return rsaKeysTypes;
        }

        private string IncludeKeyInEncryptionString(string publicKey, int keySize)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(keySize.ToString() + "!" + publicKey));
        }
    }
}
