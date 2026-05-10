using System;

public class EmprestimoExemplo
{
    public static void Executar(string connectionString)
    {
        var dao = new EmprestimoDao(connectionString);
        var entidade = new Emprestimo();

        entidade.UsuarioId = 4;
        entidade.LivroId = 5;
        entidade.DataEmprestimo = new DateTime(2025, 6, 29);

        dao.Insert(entidade);
        Console.WriteLine("Operação de exemplo para Emprestimo executada.");
    }
}
