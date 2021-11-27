namespace Authenticate.AppLib.Abstract
{
    public interface IAuthenticate
    {
        public bool AuthenticateUser(string domain, string userId, string password);
    }
}
