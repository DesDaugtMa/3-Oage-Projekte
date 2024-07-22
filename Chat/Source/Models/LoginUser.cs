using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Chat.Models
{
    public class LoginUser
    {
        [DisplayName("E-Mail-Adresse oder Benutzername")]
        [Required(ErrorMessage = "Dieses Feld darf nicht leer sein")]
        public string? EmailOrUsername { get; set; }

        [DisplayName("Passwort")]
        [Required(ErrorMessage = "Das Passwort darf nicht leer sein")]
        public string? Password { get; set; }
    }
}
