using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chat.Database
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        [MaxLength(123)]
        public string Text { get; set; }

        [Required]
        public DateTime PostedAt { get; set; }

        public ICollection<Tag>? Tags { get; set; } = new List<Tag>();

        public ICollection<Like>? Likes { get; set; }

        [NotMapped]
        public int NumberOfLikes { get; set; }

        [NotMapped]
        public bool DidCurrentUserLike { get; set; }
    }
}
