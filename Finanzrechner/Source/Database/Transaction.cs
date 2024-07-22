using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Finanzrechner.Database
{
    public class Transaction
    {

        [Key]
        public int Id { get; set; }

        [MaxLength(100, ErrorMessage = "Die Beschreibung darf maximal 100 Zeichen lang sein.")]
        [DisplayName("Beschreibung")]
        public string? Description { get; set; }

        [DisplayName("Betrag")]
        [Required(ErrorMessage = "Dieses Feld darf nicht leer sein.")]
        public decimal Amount { get; set; }

        [DisplayName("Datum")]
        [Required(ErrorMessage = "Dieses Feld darf nicht leer sein.")]
        public DateTime TimeStamp { get; set; }

        [DisplayName("Kategorie")]
        public int CategoryId { get; set; }
        [DisplayName("Kategorie")]
        public Category Category { get; set; }

        public bool IsIntake { get; set; }
    }
}
