using Licenser.Encryption.Constants;
using System;
using System.Text;

namespace Licenser.Encryption.Services
{
    public class DataEncryptionService : IDataEncryptionService
    {
        private readonly bool optimalAsymmetricEncryptionPadding = false;

        public IRSAKeyService RSAKeyService => new RSAKeyService();

        public string Encrypt(string dataToEncrypt)
        {
            try
            {
                var provider = this.RSAKeyService.GetRSAProviderFromContainer(EncryptionConstants.EncryptionContainerName);
                var encrypted = provider.Encrypt(Encoding.UTF8.GetBytes(dataToEncrypt), optimalAsymmetricEncryptionPadding);
                return Convert.ToBase64String(encrypted);

            }
            catch (Exception)
            {
                // TODO: Add logging.
                //ErrorHelper.WriteToLog($"{nameof(LicensesController)}/{nameof(LicensesController.Export)}", ex);
                return string.Empty;
            }
        }

        public string Decrypt(string encryptedText)
        {
            if (!IsBase64String(encryptedText))
            {
                return encryptedText;
            }

            try
            {
                var provider = this.RSAKeyService.GetRSAProviderFromContainer(EncryptionConstants.EncryptionContainerName);
                var decrypted = provider.Decrypt(Convert.FromBase64String(encryptedText), optimalAsymmetricEncryptionPadding);
                return Encoding.UTF8.GetString(decrypted);
            }
            catch (Exception)
            {
                // TODO: Add logging.
                //ErrorHelper.WriteToLog($"{nameof(LicensesController)}/{nameof(LicensesController.Export)}", ex);
                return string.Empty;
            }
        }

        private static bool IsBase64String(string base64)
        {
            Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
            return Convert.TryFromBase64String(base64, buffer, out _);
        }
    }
}
