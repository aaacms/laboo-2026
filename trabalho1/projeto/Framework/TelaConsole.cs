using System.Reflection;

namespace projeto.Framework;

public class TelaConsole
{
    private string _titulo;
    private List<Action> _acoesAssociadas;

    public TelaConsole(string titulo)
    {
        _titulo = titulo;
        _acoesAssociadas = new List<Action>();
    }

    public T MostrarFormulario<T>(bool incluirId) where T : new()
    {
        Console.WriteLine($"\n=== {_titulo} ===");

        var obj = new T();
        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in props)
        {
            if (!prop.CanWrite) continue;
            if (!incluirId && prop.Name.Equals("Id", StringComparison.OrdinalIgnoreCase)) continue;

            Console.Write($"[{_titulo}] => {prop.Name} ({prop.PropertyType.Name}): ");
            var entrada = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(entrada))
            {
                prop.SetValue(obj, null);
                continue;
            }

            var valorConvertido = Util.Converter(entrada, prop.PropertyType);
            prop.SetValue(obj, valorConvertido);
        }

        return obj;
    }

    public void MostrarMensagem(string mensagem)
    {
        Console.WriteLine($"[Sistema]: {mensagem}");
    }
}