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

        public async Task<IActionResult> Index()
        {
            StatisticData statisticData = new();

            await _context.Transactions.Where(x => x.IsIntake == true).OrderByDescending(x => x.Amount).Take(5).ForEachAsync(x =>
            {
                if (x.Description is null)
                    x.Description = "KEINE BESCHREIBUNG";

                statisticData.Top5Instakes.Add(new ColumnChartData
                {
                    name = x.Description,
                    y = (double)x.Amount
                });
            });


            statisticData.CountOfIntakes = _context.Transactions.Where(x => x.IsIntake == true).Count();
            statisticData.SumOfIntakes = _context.Transactions.Where(x => x.IsIntake == true).Sum(x => x.Amount);
            statisticData.CountOfOuttakes = _context.Transactions.Where(x => x.IsIntake == false).Count();
            statisticData.SumOfOuttakes = _context.Transactions.Where(x => x.IsIntake == false).Sum(x => x.Amount);

            return View(statisticData);
        }
    }
}
