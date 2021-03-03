using System.Security.Cryptography;

namespace ThemePark.Infrastructure.Security.DataProtection.Dpapi
{
    internal class DpapiDataProtector : IDataProtector
    {
        private readonly System.Security.Cryptography.DpapiDataProtector _protector;

        public DpapiDataProtector(string appName, string[] purposes)
        {
            this._protector = new System.Security.Cryptography.DpapiDataProtector(appName, "DpapiDataProtector", purposes)
            {
                Scope = DataProtectionScope.CurrentUser
            };
        }

        public byte[] Protect(byte[] plaintext)
        {
            return this._protector.Protect(plaintext);
        }

        public byte[] Unprotect(byte[] protectedData)
        {
            return this._protector.Unprotect(protectedData);
        }
    }
}
