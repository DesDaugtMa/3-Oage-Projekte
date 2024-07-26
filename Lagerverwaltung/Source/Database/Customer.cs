using System.ComponentModel.DataAnnotations;

namespace Lagerverwaltung.Database
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Firstname { get; set; }

        [Required]
        public string Lastname { get; set; }

        public string? EMailAddress { get; set; }

        public string? Company { get; set; }

        public ICollection<Sale> Sales { get; set; }
    }
}
