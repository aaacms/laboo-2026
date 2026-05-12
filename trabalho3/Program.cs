using System.Threading.Tasks;
using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

string host = config["Database:Host"] ?? throw new Exception("Database:Host não foi definido.");
string port = config["Database:Port"] ?? throw new Exception("Database:Port não foi definido.");
string username = config["Database:Username"] ?? throw new Exception("Database:Username não foi definido.");
string password = config["Database:Password"] ?? throw new Exception("Database:Password não foi definido.");
string database = config["Database:Name"] ?? throw new Exception("Database:Name não foi definido.");

var db = new DatabaseConnection($"Host={host};Port={port};Username={username};Password={password};Database={database}");

public class GenerateContentSimpleText {
  public static async Task main() {
    // The client gets the API key from the environment variable `GOOGLE_API_KEY`.
    var client = new Client();
    var response = await client.Models.GenerateContentAsync(
      model: "gemini-3-flash-preview", contents: "Explain how AI works in a few words"
    );
    Console.WriteLine(response.Candidates[0].Content.Parts[0].Text);
  }
}