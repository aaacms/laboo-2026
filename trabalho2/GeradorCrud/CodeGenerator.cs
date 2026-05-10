

using System;
using System.IO;
using System.Text;
using System.Linq;

public class CodeGenerator
{
    public static void GenerateCode(List<TableInfo> schema, string outputDir)
    {
        if (!Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        foreach (var table in schema)
        {
            string className = ToPascalCase(table.Nome);

            GenerateEntity(table, className, outputDir);
            GenerateDao(table, className, outputDir);
            GenerateExample(table, className, outputDir);
        }

        Console.WriteLine($"\nArquivos gerados com sucesso na pasta: {outputDir}");
    }

    private static void GenerateEntity(TableInfo table, string className, string outputDir)
    {
        var sb = new StringBuilder();
        sb.AppendLine("using System;");
        sb.AppendLine();
        sb.AppendLine($"public class {className}");
        sb.AppendLine("{");

        foreach (var col in table.Columns)
        {
            string csharpType = GetCSharpType(col.Tipo);
            string propName = ToPascalCase(col.Nome);
            sb.AppendLine($"    public {csharpType} {propName} {{ get; set; }}");
        }

        sb.AppendLine("}");
        File.WriteAllText(Path.Combine(outputDir, $"{className}.cs"), sb.ToString());
    }

    private static void GenerateDao(TableInfo table, string className, string outputDir)
    {
        var pk = table.Columns.FirstOrDefault(c => c.EhChavePrimaria);
        string pkType = pk != null ? GetCSharpType(pk.Tipo) : "int";
        string pkName = pk != null ? ToPascalCase(pk.Nome) : "Id";
        string pkColumnName = pk != null ? pk.Nome : "id";

        var nonPkColumns = table.Columns.Where(c => !c.EhChavePrimaria).ToList();

        string insertCols = string.Join(", ", nonPkColumns.Select(c => c.Nome));
        string insertParams = string.Join(", ", nonPkColumns.Select(c => "@" + c.Nome));
        string updateSets = string.Join(", ", nonPkColumns.Select(c => $"{c.Nome} = @{c.Nome}"));

        var sb = new StringBuilder();
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using Npgsql; // Usando ADO.NET com PostgreSQL");
        sb.AppendLine();
        sb.AppendLine($"public class {className}Dao");
        sb.AppendLine("{");
        sb.AppendLine("    private readonly string _connectionString;");
        sb.AppendLine();
        sb.AppendLine($"    public {className}Dao(string connectionString)");
        sb.AppendLine("    {");
        sb.AppendLine("        _connectionString = connectionString;");
        sb.AppendLine("    }");
        sb.AppendLine();

        // INSERT
        sb.AppendLine($"    public void Insert({className} entity)");
        sb.AppendLine("    {");
        sb.AppendLine($"        using var conn = new NpgsqlConnection(_connectionString);");
        sb.AppendLine($"        conn.Open();");
        sb.AppendLine($"        string sql = \"INSERT INTO {table.Nome} ({insertCols}) VALUES ({insertParams})\";");
        sb.AppendLine($"        using var cmd = new NpgsqlCommand(sql, conn);");
        foreach (var col in nonPkColumns)
        {
            sb.AppendLine($"        cmd.Parameters.AddWithValue(\"@{col.Nome}\", entity.{ToPascalCase(col.Nome)} ?? (object)DBNull.Value);");
        }
        sb.AppendLine($"        cmd.ExecuteNonQuery();");
        sb.AppendLine("    }");
        sb.AppendLine();

        // UPDATE
        sb.AppendLine($"    public void Update({className} entity)");
        sb.AppendLine("    {");
        sb.AppendLine($"        using var conn = new NpgsqlConnection(_connectionString);");
        sb.AppendLine($"        conn.Open();");
        sb.AppendLine($"        string sql = \"UPDATE {table.Nome} SET {updateSets} WHERE {pkColumnName} = @{pkColumnName}\";");
        sb.AppendLine($"        using var cmd = new NpgsqlCommand(sql, conn);");
        foreach (var col in table.Columns)
        {
            sb.AppendLine($"        cmd.Parameters.AddWithValue(\"@{col.Nome}\", entity.{ToPascalCase(col.Nome)} ?? (object)DBNull.Value);");
        }
        sb.AppendLine($"        cmd.ExecuteNonQuery();");
        sb.AppendLine("    }");
        sb.AppendLine();

        // DELETE
        sb.AppendLine($"    public void Delete({pkType} id)");
        sb.AppendLine("    {");
        sb.AppendLine($"        using var conn = new NpgsqlConnection(_connectionString);");
        sb.AppendLine($"        conn.Open();");
        sb.AppendLine($"        string sql = \"DELETE FROM {table.Nome} WHERE {pkColumnName} = @id\";");
        sb.AppendLine($"        using var cmd = new NpgsqlCommand(sql, conn);");
        sb.AppendLine($"        cmd.Parameters.AddWithValue(\"@id\", id);");
        sb.AppendLine($"        cmd.ExecuteNonQuery();");
        sb.AppendLine("    }");
        sb.AppendLine();

        // GET ALL
        sb.AppendLine($"    public List<{className}> GetAll()");
        sb.AppendLine("    {");
        sb.AppendLine($"        var list = new List<{className}>();");
        sb.AppendLine($"        using var conn = new NpgsqlConnection(_connectionString);");
        sb.AppendLine($"        conn.Open();");
        sb.AppendLine($"        string sql = \"SELECT * FROM {table.Nome}\";");
        sb.AppendLine($"        using var cmd = new NpgsqlCommand(sql, conn);");
        sb.AppendLine($"        using var reader = cmd.ExecuteReader();");
        sb.AppendLine($"        while (reader.Read())");
        sb.AppendLine("        {");
        sb.AppendLine($"            var entity = new {className}();");
        foreach (var col in table.Columns)
        {
            string propName = ToPascalCase(col.Nome);
            string convertTo = GetCSharpType(col.Tipo);
            sb.AppendLine($"            if (!reader.IsDBNull(reader.GetOrdinal(\"{col.Nome}\")))");
            sb.AppendLine($"                entity.{propName} = ({convertTo})reader[\"{col.Nome}\"];");
        }
        sb.AppendLine($"            list.Add(entity);");
        sb.AppendLine("        }");
        sb.AppendLine($"        return list;");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        File.WriteAllText(Path.Combine(outputDir, $"{className}Dao.cs"), sb.ToString());
    }

    private static void GenerateExample(TableInfo table, string className, string outputDir)
    {
        var sb = new StringBuilder();
        sb.AppendLine("using System;");
        sb.AppendLine();
        sb.AppendLine($"public class {className}Exemplo");
        sb.AppendLine("{");
        sb.AppendLine("    public static void Executar(string connectionString)");
        sb.AppendLine("    {");
        sb.AppendLine($"        var dao = new {className}Dao(connectionString);");
        sb.AppendLine($"        var entidade = new {className}();");
        sb.AppendLine();
        foreach (var col in table.Columns)
        {
            if (!col.EhChavePrimaria)
            {
                string propName = ToPascalCase(col.Nome);
                string stringLiteral = GetHardcodedRandomValue(col);
                sb.AppendLine($"        entidade.{propName} = {stringLiteral};");
            }
        }
        sb.AppendLine();
        sb.AppendLine($"        dao.Insert(entidade);");
        sb.AppendLine($"        Console.WriteLine(\"Operação de exemplo para {className} executada.\");");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        File.WriteAllText(Path.Combine(outputDir, $"{className}Exemplo.cs"), sb.ToString());
    }

    private static Random _rand = new Random();

    private static string GetHardcodedRandomValue(ColumnInfo col)
    {
        string lowerName = col.Nome.ToLower();
        // Infere chave estrangeira por convenção
        bool isForeignKey = lowerName.EndsWith("_id") || (lowerName.EndsWith("id") && lowerName != "id");

        if (isForeignKey)
        {
            return _rand.Next(1, 6).ToString(); // ID aleatório entre 1 e 5
        }

        string csharpType = GetCSharpType(col.Tipo);

        switch (csharpType)
        {
            case "int":
                return _rand.Next(1, 1000).ToString();
            case "double":
                return (_rand.NextDouble() * 1000.0).ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
            case "float":
                return (_rand.NextDouble() * 1000.0).ToString("F2", System.Globalization.CultureInfo.InvariantCulture) + "f";
            case "bool":
                return _rand.Next(2) == 0 ? "true" : "false";
            case "DateTime":
                var randomDate = DateTime.Now.AddDays(-_rand.Next(1, 365));
                return $"new DateTime({randomDate.Year}, {randomDate.Month}, {randomDate.Day})";
            case "Guid":
                return $"new Guid(\"{Guid.NewGuid()}\")";
            case "string":
            default:
                return $"\"{ToPascalCase(col.Nome)} {_rand.Next(1, 1000)}\"";
        }
    }

    private static string GetCSharpType(string dbType)
    {
        string lowerType = dbType.ToLower();
        if (lowerType.Contains("int")) return "int";
        if (lowerType.Contains("char") || lowerType.Contains("text")) return "string";
        if (lowerType.Contains("bool")) return "bool";
        if (lowerType.Contains("date") || lowerType.Contains("time")) return "DateTime";
        if (lowerType.Contains("numeric") || lowerType.Contains("decimal") || lowerType.Contains("double")) return "double";
        if (lowerType.Contains("float") || lowerType.Contains("real")) return "float";
        if (lowerType.Contains("uuid")) return "Guid";

        return "string"; // fallback
    }

    private static string ToPascalCase(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;

        var parts = text.Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
        var sb = new StringBuilder();
        foreach (var part in parts)
        {
            sb.Append(char.ToUpper(part[0]));
            if (part.Length > 1) sb.Append(part.Substring(1).ToLower());
        }
        return sb.ToString();
    }
}