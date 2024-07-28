using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Lagerverwaltung.Database
{
    public class Sale
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Kunde")]
        public int CustomerId { get; set; }
        [DisplayName("Kunde")]
        public Customer Customer { get; set; }

        [DisplayName("Produkt")]
        public int ProductId { get; set; }
        [DisplayName("Kunde")]
        public Product Product { get; set; }

        [DisplayName("Anzahl")]
        public int Quantity { get; set; }

        [DisplayName("Verkaufspreis")]
        public decimal ActualSalePrice { get; set; }

        [DisplayName("Verkaufsdatum")]
        public DateTime SaleDate { get; set; }
    }
}
