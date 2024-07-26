using System.ComponentModel.DataAnnotations;

namespace Lagerverwaltung.Database
{
    public class Sale
    {
        [Key]
        public int Id { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; }

        public decimal ActualSalePrice { get; set; }

        public DateTime SaleDate { get; set; }
    }
}
