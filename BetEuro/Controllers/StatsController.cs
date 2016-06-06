using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BetEuro.Controllers
{
    public class StatsController : Controller
    {
        private BEEntities db = new BEEntities();

        // GET: Stats
        public ActionResult _YourScore(string userName)
        {
            var model = db.Leaderboards.Single(p => p.User.UserName == userName);
            return View(model);
        }

        public ActionResult _YourPosition(string userName)
        {
            var model = db.Leaderboards.Single(p => p.User.UserName == userName);
            return View(model);
        }
    }
}