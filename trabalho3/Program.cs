using System.Threading.Tasks;
using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.Extensions.Configuration;
using Npgsql;

var config = new ConfigurationBuilder()
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

string host = config["Database:Host"] ?? throw new Exception("Database:Host não foi definido.");
string port = config["Database:Port"] ?? throw new Exception("Database:Port não foi definido.");
string username = config["Database:Username"] ?? throw new Exception("Database:Username não foi definido.");
string password = config["Database:Password"] ?? throw new Exception("Database:Password não foi definido.");

string apiKey = config["GOOGLE_GEMINI_API_KEY"] ?? throw new Exception("GOOGLE_GEMINI_API_KEY não foi definido.");

var db = new DatabaseConnection($"Host={host};Port={port};Username={username};Password={password};");

await GenerateContentSimpleText.RunAsync(apiKey);

public class GenerateContentSimpleText 
{
    public static async Task RunAsync(string key) 
    {
        var client = new Client(apiKey: key);
        var prompt = Console.ReadLine();
        var response = await client.Models.GenerateContentAsync(
            model: "gemini-3-flash-preview", 
            contents: $"Preciso que você gere apenas um SQL com base no seguinte prompt: {prompt}, sem explicações, apenas o código SQL." 
        );

        var text = response.Candidates?[0].Content?.Parts?[0].Text;
        
        Console.WriteLine(text ?? "Nenhum conteúdo retornado.");
    }
}