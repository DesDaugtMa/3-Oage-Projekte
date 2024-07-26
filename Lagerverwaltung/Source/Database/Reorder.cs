using System.ComponentModel.DataAnnotations;

namespace Lagerverwaltung.Database
{
    public class Reorder
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int DeliveryCompanyId { get; set; }
        public DeliveryCompany DeliveryCompany { get; set; }

        public int Quantity { get; set; }

        public DateTime ReorderDate { get; set; }

        public DateTime EstimatedDeliveryDate { get; set; }

        public bool DidArrive { get; set; }
    }
}
