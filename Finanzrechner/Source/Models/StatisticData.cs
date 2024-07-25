namespace Finanzrechner.Models
{
    public class StatisticData
    {
        public int CountOfIntakes { get; set; }
        public decimal SumOfIntakes { get; set; }
        public int CountOfOuttakes { get; set; }
        public decimal SumOfOuttakes { get; set; }

        public List<ColumnChartData> Top5Instakes { get; set; } = new List<ColumnChartData>();
    }

    public class ColumnChartData 
    {
        public string name { get; set; }
        public double y { get; set; }
    }
}
