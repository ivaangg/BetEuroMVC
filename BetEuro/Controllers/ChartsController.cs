using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DotNet.Highcharts;
using DotNet.Highcharts.Options;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Enums;


namespace BetEuro.Controllers
{
    public class ChartsController : Controller
    {

        private BEEntities db = new BEEntities();

        public class NumberAndName
        {
            public int Number { get; set; }
            public string Name { get; set; }

            public NumberAndName(int number, string name)
            {
                Name = name;
                Number = number;
            }
        }
        // GET: Charts
        public ActionResult _UserPointsOfType(string userName)
        {
            var bets = db.Bets.Where(p => p.User.UserName == userName && p.Match.Date < DateTime.Now);

            int miss = 0;
            int hit = 0;
            int score = 0;

            foreach (Bet b in bets)
            {
                if (b.Match.Score != null)
                {
                    int temp = b.GetPointsForThisMatch();
                    switch (temp/b.Match.Factor.Value)
                    {
                        case 1:
                            {
                                miss += temp;
                                break;
                            }
                        case 5:
                            {
                                hit += temp;
                                break;
                            }
                        case 25:
                            {
                                score += temp;
                                break;
                            }
                    }
                }
            }

            string[] categories = new[] { "Nietrafione", "Rezultaty", "Wyniki" };
            List<NumberAndName> data = new List<NumberAndName>();
            data.Add(new NumberAndName(miss, categories[0]));
            data.Add(new NumberAndName(hit, categories[1]));
            data.Add(new NumberAndName(score, categories[2]));



            Highcharts chart = new Highcharts("chartUPoT")
                .InitChart(new Chart { PlotBackgroundColor = null, PlotBorderWidth = null, PlotShadow = false })
                .SetTitle(new Title { Text = "Rozkład punktów w zależności od rodzaju trafienia" })
                .SetTooltip(new Tooltip { PointFormat = "<b>{point.name}</b>: {point.y:.0f} pts" })
                .SetPlotOptions(new PlotOptions
                {
                    Pie = new PlotOptionsPie
                    {
                        AllowPointSelect = true,
                        Cursor = Cursors.Pointer,
                        DataLabels = new PlotOptionsPieDataLabels
                        {
                            Enabled = false,
                            Format = "<b>{point.name}</b>: {point.y:.0f}",
                            Style = "color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'"
                        },
                        ShowInLegend = true                        
                    }
                })
                .SetSeries(new Series
                {
                    Type = ChartTypes.Pie,
                    Name = "Rozkład punktów",
                    Data = new Data(new object[]
                    {
                                    new object[] { data[0].Name, data[0].Number },
                                    new object[] { data[1].Name, data[1].Number },
                                    new object[] { data[2].Name, data[2].Number }
                    })
                });

            return View(chart);
        }
        public ActionResult _UserBetsCount(string userName)
        {
            var bets = db.Bets.Where(p => p.User.UserName == userName && p.Match.Date < DateTime.Now);

            int miss = 0;
            int hit = 0;
            int score = 0;

            foreach (Bet b in bets)
            {
                if (b.Match.Score != null)
                {
                    int temp = b.GetPointsForThisMatch();
                    switch (temp / b.Match.Factor.Value)
                    {
                        case 1:
                            {
                                miss += 1;
                                break;
                            }
                        case 5:
                            {
                                hit += 1;
                                break;
                            }
                        case 25:
                            {
                                score += 1;
                                break;
                            }
                    }
                }
            }

            string[] categories = new[] { "Nietrafione", "Rezultaty", "Wyniki" };
            List<NumberAndName> data = new List<NumberAndName>();
            data.Add(new NumberAndName(miss, categories[0]));
            data.Add(new NumberAndName(hit, categories[1]));
            data.Add(new NumberAndName(score, categories[2]));



            Highcharts chart = new Highcharts("chartUBC")
                .InitChart(new Chart { PlotBackgroundColor = null, PlotBorderWidth = null, PlotShadow = false })
                .SetTitle(new Title { Text = "Rozkład trafionych typów ilościowo" })
                .SetTooltip(new Tooltip { PointFormat = "<b>{point.name}</b>: {point.y:.0f} razy" })
                .SetPlotOptions(new PlotOptions
                {
                    Pie = new PlotOptionsPie
                    {
                        AllowPointSelect = true,
                        Cursor = Cursors.Pointer,
                        DataLabels = new PlotOptionsPieDataLabels
                        {
                            Enabled = false,
                            Format = "<b>{point.name}</b>: {point.y:.0f}",
                            Style = "color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'"
                        },
                        ShowInLegend = true
                    }
                })
                .SetSeries(new Series
                {
                    Type = ChartTypes.Pie,
                    Name = "Rozkład typów",
                    Data = new Data(new object[]
                    {
                                    new object[] { data[0].Name, data[0].Number },
                                    new object[] { data[1].Name, data[1].Number },
                                    new object[] { data[2].Name, data[2].Number }
                    })
                });

            return View(chart);
        }
        public ActionResult _MatchScores(int matchId)
        {
            var bets = db.Bets.Where(d => d.MatchId == matchId);
            List<string> scores = new List<string>();

            foreach (var b in bets)
            {
                scores.Add(b.HomeScore.ToString() + " - " + b.AwayScore.ToString());
            }



            string[] categories = scores.Distinct().ToArray();
            List<object[]> pts = new List<object[]>();

            foreach (string c in categories)
            {
                pts.Add(new object[] { c, scores.Where(d => d == c).Count() });
            }

            List<DotNet.Highcharts.Options.Point> p = new List<DotNet.Highcharts.Options.Point>();

            Highcharts chart = new Highcharts("chartMS")
                .InitChart(new Chart { PlotBackgroundColor = null, PlotBorderWidth = null, PlotShadow = false })
                .SetTooltip(new Tooltip { PointFormat = "{series.name} : <b>{point.y:.0f}</b>" })
                .SetTitle(new Title { Text="Obstawione wyniki"})
                .SetPlotOptions(new PlotOptions
                {
                    Pie = new PlotOptionsPie
                    {
                        AllowPointSelect = true,
                        Cursor = Cursors.Pointer,
                        DataLabels = new PlotOptionsPieDataLabels
                        {
                            Enabled = false,
                            Format = "<b>{point.name}</b>",
                            Style = "color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'"
                        },
                        ShowInLegend = true
                    }
                })
                .SetSeries(new Series
                {
                    Type = ChartTypes.Pie,
                    Name = "Ilość wystąpień",
                    Data = new Data(pts.ToArray())
                });

            return View(chart);
        }
        public ActionResult _MatchOver(int matchId)
        {
            var bets = db.Bets.Where(d => d.MatchId == matchId);  
            string[] categories = new string[] { "over 2.5", "under 2.5" };

            List<DotNet.Highcharts.Options.Point> p = new List<DotNet.Highcharts.Options.Point>();

            Highcharts chart = new Highcharts("chartOV")
                .InitChart(new Chart { PlotBackgroundColor = null, PlotBorderWidth = null, PlotShadow = false })
                .SetTooltip(new Tooltip { PointFormat = "{series.name} : <b>{point.y:.0f}</b>" })
                .SetTitle(new Title { Text = "Powyżej/poniżej 2.5" })
                .SetPlotOptions(new PlotOptions
                {
                    Pie = new PlotOptionsPie
                    {
                        AllowPointSelect = true,
                        Cursor = Cursors.Pointer,
                        DataLabels = new PlotOptionsPieDataLabels
                        {
                            Enabled = false,
                            Format = "<b>{point.name}</b>",
                            Style = "color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'"
                        },
                        ShowInLegend = true
                    }
                })
                .SetSeries(new Series
                {
                    Type = ChartTypes.Pie,
                    Name = "Ilość wystąpień",
                    Data = new Data(new object[]
                    {
                                    new object[] { categories[0], bets.Where(c => c.HomeScore + c.AwayScore > 2).Count() },
                                    new object[] { categories[1], bets.Where(c => c.HomeScore + c.AwayScore < 3).Count() }
                    })
                });

            return View(chart);
        }
        public ActionResult _MatchResult(int matchId)
        {
            var bets = db.Bets.Where(d => d.MatchId == matchId);

            string homeName = db.Matches.Single(c => c.Id == matchId).HomeTeam.LongName;
            string awayName = db.Matches.Single(c => c.Id == matchId).AwayTeam.LongName;

            string[] categories = new string[] { homeName, "Remis", awayName };

            Highcharts chart = new Highcharts("chartMR")
                .InitChart(new Chart { PlotBackgroundColor = null, PlotBorderWidth = null, PlotShadow = false })
                .SetTooltip(new Tooltip { PointFormat = "{series.name} : <b>{point.y:.0f}</b>" })
                .SetTitle(new Title { Text = "Obstawione rezultaty" })
                .SetPlotOptions(new PlotOptions
                {
                    Pie = new PlotOptionsPie
                    {
                        AllowPointSelect = true,
                        Cursor = Cursors.Pointer,
                        DataLabels = new PlotOptionsPieDataLabels
                        {
                            Enabled = false,
                            Format = "<b>{point.name}</b>",
                            Style = "color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'"
                        },
                        ShowInLegend = true
                    }
                })
                .SetSeries(new Series
                {
                    Type = ChartTypes.Pie,
                    Name = "Ilość wystąpień",
                    Data = new Data(new object[]
                    {
                                    new object[] { categories[0], bets.Where(c => c.Result == 1).Count() },
                                    new object[] { categories[1], bets.Where(c => c.Result == 0).Count() },
                                    new object[] { categories[2], bets.Where(c => c.Result == 2).Count() }
                    })
                });

            return View(chart);
        }

    }
}