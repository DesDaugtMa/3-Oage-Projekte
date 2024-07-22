using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Finanzrechner.Database
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(7)]
        [DisplayName("Farbe")]
        public string ColorCode { get; set; }

        public ICollection<Transaction>? Transactions { get; set; }
    }
}
