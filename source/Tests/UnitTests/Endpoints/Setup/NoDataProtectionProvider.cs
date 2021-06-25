using Microsoft.Owin.Security.DataProtection;

namespace IdentityServer3.Tests.Endpoints.Setup
{
    public class NoDataProtectionProvider : IDataProtectionProvider
    {
        public IDataProtector Create(params string[] purposes)
        {
            return new NoDataProtector();
        }
    }

    public class NoDataProtector : IDataProtector
    {
        public byte[] Protect(byte[] userData)
        {
            return userData;
        }

        public byte[] Unprotect(byte[] protectedData)
        {
            return protectedData;
        }
    }
}