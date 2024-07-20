using System.ComponentModel.DataAnnotations;

namespace Chat.Database
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        public string EMailAddress { get; set; }

        [MaxLength(16)]
        public string Username { get; set; }

        public string Password { get; set; }

        [MaxLength(300)]
        public string Description { get; set; }
    }
}
