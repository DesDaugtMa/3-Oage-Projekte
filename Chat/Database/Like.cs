﻿namespace Chat.Database
{
    public class Like
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int MessageId { get; set; }
        public Message Message { get; set; }
    }
}
