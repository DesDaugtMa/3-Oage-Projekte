namespace Finanzrechner.Models
{
    public class StatisticData
    {
        public int CountOfIntakes { get; set; }
        public decimal SumOfIntakes { get; set; }
        public ColumnChartData Top5Intakes { get; set; } = new ColumnChartData();
        public int CountOfOuttakes { get; set; }
        public decimal SumOfOuttakes { get; set; }
        public ColumnChartData Top5Outtakes { get; set; } = new ColumnChartData();
    }

    public class ColumnChartData 
    {

        public List<string> Categories { get; set; } = new List<string>();
        public List<ColumnChartSeries> Series { get; set; } = new List<ColumnChartSeries>();
    }

    public class ColumnChartSeries
    {
        public string name { get; set; }
        public List<double> data { get; set; } = new List<double>();
    }
}
