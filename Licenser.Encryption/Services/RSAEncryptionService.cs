using Licenser.Encryption.Extensions;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Licenser.Encryption.Services
{
    public class RSAEncryptionService : IEncryptionService
    {
        private readonly bool optimalAsymmetricEncryptionPadding = false;

        public IRSAKeyService RSAKeyService => new RSAKeyService();

        public string Encrypt(string plainText)
        {
            var publicKey = this.RSAKeyService.GetPublicKey().Result;

            GetKeyFromEncryptionString(publicKey, out int keySize, out string publicKeyXml);
            var encrypted = this.Encrypt(Encoding.UTF8.GetBytes(plainText), keySize, publicKeyXml);

            return Convert.ToBase64String(encrypted);
        }

        public string Decrypt(string encryptedText)
        {
            if (!IsBase64String(encryptedText))
            {
                return encryptedText;
            }

            var privateKey = this.RSAKeyService.GetPrivateKey().Result;

            GetKeyFromEncryptionString(privateKey, out int keySize, out string publicAndPrivateKeyXml);
            var decrypted = this.Decrypt(Convert.FromBase64String(encryptedText), keySize, publicAndPrivateKeyXml);

            return Encoding.UTF8.GetString(decrypted);
        }

        private byte[] Encrypt(byte[] data, int keySize, string publicKeyXml)
        {
            if (data == null || data.Length == 0)
            {
                throw new ArgumentException("Data are empty", "data");
            }

            int maxLength = GetMaxDataLength(keySize);
            if (data.Length > maxLength)
            {
                throw new ArgumentException(string.Format("Maximum data length is {0}", maxLength), "data");
            }

            if (!IsKeySizeValid(keySize))
            {
                throw new ArgumentException("Key size is not valid", "keySize");
            }

            if (string.IsNullOrEmpty(publicKeyXml))
            {
                throw new ArgumentException("Key is null or empty", "publicKeyXml");
            }

            using (var provider = new RSACryptoServiceProvider(keySize))
            {
                RSAKeyExtensions.FromXmlString(provider, publicKeyXml);
                return provider.Encrypt(data, optimalAsymmetricEncryptionPadding);
            }
        }

        private byte[] Decrypt(byte[] data, int keySize, string publicAndPrivateKeyXml)
        {
            if (data == null || data.Length == 0)
            {
                throw new ArgumentException("Data are empty", "data");
            }

            if (!IsKeySizeValid(keySize))
            {
                throw new ArgumentException("Key size is not valid", "keySize");
            }

            if (string.IsNullOrEmpty(publicAndPrivateKeyXml))
            {
                throw new ArgumentException("Key is null or empty", "publicAndPrivateKeyXml");
            }

            using (var provider = new RSACryptoServiceProvider(keySize))
            {
                RSAKeyExtensions.FromXmlString(provider, publicAndPrivateKeyXml);
                return provider.Decrypt(data, optimalAsymmetricEncryptionPadding);
            }
        }

        private int GetMaxDataLength(int keySize)
        {
            if (optimalAsymmetricEncryptionPadding)
            {
                return ((keySize - 384) / 8) + 7;
            }
            return ((keySize - 384) / 8) + 37;
        }

        private bool IsKeySizeValid(int keySize)
        {
            return keySize >= 384 && keySize <= 16384 && keySize % 8 == 0;
        }

        private void GetKeyFromEncryptionString(string rawkey, out int keySize, out string xmlKey)
        {
            keySize = 0;
            xmlKey = "";

            if (rawkey != null && rawkey.Length > 0)
            {
                byte[] keyBytes = Convert.FromBase64String(rawkey);
                var stringKey = Encoding.UTF8.GetString(keyBytes);

                if (stringKey.Contains("!"))
                {
                    var splittedValues = stringKey.Split(new char[] { '!' }, 2);

                    try
                    {
                        keySize = int.Parse(splittedValues[0]);
                        xmlKey = splittedValues[1];
                    }
                    catch (Exception) { }
                }
            }
        }

        private static bool IsBase64String(string base64)
        {
            Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
            return Convert.TryFromBase64String(base64, buffer, out _);
        }
    }
}
