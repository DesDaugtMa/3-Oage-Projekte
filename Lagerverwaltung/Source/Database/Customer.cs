using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [NotMapped]
        public string FullInformation
        { 
            get
            {
                if (!String.IsNullOrEmpty(this.Company))
                    return this.Firstname + " " + this.Lastname + ", " + this.Company;
                else
                    return this.Firstname + " " + this.Lastname;
            }
        }
    }
}
