namespace Authenticate.AppLib.Concrete
{
    using System;

    public static class Constants
    {
        // SESSION        
        public static string Session_Cookie_Name = "app.session.cookie";
        public static TimeSpan Session_IdleTimeout = TimeSpan.FromMinutes(20);


        // COMMON

        public static string Claims_Role_Seperator = ";";

        // AUTHENTICATION

        public static string Auth_Cookie_Name = "app.authentication.cookie";
        public static string Auth_Cookie_LoginPath = "/Account/Login";
        public static string Auth_Cookie_LogoutPath = "/Account/Logout";
        public static string Auth_Cookie_AccessDeniedPath = "/Account/AccessDenied";
        public static string Auth_Cookie_ClaimsIssuer = "app.issuer";
        public static string Auth_Cookie_ReturnUrlParameter = "ReturnUrl";
        public static TimeSpan Auth_Cookie_ExpireTimeSpan = TimeSpan.FromMinutes(20);



        // SESSION KEYS

        public static string SessionKeyLogin = "LOGIN";
        public static string SessionKeyLoginUser = "LOGINUSER";
        public static string SessionKey_SelectedRole = "SELECTED_ROLE";
        public static string SessionKey_System_Message = "SYSTEM_MESSAGE";
    }
}
