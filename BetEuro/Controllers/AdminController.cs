using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using BetEuro.Models;
using BetEuro;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Net;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BetEuro.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {

        // GET: Admin
        public ActionResult Admin()
        {
            return View();
        }
    }
}