using System;

public class LivroExemplo
{
    public static void Executar(string connectionString)
    {
        var dao = new LivroDao(connectionString);
        var entidade = new Livro();

        entidade.Titulo = "Titulo 245";
        entidade.AutorId = 2;

        dao.Insert(entidade);
        Console.WriteLine("Operação de exemplo para Livro executada.");
    }
}
