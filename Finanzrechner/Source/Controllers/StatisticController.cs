using Finanzrechner.Database;
using Finanzrechner.Models;
using Highsoft.Web.Mvc.Charts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Finanzrechner.Controllers
{
    public class StatisticController : Controller
    {
        private readonly DatabaseContext _context;

        public StatisticController(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(DateTime? dateFrom, DateTime? dateTo)
        {
            List<Transaction> transactions = _context.Transactions.ToList();

            if (dateFrom is not null)
            {
                ViewBag.DateFromFilter = dateFrom.Value.ToString("yyyy-MM-dd");
                ViewBag.ShowDeleteFilterButton = true;
                transactions = transactions.Where(x => x.TimeStamp >= dateFrom).ToList();
            }

            if (dateTo is not null)
            {
                ViewBag.DateToFilter = dateTo.Value.ToString("yyyy-MM-dd");
                ViewBag.ShowDeleteFilterButton = true;
                transactions = transactions.Where(x => x.TimeStamp <= dateTo).ToList();
            }



            StatisticData statisticData = new();
            statisticData.CountOfIntakes = transactions.Where(x => x.IsIntake == true).Count();
            statisticData.SumOfIntakes = transactions.Where(x => x.IsIntake == true).Sum(x => x.Amount);
            statisticData.CountOfOuttakes = transactions.Where(x => x.IsIntake == false).Count();
            statisticData.SumOfOuttakes = transactions.Where(x => x.IsIntake == false).Sum(x => x.Amount);



            List<Transaction> top5intakeTransactions = transactions.Where(x => x.IsIntake == true).OrderByDescending(x => x.Amount).Take(5).ToList();
            List<Transaction> top5outtakeTransactions = transactions.Where(x => x.IsIntake == false).OrderByDescending(x => x.Amount).Take(5).ToList();

            statisticData.Top5Outtakes.Series.Add(new ColumnChartSeries
            {
                name = "Ausgaben"
            });

            foreach (Transaction transaction in top5outtakeTransactions)
            {
                if (transaction.Description is null)
                    transaction.Description = "KEINE BESCHREIBUNG";

                statisticData.Top5Outtakes.Categories.Add(transaction.Description);

                ColumnChartSeries series = statisticData.Top5Outtakes.Series.Where(x => x.name == "Ausgaben").First();
                series.data.Add((double)transaction.Amount);
            }

            statisticData.Top5Intakes.Series.Add(new ColumnChartSeries
            {
                name = "Einnahmen"
            });

            foreach (Transaction transaction in top5intakeTransactions)
            {
                if (transaction.Description is null)
                    transaction.Description = "KEINE BESCHREIBUNG";

                statisticData.Top5Intakes.Categories.Add(transaction.Description);

                ColumnChartSeries series = statisticData.Top5Intakes.Series.Where(x => x.name == "Einnahmen").First();
                series.data.Add((double)transaction.Amount);
            }

            return View(statisticData);
        }
    }
}
