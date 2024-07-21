using Chat.Database;

namespace Chat.Models
{
    public class Profile
    {
        public List<Message> LastMessages { get; set; }
        public string Username { get; set; }
        public string Description { get; set; }
        public DateTime MemberSince { get; set; }
        public int CountOfMessages { get; set; }
        public int LikesGiven { get; set; }
        public int LikesGoten { get; set; }
    }
}
