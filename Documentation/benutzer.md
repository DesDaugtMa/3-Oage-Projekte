# Der Applikation Benutzer hinzufügen

## Program.cs

Damit alles gscheit Funktioniert, diesen Code in die *Program.cs* hinzufügen:

```C#
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultForbidScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
});

builder.Services.AddAuthorization();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

...

app.UseAuthentication();
app.UseAuthorization();
```

## Models

*UserLogin.cs* in Models-Folder erstellen und folgende Properties erstellen:

```C#
[DisplayName("E-Mail-Adresse oder Benutzername")]
[Required(ErrorMessage = "Dieses Feld darf nicht leer sein")]
public string? EmailOrUsername { get; set; }

[DisplayName("Passwort")]
[Required(ErrorMessage = "Das Passwort darf nicht leer sein")]
public string? Password { get; set; }
```

*UserRegister.cs* in Models-Folder erstellen und folgende Properties erstellen:

```C#
[DisplayName("E-Mail-Adresse")]
[Required(ErrorMessage = "Die E-Mail-Adresse darf nicht leer sein.")]
[MaxLength(200)]
public string? Email { get; set; }

[DisplayName("Benutzername")]
[Required(ErrorMessage = "Der Benutzername darf nicht leer sein.")]
[MaxLength(16, ErrorMessage = "Der Benutzetname darf maximal 16 Zeichen lang sein.")]
[RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Der Benutzername darf nur aus Buchstaben und Zahlen bestehen.")]
public string? Username { get; set; }

[DisplayName("Passwort")]
[Required(ErrorMessage = "Das Passwort darf nicht leer sein.")]
public string? Password { get; set; }

[DisplayName("Passwort wiederholen")]
[Required(ErrorMessage = "Das Passwort muss wiederholt werden")]
[Compare(nameof(Password), ErrorMessage = "Die Passwörter stimmen nicht überein.")]
public string? PasswordRepeat { get; set; }

[DisplayName("Kurzbeschreibung")]
[MaxLength(300, ErrorMessage = "Die Kurzbeschreibung darf maximal 300 Zeichen lang sein.")]
public string? Description { get; set; }
```

## Methoden

Dem User-Model aus der Datenbank müssen folgende Methoden hinzugefügt werden, um richtig mit dem Passwort umzugehen:

```C#
public void GenerateHashedPassword(string passwordToHash)
{
    var hasher = new PasswordHasher<IdentityUser>();
    var identityUser = new IdentityUser(EMailAddress);
    Password = hasher.HashPassword(identityUser, passwordToHash);
}

public bool CheckPassword(string passwordToCheck)
{
    var hasher = new PasswordHasher<IdentityUser>();
    var identityUser = new IdentityUser(EMailAddress);
    return PasswordVerificationResult.Failed != hasher.VerifyHashedPassword(identityUser, Password, passwordToCheck);
}
```

## Controller

Neuen Controller *AccountController.cs* erstellen. Folgende Logik für Account erstellen und Login:

```C#
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
            Description = userToRegister.Description
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
```
## Views

### LoginView

Neue Ansicht für Login erstellen mit folgender Struktur:

```cshtml
@model Chat.Models.LoginUser

@{
    ViewData["Title"] = "Anmelden";
}

<h1>Anmelden</h1>

<div class="row">
    <p class="text-danger">@ViewBag.LoginError</p>
    <div class="col-md-4">
        <form asp-action="Login">
            <div asp-validation-summary="ModelOnly" class="text-danger"></div>
            <div class="form-group">
                <label asp-for="EmailOrUsername" class="control-label"></label>
                <input asp-for="EmailOrUsername" class="form-control" />
                <span asp-validation-for="EmailOrUsername" class="text-danger"></span>
            </div>
            <div class="form-group">
                <label asp-for="Password" class="control-label"></label>
                <input asp-for="Password" class="form-control" type="password"/>
                <span asp-validation-for="Password" class="text-danger"></span>
            </div>
            <div class="form-group">
                <input type="submit" value="Anmelden" class="btn btn-success" />
            </div>
        </form>
    </div>
</div>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}
```

### RegisterView

Neue Ansicht für Registrieren erstellen mit folgender Struktur:

```cshtml
@model Chat.Models.RegisterUser

@{
    ViewData["Title"] = "Registrieren";
}

<h1>Registrieren</h1>

<div class="row">
    <div class="col-md-4">
        <form asp-action="Register">
            <div asp-validation-summary="ModelOnly" class="text-danger"></div>
            <div class="form-group">
                <label asp-for="Email" class="control-label"></label>
                <input asp-for="Email" class="form-control" />
                <span asp-validation-for="Email" class="text-danger"></span>
            </div>
            <div class="form-group">
                <label asp-for="Username" class="control-label"></label>
                <input asp-for="Username" class="form-control" />
                <span asp-validation-for="Username" class="text-danger"></span>
            </div>
            <div class="form-group">
                <label asp-for="Password" class="control-label"></label>
                <input asp-for="Password" class="form-control" type="password"/>
                <span asp-validation-for="Password" class="text-danger"></span>
            </div>
            <div class="form-group">
                <label asp-for="PasswordRepeat" class="control-label"></label>
                <input asp-for="PasswordRepeat" class="form-control" type="password"/>
                <span asp-validation-for="PasswordRepeat" class="text-danger"></span>
            </div>
            <div class="form-group">
                <label asp-for="Description" class="control-label"></label>
                <textarea asp-for="Description" class="form-control" rows="3"></textarea>
                <span asp-validation-for="Description" class="text-danger"></span>
            </div>
            <div class="form-group">
                <input type="submit" value="Account erstellen" class="btn btn-success" />
            </div>
        </form>
    </div>
</div>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}
```

### PartialView

Eine PartialView *_AccountPartial.cshtml* erstellen, um die Links einzufügen.

```cshtml
@inject Microsoft.AspNetCore.Http.IHttpContextAccessor HttpContextAccessor;
<ul class="navbar-nav">
    @if (HttpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
    {
        <li class="nav-item">
            <a class="nav-link text-dark" asp-controller="Person" asp-action="Profil" title="Profil">Angemeldet als @User.Identity.Name</a>
        </li>

        <li class="nav-item">
            <form asp-controller="Account" asp-action="Logout" method="post" id="logoutForm">
                <button type="submit" class="nav-link btn btn-link text-danger">Abmelden</button>
            </form>
        </li>
    }
    else
    {
        <li class="nav-item">
            <a class="nav-link text-dark" asp-area="" asp-controller="Account" asp-action="Register">Registrieren</a>
        </li>
        <li class="nav-item">
            <a class="nav-link text-dark" asp-area="" asp-controller="Account" asp-action="Login">Anmelden</a>
        </li>
    }
</ul>
```

Diese PartialView dann im _Layout.cshtml hinzufügen:

```cstml
<header>
    <nav class="navbar navbar-expand-sm navbar-toggleable-sm navbar-light bg-white border-bottom box-shadow mb-3">
        <div class="container-fluid">
            <a class="navbar-brand" asp-area="" asp-controller="Home" asp-action="Index">Chat</a>
            <div class="d-sm-inline-flex justify-content-between">
                <ul class="navbar-nav flex-grow-1"></ul>
                <partial name="_AccountPartial" />
            </div>
        </div>
    </nav>
</header>
```