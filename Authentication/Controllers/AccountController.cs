﻿namespace Authentication.Controllers
{
    using Authenticate.AppLib.Abstract;
    using Authenticate.AppLib.Concrete;
    using Authentication.AppLib.Concrete;
    using Authentication.AppLib.StartupExt;
    using Authentication.AppLib.Tools;
    using Authentication.ViewModels;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.IO;
    using System.Threading.Tasks;

    [ApiExplorerSettings(IgnoreApi = true)]
    [AllowAnonymous]
    public partial class AccountController : Controller
    {
        private IAuthenticate _authenticator;
        private IAuthorize _authorizer;

        public AccountController(IAuthenticate authenticator, IAuthorize authorizer)
        {
            _authenticator = authenticator;
            _authorizer = authorizer;
        }

        [NonAction]
        private async Task<IActionResult> LoginUser(string username, string password, bool remember)
        {
            if (_authenticator.AuthenticateUser("", username, password))
            {
                AuthenticationTicket userTicket = _authorizer.GetTicket(TestAccounts.GetUserClaims());
                
                userTicket.Properties.IsPersistent = remember;

                HttpContext.Session.SetKey<bool>(Constants.SessionKeyLogin , true);

                await HttpContext.SignInAsync(
                    userTicket.AuthenticationScheme,
                    userTicket.Principal,
                    userTicket.Properties
                );
                
                return RedirectToAction("Index", "Home");
            }
            else
            {
                throw new Exception("Kullanıcı bilgileri hatalı.");
            }
        }

        [HttpGet]
        public IActionResult Login() => View(model: new LoginViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            void ClearCaptchaText()
            {
                // Even the CaptchaCode is cleared in Model,
                // without clearing the Captcha.CaptchaCode in ModelState ,
                // CaptchaCode textbox is preserving the previous entry !!!
                model.Captcha.CaptchaCode = string.Empty;
                // ModelState.SetModelValue("Captcha.CaptchaCode", new ValueProviderResult(string.Empty)); // Not working!!!
                ModelState.Remove("Captcha.CaptchaCode"); 
            }

            try
            {
                if (ModelState.IsValid)
                {
                    if (!Captcha2.ValidateCaptchaCode(model.Captcha.CaptchaCode, HttpContext))
                    {
                        ClearCaptchaText();
                        ModelState.AddModelError("Captcha", "Güvenlik kodu yanlış.");
                        return View(model);
                    }
                    else
                    {
                        return await LoginUser(model.Username, model.Password, model.RememberMe);
                    }
                }
                else
                {
                    ClearCaptchaText();
                    ModelState.AddModelError("ERR", $"Formda hatalar var. Lütfen hataları düzeltip, işleminizi yeniden deneyiniz.");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ClearCaptchaText();
                ModelState.AddModelError("ERR", $"Hata: {ex.Message}");
                return View(model);
            }
        }

        public IActionResult AccessDenied()
        {
            //return Forbid();
            //return StatusCode(403);
            return Content("You are not allowed to access this resource");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.RemoveKey(Constants.SessionKeyLogin);

            HttpContext.Session.Clear();

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var session_cookie = HttpContext.Request.Cookies[Constants.Session_Cookie_Name];
            if (session_cookie != null)
            {
                var options = new CookieOptions { Expires = DateTime.Now.AddDays(-1) };
                HttpContext.Response.Cookies.Append(Constants.Session_Cookie_Name, session_cookie, options);
            }

            var auth_cookie = HttpContext.Request.Cookies[Constants.Auth_Cookie_Name];
            if (auth_cookie != null)
            {
                var options = new CookieOptions { Expires = DateTime.Now.AddDays(-1) };
                HttpContext.Response.Cookies.Append(Constants.Session_Cookie_Name, auth_cookie, options);
            }

            return RedirectToAction("Login");
        }

        [Route("get-captcha-image")]
        public IActionResult GetCaptchaImage()
        {
            var result = Captcha2.GenerateCaptchaImage(HttpContext);
            Stream s = new MemoryStream(result.CaptchaByteData);
            return new FileStreamResult(s, "image/png");
        }

        [HttpGet]
        public IActionResult Register() => View(model: new RegisterViewModel());

        // ASP.NET Core provides an attribute called FromServices to inject the dependencies directly into the controller’s action method
        // use FromServices attribute to inject the dependency directly (without constructor DI)
        // services.AddTransient<IAccountService, AccountService>(); is required !!!
        // So when we decorate the parameter with FromServices attribute, this instructs the ASP.NET 
        // Core to get it from the services container and inject the matching implementation. 
        // This makes the code cleaner and will also reduce the work of modifying the existing unit test cases.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            void ClearCaptchaText()
            {
                // model içindeki CaptchaCode temizlense bile, ModelState değeri de temizlenmeden CaptchaCode textbox değeri dolu geliyor
                model.Captcha.CaptchaCode = string.Empty;
                ModelState.Remove("Captcha.CaptchaCode"); // ModelState.SetModelValue("Captcha.CaptchaCode", new ValueProviderResult(string.Empty));
            }

            try
            {
                if (ModelState.IsValid)
                {
                    if (!Captcha2.ValidateCaptchaCode(model.Captcha.CaptchaCode, HttpContext))
                    {
                        ClearCaptchaText();
                        ModelState.AddModelError("Captcha", "Güvenlik kodu yanlış.");
                        return View(model);
                    }
                    else
                    {
                        // INFO Register user here
                        await Task.Delay(1);
                        return RedirectToAction("Login");
                    }
                }
                else
                {
                    ClearCaptchaText();
                    ModelState.AddModelError("ERR", $"Formda hatalar var. Lütfen hataları düzeltip, işleminizi yeniden deneyiniz.");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ClearCaptchaText();
                ModelState.AddModelError("ERR", $"Ex:{ex?.Message}{Environment.NewLine}InnerEx:{ex?.InnerException?.Message}");
                return View(model);
            }
        }
    }
}


// Snippets
//[NonAction]
//private async Task<IActionResult> CustomLogin()
//{
//    List<Claim> userClaims = new List<Claim>();
//    ClaimsIdentity claimsIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);
//    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
//    AuthenticationProperties apr = new AuthenticationProperties() { ExpiresUtc = new DateTimeOffset().AddMinutes(20) };
//    await HttpContext.RequestServices.GetRequiredService<IAuthenticationService>().SignInAsync(HttpContext, "cookies", claimsPrincipal, apr);
//    HttpContext.Session.SetKey<bool>("login", true);
//    HttpContext.User = claimsPrincipal;
//    return RedirectToAction("Index", "Home");
//}

//IServiceProvider services = HttpContext.RequestServices;
//IAuthorize authorizer = services.GetRequiredService<IAuthorize>();
//List<Claim> userClaims = new List<Claim>();
//ClaimsIdentity claimsIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);
//ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
//HttpContext.Session.SetKey<bool>("login", true);
//await HttpContext.SignInAsync(
//    CookieAuthenticationDefaults.AuthenticationScheme,
//    claimsPrincipal,
//    new AuthenticationProperties
//    {
//        AllowRefresh = true,
//        ExpiresUtc = DateTime.UtcNow.AddMinutes(10),
//        IsPersistent = (true),
//        IssuedUtc = DateTime.UtcNow
//    }
//);
//System.Security.Claims.ClaimsIdentity ci2 = ((System.Security.Claims.ClaimsIdentity)User.Identity);
//return RedirectToAction("Index", "Home");

// Install-Package Microsoft.IdentityModel
// Install-Package Microsoft.IdentityModel.Tokens
// Install-Package System.IdentityModel.Tokens.Jwt
//[NonAction]
//public string GenerateJwtToken()
//{
//    var claims = new List<Claim>{
//                new Claim(JwtRegisteredClaimNames.Sub, "sub"),
//                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
//                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
//            };

//    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ThisIsMyPasswrod3"));
//    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
//    var expires = DateTime.Now.AddDays(Convert.ToDouble(30));

//    var token = new JwtSecurityToken(
//        issuer: "http://localhost",
//        audience: "http://localhost",
//        claims: claims,
//        expires: expires,
//        signingCredentials: creds
//    );

//    return new JwtSecurityTokenHandler().WriteToken(token);
//}