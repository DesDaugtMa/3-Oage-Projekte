using Finanzrechner.Database;

namespace Finanzrechner.Models
{
    public class SwitchTransactionsModel
    {
        public Category Category { get; set; }
        public int FromCategoryId { get; set; }
        public int ToCategoryId { get; set; }
    }
}
