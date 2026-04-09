using System.Reflection;

namespace projeto.Framework; 

public static class Util
{
    // Método para ler inteiros do console com segurança e validação
    public static int LerIntObrigatorio(string rotulo)
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

    // Método para ler textos garantindo que não fiquem vazios
    public static string LerTextoObrigatorio(string rotulo)
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

    // Método para tratar a conversão de strings do console para os tipos reais da Entidade
    public static object? Converter(string valor, Type tipo)
    {
        var tipoReal = Nullable.GetUnderlyingType(tipo) ?? tipo;

        if (tipoReal == typeof(string)) return valor;
        if (tipoReal == typeof(int)) return int.Parse(valor);
        if (tipoReal == typeof(long)) return long.Parse(valor);
        if (tipoReal == typeof(decimal)) return decimal.Parse(valor);
        if (tipoReal == typeof(double)) return double.Parse(valor);
        if (tipoReal == typeof(float)) return float.Parse(valor);
        if (tipoReal == typeof(bool)) return bool.Parse(valor);
        if (tipoReal == typeof(DateOnly)) return DateOnly.ParseExact(valor, "yyyy-MM-dd");

        return Convert.ChangeType(valor, tipoReal);
    }

    public static void ImprimirObjeto<T>(T obj)
    {
        if (obj == null) return;

        var props = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var prop in props)
        {
            var valor = prop.GetValue(obj);
            Console.WriteLine($"{prop.Name}: {valor}");
        }
    }
}