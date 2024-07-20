# Code First

Vorraussetzung dafür ist die *AppSettings.md*, weil der Connectionstring für die Datenbank aus der AppSettings-Datei geholt wird.

## Pakete

 - Microsoft.EntityFrameworkCore
 - Microsoft.EntityFrameworkCore.Tools
 - Microsoft.EntityFrameworkCore.Design
 - Microsoft.EntityFrameworkCore.SqlServer

 ## Models & Context

Die Datenbankmodels und den DbContext 'DatabaseContext' in einem neuen Folder erstellen. z.B. 'Database'-Folder

Code im Context:
```C#
public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions options) : base(options) { }

    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        
    }
}
```

Den DbContext in der *Program.cs* registrieren:
```C#
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

## Migrations

Migration erstellen:
```bash
add-migration AddBookToDb
```

Migartion auf Datenbank spielen:
```bash
update-database <optionaler migrationnae>
```

Migration entfernen:
```bash
remove-migration
```

## Context in Controller verwenden

```C#
private readonly DatabaseContext _context;

public AdminController(DatabaseContext context)
{
    _context = context;
}
```