# Der Applikation Benutzer hinzufügen

## Controller

Neuen Controller *AccountController.cs* erstellen. Folgende Logik für Account erstellen und Login:

```C#
public class AccountController : Controller
    {
        IConfiguration configuration;
        private readonly InventoryContext _context;

        public AccountController(InventoryContext context, IConfiguration configuration)
        {
            _context = context;
            this.configuration = configuration;
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Profil()
        {
            var userData = _context.TblPeople.Where(p => p.Username == User.Identity.Name).FirstOrDefault();
            return View(userData);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profil(int id, [Bind("Id,Vorname,Nachname,Username,Passwort,Email,GroupId")] Person tblPerson)
        {

            //ModelState.Remove<TblPerson>(

            if (id != tblPerson.Id)
            {
                return NotFound();
            }

            tblPerson.Group = _context.TblGroups.Where(g => g.Id == tblPerson.GroupId).FirstOrDefault();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tblPerson);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblPersonExists(tblPerson.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tblPerson);
        }

        [BindProperty]
        public LoginModel user { get; set; }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            if (_context.TblPeople.Any(u => u.Username == user.Benutzername))
            {
                // Code vorgezeigt zum verschlüsseln
                var loginuser = _context.TblPeople.Where(u => u.Username == user.Benutzername).FirstOrDefault();
                bool correctUser = false;
                if (loginuser.Passwort.Length < 30 && loginuser.Passwort.Equals(user.Kennwort))
                {
                    (string hashed, string salt) = loginuser.PasswortToHash(user.Kennwort);

                    loginuser.Passwort = hashed;
                    loginuser.Salt = salt;
                    _context.Update(loginuser);
                    _context.SaveChanges();

                }

                if (loginuser.CorrectPassword(user.Kennwort))
                {
                    string pepper = configuration["Pepper"];
                    (string hashed, string salt) = loginuser.PasswortToHash(pepper+user.Kennwort);

                    loginuser.Passwort = hashed;
                    loginuser.Salt = salt;
                    _context.Update(loginuser);
                    _context.SaveChanges();
                }

                if (loginuser.CorrectPasswordWithPepper(user.Kennwort, configuration["Pepper"]))
                {
                    var userRole = _context.TblPeople.Where(p => p.Username == user.Benutzername).Include(p => p.Group).Select(p => p.Group.Name).FirstOrDefault();

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Benutzername),  //wir sichern den Benutzername
                        new Claim(ClaimTypes.Role, userRole.ToString()),             //und er ist User, hier kann auch weitere Rollen hinzufefügt werdem
                        //z.B.
                        //new Claim(ClaimTypes.Role, "Administrator"),
                    };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                    };
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                    ViewBag.LoginError = false;

                    if (returnUrl != null)
                        return Redirect(returnUrl);
                    else
                        return Redirect("~/");


                }
            }
            ViewBag.LoginError = true;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("~/");
        }

        private bool TblPersonExists(int id)
        {
            return _context.TblPeople.Any(e => e.Id == id);
        }


    }

```


Controller gscheit machen
UserRegister Model
View dafür erstellen
in Program.cs die auth-dinger
userLogin model -> allles auch dafür
partial view
httpcontextaccessor in program.cs
logout