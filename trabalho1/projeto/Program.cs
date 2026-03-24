using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("projeto/appsettings.json", optional: false, reloadOnChange: true)
    .Build();

string host = config["Database:Host"] ?? throw new Exception("Database:Host não foi definido.");
string port = config["Database:Port"] ?? throw new Exception("Database:Port não foi definido.");
string username = config["Database:Username"] ?? throw new Exception("Database:Username não foi definido.");
string password = config["Database:Password"] ?? throw new Exception("Database:Password não foi definido.");
string database = config["Database:Name"] ?? throw new Exception("Database:Name não foi definido.");

var db = new DatabaseConnection($"Host={host};Port={port};Username={username};Password={password};Database={database}");