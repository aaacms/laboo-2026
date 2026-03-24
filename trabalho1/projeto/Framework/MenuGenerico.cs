using System.Reflection;

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
            var opcao = LerTextoObrigatorio("Opção");

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
        var obj = LerObjetoDoConsole<T>(incluirId: false);
        dao.Insert(obj);
        Console.WriteLine("Registro inserido com sucesso.");
    }

    private static void Atualizar<T>(IDao<T> dao) where T : new()
    {
        var obj = LerObjetoDoConsole<T>(incluirId: true);
        dao.Update(obj);
        Console.WriteLine("Registro atualizado com sucesso.");
    }

    private static void Excluir<T>(IDao<T> dao) where T : new()
    {
        var id = LerIntObrigatorio("Id para excluir");
        dao.Delete(id);
        Console.WriteLine("Registro excluído com sucesso.");
    }

    private static void BuscarPorId<T>(IDao<T> dao) where T : new()
    {
        var id = LerIntObrigatorio("Id para buscar");

        var obj = dao.GetById(id);
        ImprimirObjeto(obj);
    }

    private static void ListarTodos<T>(IDao<T> dao) where T : new()
    {
        var lista = dao.GetAll();
        foreach (var item in lista)
        {
            ImprimirObjeto(item);
            Console.WriteLine(new string('-', 30));
        }

        if (lista.Count == 0)
            Console.WriteLine("Nenhum registro encontrado.");
    }

    private static T LerObjetoDoConsole<T>(bool incluirId) where T : new()
    {
        var obj = new T();
        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in props)
        {
            if (!prop.CanWrite) continue;
            if (!incluirId && prop.Name.Equals("Id", StringComparison.OrdinalIgnoreCase)) continue;

            Console.Write($"{prop.Name} ({prop.PropertyType.Name}): ");
            var entrada = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(entrada))
            {
                prop.SetValue(obj, null);
                continue;
            }

            var valorConvertido = Converter(entrada, prop.PropertyType);
            prop.SetValue(obj, valorConvertido);
        }

        return obj;
    }

    private static int LerIntObrigatorio(string rotulo)
    {
        while (true)
        {
            Console.Write($"{rotulo}: ");
            var entrada = Console.ReadLine();
            if (int.TryParse(entrada, out var valor))
            {
                return valor;
            }

            Console.WriteLine("Valor inválido. Digite um número inteiro.");
        }
    }

    private static string LerTextoObrigatorio(string rotulo)
    {
        while (true)
        {
            Console.Write($"{rotulo}: ");
            var entrada = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(entrada))
            {
                return entrada.Trim();
            }

            Console.WriteLine("Valor obrigatório.");
        }
    }

    private static object? Converter(string valor, Type tipo)
    {
        var tipoReal = Nullable.GetUnderlyingType(tipo) ?? tipo;

        if (tipoReal == typeof(string)) return valor;
        if (tipoReal == typeof(int)) return int.Parse(valor);
        if (tipoReal == typeof(long)) return long.Parse(valor);
        if (tipoReal == typeof(decimal)) return decimal.Parse(valor);
        if (tipoReal == typeof(double)) return double.Parse(valor);
        if (tipoReal == typeof(float)) return float.Parse(valor);
        if (tipoReal == typeof(bool)) return bool.Parse(valor);
        if (tipoReal == typeof(DateTime)) return DateTime.Parse(valor);

        return Convert.ChangeType(valor, tipoReal);
    }

    private static void ImprimirObjeto<T>(T obj)
    {
        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var prop in props)
        {
            var valor = prop.GetValue(obj);
            Console.WriteLine($"{prop.Name}: {valor}");
        }
    }
}