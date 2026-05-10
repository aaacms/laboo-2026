using System;

public class UsuarioExemplo
{
    public static void Executar(string connectionString)
    {
        var dao = new UsuarioDao(connectionString);
        var entidade = new Usuario();

        entidade.Nome = "Nome 786";
        entidade.Email = "Email 471";

        dao.Insert(entidade);
        Console.WriteLine("Operação de exemplo para Usuario executada.");
    }
}
