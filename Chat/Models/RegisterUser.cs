using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Chat.Models
{
    public class RegisterUser
    {
        [DisplayName("E-Mail-Adresse")]
        [Required(ErrorMessage = "Die E-Mail-Adresse darf nicht leer sein.")]
        [MaxLength(200)]
        public string? Email { get; set; }

        [DisplayName("Benutzername")]
        [Required(ErrorMessage = "Der Benutzername darf nicht leer sein.")]
        [MaxLength(16, ErrorMessage = "Der Benutzetname darf maximal 16 Zeichen lang sein.")]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Der Benutzername darf nur aus Buchstaben und Zahlen bestehen.")]
        public string? Username { get; set; }

        [DisplayName("Passwort")]
        [Required(ErrorMessage = "Das Passwort darf nicht leer sein.")]
        public string? Password { get; set; }

        [DisplayName("Passwort wiederholen")]
        [Required(ErrorMessage = "Das Passwort muss wiederholt werden")]
        [Compare(nameof(Password), ErrorMessage = "Die Passwörter stimmen nicht überein.")]
        public string? PasswordRepeat { get; set; }

        [DisplayName("Kurzbeschreibung")]
        [MaxLength(300, ErrorMessage = "Die Kurzbeschreibung darf maximal 300 Zeichen lang sein.")]
        public string? Description { get; set; }
    }
}
