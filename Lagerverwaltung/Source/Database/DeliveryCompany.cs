using System.ComponentModel.DataAnnotations;

namespace Lagerverwaltung.Database
{
    public class DeliveryCompany
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public ICollection<Reorder> Reorders { get; set; }
    }
}
