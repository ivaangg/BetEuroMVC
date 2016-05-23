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
    public class ScoresController : Controller
    {
        private BEEntities db = new BEEntities();

        // GET: Scores
        public async Task<ActionResult> Index()
        {
            var scores = db.Scores.Include(s => s.Match);
            return View(await scores.ToListAsync());
        }

        // GET: Scores/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Score score = await db.Scores.FindAsync(id);
            if (score == null)
            {
                return HttpNotFound();
            }
            return View(score);
        }

        // GET: Scores/Create
        public ActionResult Create()
        {
            ViewBag.Id = new SelectList((from s in db.Matches.Where(p => p.Score == null).ToList() select new {Id = s.Id,FullName = s.HomeTeam.ShortName + "-" + s.AwayTeam.ShortName}),"Id","FullName",null);
            return View();
        }

        // POST: Scores/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,HomeScore,AwayScore")] Score score)
        {
            if (ModelState.IsValid)
            {
                if (score.HomeScore > score.AwayScore)
                {
                    score.Result = 1;
                }
                else if (score.HomeScore == score.AwayScore)
                {
                    score.Result = 0;
                }
                else
                {
                    score.Result = 2;
                }
                db.Scores.Add(score);                
                await db.SaveChangesAsync();

                UpdateLeaderboard();

                return RedirectToAction("Index");
            }

            ViewBag.Id = new SelectList((from s in db.Matches.Where(p => p.Score == null).ToList() select new { Id = s.Id, FullName = s.HomeTeam.ShortName + "-" + s.AwayTeam.ShortName }), "Id", "FullName", null);
            return View();
        }

        // GET: Scores/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Score score = await db.Scores.FindAsync(id);
            if (score == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id = new SelectList(db.Matches, "Id", "Id", score.Id);
            return View(score);
        }

        // POST: Scores/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,HomeScore,AwayScore")] Score score)
        {
            if (ModelState.IsValid)
            {               
                if (score.HomeScore > score.AwayScore)
                {
                    score.Result = 1;
                }
                else if (score.HomeScore == score.AwayScore)
                {
                    score.Result = 0;
                }
                else
                {
                    score.Result = 2;
                }

                db.Entry(score).State = EntityState.Modified;
                await db.SaveChangesAsync();

                UpdateLeaderboard();

                return RedirectToAction("Index");
            }
            ViewBag.Id = new SelectList(db.Matches, "Id", "Id", score.Id);
            return View(score);
        }

        // GET: Scores/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Score score = await db.Scores.FindAsync(id);
            if (score == null)
            {
                return HttpNotFound();
            }
            return View(score);
        }

        // POST: Scores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Score score = await db.Scores.FindAsync(id);
            db.Scores.Remove(score);
            await db.SaveChangesAsync();
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
                        // BET POINTS
                        lb.PlacedBets++;
                        lb.Points += m.Factor.Value * db.Points.Single(p => p.Id == "Bet").Points;

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
