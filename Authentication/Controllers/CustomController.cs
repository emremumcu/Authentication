using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Authentication.AppLib.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.Controllers
{
    [CustomAuthorize(Privileges = "P1, P2")]
    public class CustomController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
