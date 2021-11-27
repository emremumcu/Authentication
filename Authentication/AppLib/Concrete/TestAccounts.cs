namespace Authentication.AppLib.Concrete
{
    using System.Collections.Generic;
    using System.Security.Claims;

    public class TestAccounts
    {
        public static List<Claim> GetUserClaims()
        {
            List<Claim> UserClaims = new List<Claim>
            {
                // https://docs.microsoft.com/en-us/dotnet/api/system.security.claims.claimtypes?view=net-6.0

                new Claim(ClaimTypes.NameIdentifier, "User1"),
                new Claim(ClaimTypes.Name, "User"),
                new Claim(ClaimTypes.Surname, "One"),
                new Claim(ClaimTypes.Email, "userone@myapp.com"),                
                new Claim(ClaimTypes.IsPersistent, "true"),
                new Claim(ClaimTypes.Role, "USER")
            };

            return UserClaims;
        }

        public static List<Claim> GetAdminClaims()
        {
            List<Claim> AdminClaims = new List<Claim>
            {
                // https://docs.microsoft.com/en-us/dotnet/api/system.security.claims.claimtypes?view=net-6.0

                new Claim(ClaimTypes.NameIdentifier, "Admin1"),
                new Claim(ClaimTypes.Name, "Admin"),
                new Claim(ClaimTypes.Surname, "One"),
                new Claim(ClaimTypes.Email, "adminone@myapp.com"),
                new Claim(ClaimTypes.IsPersistent, "true"),
                new Claim(ClaimTypes.Role, "ADMIN,USER")
            };

            return AdminClaims;
        }
    }
}
