using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BetEuro;

namespace BetEuro.Controllers
{
    public class HomeController : Controller
    {
        private BEEntities db = new BEEntities();

        public async Task<ActionResult> Index()
        {
            var matches = db.Matches.Include(m => m.AwayTeam).Include(m => m.HomeTeam).Include(m => m.Score);
            return View(await matches.ToListAsync());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}