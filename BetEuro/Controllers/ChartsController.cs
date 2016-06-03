using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotNet.Highcharts;
using DotNet.Highcharts.Options;
using DotNet.Highcharts.Helpers;
using System.Drawing;
using System.Globalization;
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



            Highcharts chart = new Highcharts("chart")
                .InitChart(new Chart { PlotBackgroundColor = null, PlotBorderWidth = null, PlotShadow = false })
                .SetTitle(new Title { Text = "Rozkład punktów w zależności ich rodzaju" })
                .SetTooltip(new Tooltip { PointFormat = "{series.name}: <b>{point.percentage:.0f}%</b>" })
                .SetPlotOptions(new PlotOptions
                {
                    Pie = new PlotOptionsPie
                    {
                        AllowPointSelect = true,
                        Cursor = Cursors.Pointer,
                        DataLabels = new PlotOptionsPieDataLabels
                        {
                            Enabled = true,
                            Format = "<b>{point.name}</b>: {point.y:.0f}",
                            Style = "color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'"
                        }
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

    }
}