namespace Authentication.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Authentication.AppLib.Requirements;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    // Role based Authorization
    //[Authorize(Roles = "USER, GUEST")]
    //[Authorize(Roles = "ADMIN")]

    // Policy based Authorization
    [Authorize(Policy = AdminRequirement.PolicyName)]
    public class AdminController : Controller
    {
        public IActionResult Index() => View(viewName: "_Temp", model: "Admin/Index");        
    }
}
