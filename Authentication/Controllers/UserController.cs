namespace Authentication.Controllers
{
    using Authentication.AppLib.Requirements;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Policy = UserRequirement.PolicyName)]
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
