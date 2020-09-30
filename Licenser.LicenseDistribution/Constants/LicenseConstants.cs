namespace Licenser.LicenseDistribution.Constants
{
    public static class LicenseConstants
    {
        public const string LicenseDbIdAttributeName = "LicenseDbId";
        public const string NumberOfConcurrentUserSessionsAttributeName = "NumberOfConcurrentUserSessionsAllowed";
        public const string LarktonKeyContainer = "LarktonKeyContainer";

        public const string PassPhrase = "T3OUFH-1BX0V4W-15VPOIB-CS4BGD";

        public const string LicensesDirectory = "Licenses";
        public const string LicensFileExtension = ".lic";

        public const string KeysDirectory = "Keys";
        public static readonly string PrivateKeyDirectory = $"{KeysDirectory}/pvtk_lic.txt";
        public static readonly string PublicKeyDirectory = $"{KeysDirectory}/pbk_lic.txt";
    }
}
