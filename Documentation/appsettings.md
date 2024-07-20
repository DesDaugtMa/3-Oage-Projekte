# AppSettings
Neuen Folder *Config* erstellen und Klasse *AppSettings.cs* erstellen. *appsettings.json*, *appsettings.Development.json* und *appsettings.Production.json* ebenfalls im *Config*-Folder erstellen.

Folgenden Code in *Program.cs* hinzufügen:
```C#
IHostBuilder hostBuilder = builder.Host.ConfigureAppConfiguration(configurationBuilder =>
{
    configurationBuilder.Sources.Clear();
    configurationBuilder.AddJsonFile($"Config/appsettings.json", false);
    configurationBuilder.AddJsonFile($"Config/appsettings.{builder.Environment.EnvironmentName}.json", true);
    configurationBuilder.AddEnvironmentVariables();
    configurationBuilder.AddUserSecrets<Program>(true);
});

AppSettings appSettings = builder.Configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();
```

Inhalt der *appsettings.Development.json*:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "DEFINED IN USER SECRETS / ENVIRONMENT VARIABLE"
  },
  "AppSettings": {
    
  }
}
```

Inhalt der *secrets.json* (Rechtsklick auf Projekt -> 'Manage User Secrets'/'Geheime Benutzerschlüssel verwalten'):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BudgetManager;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False"
  },
  "AppSettings": {
    
  }
}
```

AppSettings in Controller verwenden:
```C#
private readonly AppSettings _appSettings;

public AdminController(IConfiguration appSettings)
{
    _appSettings = appSettings.GetSection(nameof(AppSettings)).Get<AppSettings>();
}
```