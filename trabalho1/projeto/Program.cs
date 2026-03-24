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
var conn = db.GetConnection();

var autor1 = new Autor { Nome = "Autor 1", Id = 001 };
var autor2 = new Autor { Nome = "Autor 2", Id = 002 };
var livro1 = new Livro { Titulo = "Livro 1", Id = 001, Autor_Id = 001};
var livro2 = new Livro { Titulo = "Livro 2", Id = 002, Autor_Id = 002 };
var user1 = new Usuario { Nome = "Usuario 1", Id = 001, Email = "usuario1@example.com" };
var emprestimo1 = new Emprestimo { Id = 001, Livro_Id = 001, Usuario_Id = 001, Data_emprestimo = DateTime.Now };


var daoCliente = new BaseDao<Autor>(conn);
var daoLivro = new BaseDao<Livro>(conn);
var daoUsuario = new BaseDao<Usuario>(conn);
var daoEmprestimo = new BaseDao<Emprestimo>(conn);

daoCliente.Insert(autor1);
daoCliente.Insert(autor2);
daoLivro.Insert(livro1);
daoLivro.Insert(livro2);
daoUsuario.Insert(user1);
daoEmprestimo.Insert(emprestimo1);

var menu = new Menu();
