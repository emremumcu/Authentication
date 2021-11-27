using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Authentication.AppLib.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.Controllers
{
    public class CustomController : Controller
    {
        [AppAuthorizeAttribute(Privileges = "")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
