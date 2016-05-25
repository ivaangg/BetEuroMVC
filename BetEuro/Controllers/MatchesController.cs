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
    [Authorize(Roles = "Admin")]
    public class MatchesController : Controller
    {
        private BEEntities db = new BEEntities();
        


        // GET: Matches
        public async Task<ActionResult> Index()
        {
            var matches = db.Matches.Include(m => m.AwayTeam).Include(m => m.HomeTeam).Include(m => m.Score);
            return View(await matches.ToListAsync());
        }

        // GET: Matches/Create
        public ActionResult Create()
        {
            ViewBag.AwayId = new SelectList(db.Teams, "Id", "LongName");
            ViewBag.HomeId = new SelectList(db.Teams, "Id", "LongName");
            ViewBag.Id = new SelectList(db.Scores, "Id", "Id");
            return View();
        }

        // POST: Matches/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,HomeId,AwayId,Date,Factor")] Match match)
        {
            if (ModelState.IsValid)
            {
                db.Matches.Add(match);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.AwayId = new SelectList(db.Teams, "Id", "LongName", match.AwayId);
            ViewBag.HomeId = new SelectList(db.Teams, "Id", "LongName", match.HomeId);
            ViewBag.Id = new SelectList(db.Scores, "Id", "Id", match.Id);
            return View(match);
        }

        // GET: Matches/Edit/5
        public async Task<ActionResult> Edit(int? id)
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

        // POST: Matches/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,HomeId,AwayId,Date,Factor")] Match match)
        {
            if (ModelState.IsValid)
            {
                db.Entry(match).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.AwayId = new SelectList(db.Teams, "Id", "LongName", match.AwayId);
            ViewBag.HomeId = new SelectList(db.Teams, "Id", "LongName", match.HomeId);
            ViewBag.Id = new SelectList(db.Scores, "Id", "Id", match.Id);
            return View(match);
        }

        // GET: Matches/Delete/5
        public async Task<ActionResult> Delete(int? id)
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
            return View(match);
        }

        // POST: Matches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Match match = await db.Matches.FindAsync(id);
            foreach (Bet bet in db.Bets.Where(p => p.Match.Id == id))
                db.Bets.Remove(bet);
            foreach (Score score in db.Scores.Where(p => p.Match.Id == id))
                db.Scores.Remove(score);
            db.Matches.Remove(match);
            await db.SaveChangesAsync();

            UpdateLeaderboard();

            return RedirectToAction("Index");
        }

        private void UpdateLeaderboard()
        {
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [Leaderboard]");
            db.SaveChanges();

            foreach (User u in db.Users.Where(p => p.isActive))
            {
                Leaderboard lb = new Leaderboard();
                lb.User = u;
                lb.UserId = u.Id;
                lb.PlacedBets = 0;
                lb.ResultHit = 0;
                lb.ScoreHit = 0;
                lb.Points = 0;

                foreach (Bet b in u.Bets)
                {
                    Match m = db.Matches.Single(p => p.Id == b.MatchId);

                    if (m.Score != null)
                    {
                        if (m.Score.HomeScore == b.HomeScore && m.Score.AwayScore == b.AwayScore)
                        {
                            //SCORE
                            lb.ScoreHit++;
                            lb.Points += m.Factor.Value * db.Points.Single(p => p.Id == "Score").Points;
                        }
                        else if (m.Score.Result == b.Result)
                        {
                            //RESULT
                            lb.ResultHit++;
                            lb.Points += m.Factor.Value * db.Points.Single(p => p.Id == "Result").Points;
                        }
                        else
                        {
                            // BET POINTS
                            lb.PlacedBets++;
                            lb.Points += m.Factor.Value * db.Points.Single(p => p.Id == "Bet").Points;
                        }
                    }

                }

                db.Leaderboards.Add(lb);
            }

            db.SaveChanges();
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
