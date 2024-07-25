using Chat.Config;
using Chat.Database;
using Chat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Chat.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppSettings _appSettings;
        private readonly DatabaseContext _context;

        public HomeController(ILogger<HomeController> logger, IConfiguration appSettings, DatabaseContext context)
        {
            _logger = logger;
            _appSettings = appSettings.GetSection(nameof(AppSettings)).Get<AppSettings>();
            _context = context;
        }

        public IActionResult Index()
        {
            IndexMessages indexMessages = new IndexMessages();

            if (User.FindFirstValue(ClaimTypes.NameIdentifier) is not null)
                indexMessages.AlreadySentMessages = _context.Messages.Include(x => x.User).OrderByDescending(x => x.PostedAt).Take(10).ToList();
            else
                indexMessages.AlreadySentMessages = _context.Messages.Include(x => x.User).OrderByDescending(x => x.PostedAt).Take(5).ToList();

            foreach (var message in indexMessages.AlreadySentMessages)
            {
                message.NumberOfLikes = _context.Likes.Where(x => x.MessageId == message.Id).Count();

                if (User.FindFirstValue(ClaimTypes.NameIdentifier) is null)
                    continue;

                if (_context.Likes.Where(x => x.MessageId == message.Id).Any(x => x.UserId == Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)))
                    message.DidCurrentUserLike = true;
                else
                    message.DidCurrentUserLike = false;
            }

            List<Message> messagesLast24Hours = _context.Messages.Where(x => x.PostedAt >= DateTime.Now.AddHours(-24)).Include(x => x.Tags).ToList();
            
            List<string> topHashtags = new List<string>();
            foreach (var message in messagesLast24Hours)
            {
                foreach(var tag in message.Tags)
                {
                    topHashtags.Add(tag.Name);
                }
            }

            indexMessages.TopHashtags = topHashtags.GroupBy(x => x)
              .Where(x => x.Count() > 1)
              .OrderByDescending(x => x.Count())
              .Select(x => x.Key)
              .Take(5)
              .ToList();

            return View(indexMessages);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> PostMessage(IndexMessages messages)
        {
            if(!String.IsNullOrWhiteSpace(messages.Text) && !String.IsNullOrEmpty(messages.Text))
            {
                Message newMessage = new Message
                {
                    UserId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
                    Text = messages.Text,
                    PostedAt = DateTime.Now,
                };

                Regex regex = new Regex(@"(^|\W)#([a-zA-z0-9]{3,16})");
                MatchCollection matchedHashtags = regex.Matches(newMessage.Text);

                List<string> hashtags = new List<string>();

                for (int i = 0; i < matchedHashtags.Count; i++)
                {
                    if (matchedHashtags[i].Value[0] == ' ')
                        hashtags.Add(matchedHashtags[i].Value.Remove(0, 1));
                    else
                        hashtags.Add(matchedHashtags[i].Value);
                }

                foreach (string hashtag in hashtags)
                {
                    if (!_context.Tags.Any(x => x.Name == hashtag.ToLower()))
                        _context.Tags.Add(new Tag
                        {
                            Name = hashtag.ToLower()
                        });
                }

                _context.Messages.Add(newMessage);
                _context.SaveChanges();

                foreach (string hashtag in hashtags)
                {
                    newMessage.Tags.Add(_context.Tags.Where(x => x.Name == hashtag.ToLower()).First());
                }

                _context.SaveChanges();
            }

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public async Task<IActionResult> LikeOrDislike(int id)
        {
            Like like = _context.Likes.Where(x => x.MessageId == id && x.UserId == Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)).FirstOrDefault()!;

            if (like is null)
            {
                _context.Likes.Add(new Like { MessageId = id, UserId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!) });
            } else
            {
                _context.Likes.Remove(like);
            }

            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public string GetLastMessages()
        {
            int amountOfMessages = 0;

            if (User.FindFirstValue(ClaimTypes.NameIdentifier) is not null)
                amountOfMessages = 10;
            else
                amountOfMessages = 5;

            List<Message> messages = _context.Messages.Include(x => x.User).OrderByDescending(x => x.PostedAt).Take(amountOfMessages).ToList();

            foreach (Message message in messages)
            {
                message.DateTimeFormated = message.PostedAt.ToString("dd.MM.yyyy HH:mm:ss");

                if (amountOfMessages == 5)
                    message.IsUserLoggedIn = false;
                else
                {
                    message.IsUserLoggedIn = true;

                    if (_context.Likes.Where(x => x.MessageId == message.Id).Any(x => x.UserId == Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)))
                        message.DidCurrentUserLike = true;
                    else
                        message.DidCurrentUserLike = false;
                }

                message.NumberOfLikes = _context.Likes.Where(x => x.MessageId == message.Id).Count();
            }

            return JsonConvert.SerializeObject(
                messages, 
                Formatting.None, 
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }
            );
        }
    }
}
