using System;

public class AutorExemplo
{
    public static void Executar(string connectionString)
    {
        var dao = new AutorDao(connectionString);
        var entidade = new Autor();

        entidade.Nome = "Nome 207";

        dao.Insert(entidade);
        Console.WriteLine("Operação de exemplo para Autor executada.");
    }
}
