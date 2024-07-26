using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Lagerverwaltung.Database
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(5000)]
        [DisplayName("Beschreibung")]
        public string Description { get; set; }

        [DisplayName("Lagerbestand")]
        public int InStorage { get; set; } = 0;

        [DisplayName("Aktueller Verkaufspreis")]
        public decimal CurrentSellPrice { get; set; }

        [DisplayName("Kategorie")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public ICollection<Sale> Sales { get; set; }
        public ICollection<Reorder> Reorders { get; set; }
    }
}
