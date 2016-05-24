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
    [Authorize]
    public class HomeController : Controller
    {
        private BEEntities db = new BEEntities();

        [AllowAnonymous]
        public async Task<ActionResult> Index()
        {
            var matches = db.Matches.Include(m => m.AwayTeam).Include(m => m.HomeTeam).Include(m => m.Score);
            return View(await matches.ToListAsync());
        }

        public async Task<ActionResult> Rules()
        {
            return View();
        }

        public async Task<ActionResult> Matches()
        {
            var matches = db.Matches.Include(m => m.AwayTeam).Include(m => m.HomeTeam).Include(m => m.Score);
            return View(await matches.ToListAsync());
        }

        public async Task<ActionResult> Leaderboard()
        {
            var points = db.Leaderboards;
            return View(await points.ToListAsync());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // GET: Matches/SingleMatch/
        public async Task<ActionResult> SingleMatch(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Match match = await db.Matches.FindAsync(id);
            if (match == null)
            {
                return HttpNotFound();
            }
            ViewBag.AwayId = new SelectList(db.Teams, "Id", "LongName", match.AwayId);
            ViewBag.HomeId = new SelectList(db.Teams, "Id", "LongName", match.HomeId);
            ViewBag.Id = new SelectList(db.Scores, "Id", "Id", match.Id);
            return View(match);
        }

        public async Task<ActionResult> BetThisMatch(FormCollection form)
        {
            Bet bet;

            int matchId = 0;
            int homeScore = 0;
            int awayScore = 0;

            if (!Int32.TryParse(form[0], out matchId))
            {
                return RedirectToAction("Index");
            }
            if (!Int32.TryParse(form[2], out homeScore))
            {
                return View("SingleMatch", db.Matches.Single(p => p.Id == matchId));
            }
            if (!Int32.TryParse(form[3], out awayScore))
            {
                return View("SingleMatch", db.Matches.Single(p => p.Id == matchId));
            }

            if (db.Matches.Single(p => p.Id == matchId).Date > DateTime.Now)
            {

                string userId = form[1];
                if (matchId != 0 && userId != "")
                {
                    if (db.Bets.Any(p => matchId == p.MatchId && p.User.UserName == userId))
                    {
                        var betToDel = db.Bets.Single(p => matchId == p.MatchId && p.User.UserName == userId);
                        db.Bets.Remove(betToDel);
                        db.SaveChanges();
                    }

                    bet = new Bet();
                    bet.Match = db.Matches.Single(p => p.Id == matchId);
                    bet.User = db.Users.Single(p => p.UserName == userId);
                }
                else
                {
                    return View("SingleMatch", db.Matches.Single(p => p.Id == matchId));
                }

                bet.HomeScore = homeScore;
                bet.AwayScore = awayScore;

                if (homeScore > awayScore)
                    bet.Result = 1;
                else if (homeScore < awayScore)
                    bet.Result = 2;
                else
                    bet.Result = 0;


                db.Bets.Add(bet);


                await db.SaveChangesAsync();
                return View("SingleMatch", db.Matches.Single(p => p.Id == matchId));
            }
            else
            {
                ViewBag.Message = "Czas na obstawianie wyniku się zakończył.";
                return View("SingleMatch", db.Matches.Single(p => p.Id == matchId));
            }
        }
    }
}