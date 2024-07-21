using Chat.Database;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Chat.Models
{
    public class IndexMessages
    {
        public List<Message> AlreadySentMessages { get; set; } = new List<Message>();

        public List<string> TopHashtags { get; set; } = new List<string>();

        [DisplayName("Nachricht verfassen")]
        [MaxLength(123, ErrorMessage = "Der Text darf maximal 123 Zeichen lang sein.")]
        public string? Text { get; set; }
    }
}
