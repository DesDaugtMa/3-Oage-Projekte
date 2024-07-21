using System.ComponentModel.DataAnnotations;

namespace Chat.Database
{
    public class Tag
    {
        public int Id { get; set; }

        [MaxLength(17)]
        public string Name { get; set; }

        public ICollection<Message>? Messages { get; set; }
    }
}
