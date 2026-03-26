using System.Reflection;
using projeto.Framework;

public static class MenuGenerico
{
    private sealed record MenuItem(string Opcao, string Descricao, Action Acao);

    public static void ExecutarCrud<T>(IDao<T> dao) where T : new()
    {
        var itens = new List<MenuItem>
        {
            new("1", "Inserir", () => Inserir(dao)),
            new("2", "Atualizar", () => Atualizar(dao)),
            new("3", "Excluir", () => Excluir(dao)),
            new("4", "Buscar por Id", () => BuscarPorId(dao)),
            new("5", "Listar todos", () => ListarTodos(dao))
        };

        while (true)
        {
            DesenharMenuCrud<T>(itens);
            var opcao = Util.LerTextoObrigatorio("Opção");

            try
            {
                if (opcao == "0")
                {
                    return;
                }

                var item = itens.FirstOrDefault(i => i.Opcao == opcao);
                if (item is null)
                {
                    Console.WriteLine("Opção inválida.");
                    continue;
                }

                item.Acao();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
            }
        }
    }

    private static void DesenharMenuCrud<T>(IEnumerable<MenuItem> itens)
    {
        Console.WriteLine($"\n=== CRUD {typeof(T).Name} ===");
        foreach (var item in itens)
        {
            Console.WriteLine($"{item.Opcao} - {item.Descricao}");
        }
        Console.WriteLine("0 - Sair");
    }

    private static void Inserir<T>(IDao<T> dao) where T : new()
    {
        var tela = new TelaConsole($"Inclusão de {typeof(T).Name}");
        var obj = tela.MostrarFormulario<T>(incluirId: false);
        dao.Insert(obj);
        tela.MostrarMensagem("Registro inserido com sucesso.");
    }

    private static void Atualizar<T>(IDao<T> dao) where T : new()
    {
        var tela = new TelaConsole($"Atualização de {typeof(T).Name}");
        var obj = tela.MostrarFormulario<T>(incluirId: true);
        dao.Update(obj);
        tela.MostrarMensagem("Registro atualizado com sucesso.");
    }

    private static void Excluir<T>(IDao<T> dao) where T : new()
    {
        var id = Util.LerIntObrigatorio("Id para excluir");
        dao.Delete(id);
        Console.WriteLine("Registro excluído com sucesso.");
    }

    private static void BuscarPorId<T>(IDao<T> dao) where T : new()
    {
        var id = Util.LerIntObrigatorio("Id para buscar");

        var obj = dao.GetById(id);
        Util.ImprimirObjeto(obj);
    }

    private static void ListarTodos<T>(IDao<T> dao) where T : new()
    {
        var lista = dao.GetAll();
        foreach (var item in lista)
        {
            Util.ImprimirObjeto(item);
            Console.WriteLine(new string('-', 30));
        }

        if (lista.Count == 0)
            Console.WriteLine("Nenhum registro encontrado.");
    }
}