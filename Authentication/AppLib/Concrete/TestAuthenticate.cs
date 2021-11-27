namespace Authenticate.AppLib.Concrete
{
    using Authenticate.AppLib.Abstract;

    public class TestAuthenticate : IAuthenticate
    {
        public bool AuthenticateUser(string domain, string userId, string password)
        {
            // TODO : Check user credentials
            return true;
        }
    }
}
