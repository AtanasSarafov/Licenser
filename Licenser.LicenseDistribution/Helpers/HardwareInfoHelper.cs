using Licenser.Encryption.Services;
using System;
using System.Management;

namespace Licenser.LicenseDistribution.Helpers
{
    public static class HardwareInfoHelper
    {
        private const string splitter = "-";

        private static readonly RSAEncryptionService RSAEncryptionService;

        static HardwareInfoHelper()
        {
            RSAEncryptionService = new RSAEncryptionService();
        }

        public static string GenerateHardwareId()
        {
            var hardwareId = $"{GetProcessorId()}-{GetMotherboardID()}-{GetDiskVolumeSerialNumber()}";
            return hardwareId;
        }

        public static string EncryptHardwareId(string hardwareId)
        {
            var encryptedData = RSAEncryptionService.Encrypt(hardwareId);
            return encryptedData;
        }

        public static bool ValidateHardwareId(string encryptedDataHardwareId)
        {
            var hardwareIdForValidation = RSAEncryptionService.Decrypt(encryptedDataHardwareId);

            if (!ValidateHardwareIdFormat(hardwareIdForValidation))
            {
                throw new ArgumentException("Wrong Hardware ID");
            }

            var currentHardwareId = $"{GetProcessorId()}{splitter}{GetMotherboardID()}{splitter}{GetDiskVolumeSerialNumber()}";

            return hardwareIdForValidation == currentHardwareId;
        }

        public static bool ValidateHardwareIdFormat(string hardwareId)
        {
            if (string.IsNullOrWhiteSpace(hardwareId))
            {
                return false;
            }

            return (hardwareId.Split(splitter).Length == 3);
        }

        private static string GetDiskVolumeSerialNumber()
        {
            try
            {
                var disk = new ManagementObject(@"Win32_LogicalDisk.deviceid=""c:""");
                disk.Get();
                return disk["VolumeSerialNumber"].ToString();
            }
            catch
            {
                // TODO: Add error logging.
                return string.Empty;
            }
        }

        private static string GetProcessorId()
        {
            try
            {
                var cpu = new ManagementObjectSearcher("SELECT ProcessorId From Win32_Processor");
                var cpuCollection = cpu.Get();
                var cpuId = string.Empty;

                foreach (var obj in cpuCollection)
                {
                    cpuId += obj["ProcessorId"].ToString();
                }

                return cpuId;
            }
            catch
            {
                // TODO: Add error logging.
                return string.Empty;
            }
        }

        private static string GetMotherboardID()
        {
            try
            {
                var baseBoard = new ManagementObjectSearcher("Select SerialNumber From Win32_BaseBoard");
                var baseBoardCollection = baseBoard.Get();
                var motherboardSerialNumber = string.Empty;

                foreach (var obj in baseBoardCollection)
                {
                    motherboardSerialNumber = obj["SerialNumber"].ToString();
                    break;
                }

                return motherboardSerialNumber;
            }
            catch
            {
                // TODO: Add error logging.
                return string.Empty;
            }
        }
    }
}
