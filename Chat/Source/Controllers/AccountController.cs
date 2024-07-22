using Chat.Config;
using Chat.Database;
using Chat.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Chat.Controllers
{
    public class AccountController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly AppSettings _appSettings;

        public AccountController(DatabaseContext context, IConfiguration configuration)
        {
            _context = context;
            _appSettings = configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();
        }

        public IActionResult Register()
        {
            return View();
        }

        [BindProperty]
        public RegisterUser userToRegister { get; set; }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string returnUrl = null)
        {
            try
            {
                if (_context.Users.Any(x => x.EMailAddress == userToRegister.Email))
                {
                    ModelState.AddModelError("Email", "Diese E-Mail-Adresse wird bereits verwendet.");
                    return RedirectToAction("Register", "Account");
                }

                if (_context.Users.Any(x => x.Username == userToRegister.Username))
                {
                    ModelState.AddModelError("Username", "Dieser Benutzername wird bereits verwendet.");
                    return RedirectToAction("Register", "Account");
                }

                User newUser = new User
                {
                    EMailAddress = userToRegister.Email,
                    Username = userToRegister.Username,
                    Description = userToRegister.Description,
                    MemberSince = DateTime.Now
                };

                newUser.GenerateHashedPassword(userToRegister.Password!);

                _context.Users.Add(newUser);
                _context.SaveChanges();

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, newUser.Id.ToString()),
                    new Claim(ClaimTypes.Email, newUser.EMailAddress!),
                    new Claim(ClaimTypes.Name, newUser.Username!)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Email", "Es ist ein Fehler aufgetreten");
                return RedirectToAction("Register", "Account");
            }
        }

        public IActionResult Login()
        {
            return View();
        }

        [BindProperty]
        public LoginUser userToLogIn { get; set; }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            User? userFromDatabase = null;

            if (_context.Users.Any(u => u.EMailAddress == userToLogIn.EmailOrUsername))
                userFromDatabase = _context.Users.Where(u => u.EMailAddress == userToLogIn.EmailOrUsername).FirstOrDefault();

            if (_context.Users.Any(u => u.Username == userToLogIn.EmailOrUsername))
                userFromDatabase = _context.Users.Where(u => u.Username == userToLogIn.EmailOrUsername).FirstOrDefault();

            if (userFromDatabase is not null && userFromDatabase.CheckPassword(userToLogIn.Password!))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userFromDatabase.Id.ToString()),
                    new Claim(ClaimTypes.Email, userFromDatabase.EMailAddress!),
                    new Claim(ClaimTypes.Name, userFromDatabase.Username!)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home");
                
            }

            ViewBag.LoginError = "E-Mail, Benutzername oder Passwort ist nicht korrekt";
            return View();
        }

        public async Task<IActionResult> LogOut()
        {
            Task.Run(async () => await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme)).Wait();
            return Redirect("~/");
        }

        public IActionResult Profile(string username)
        {
            User user = _context.Users.Where(x => x.Username == username).First();
            List<Message> messagesOfUser = _context.Messages.Where(x => x.User == user).ToList();

            int likesGoten = 0;

            foreach (Message message in messagesOfUser)
            {
                likesGoten += _context.Likes.Where(x => x.MessageId == message.Id).Count();
            }

            Profile userProfile = new Profile
            {
                LastMessages = _context.Messages.Where(x => x.User == user).OrderByDescending(x => x.PostedAt).Take(5).ToList(),
                Username = user.Username,
                Description = user.Description,
                MemberSince = user.MemberSince,
                CountOfMessages = _context.Messages.Where(x => x.User == user).Count(),
                LikesGiven = _context.Likes.Where(x => x.User == user).Count(),
                LikesGoten = likesGoten
            };

            return View(userProfile);
        }
    }
}
