using System.ComponentModel;
using System.Data.Common;
using Npgsql;

public class BaseDao<T> : IDao<T>
{

    protected DbConnection _conn;

    public BaseDao(DbConnection conn)
    {
        _conn = conn;
    }

    public void Insert(T obj)
    {
        var atributos = typeof(T).GetProperties();
        var idProp = typeof(T).GetProperty("Id");

        if (idProp == null)
        {
            throw new Exception("A classe deve possuir uma propriedade Id");
        }

        List<string> partes = new List<string>();
        foreach (var prop in atributos)
        {
            if (prop.Name != "Id")
            {
                var valor = prop.GetValue(obj);

                if (valor == null)
                {

                    valor = "NULL";
                }
                else if (valor is string)
                {
                    valor = $"'{valor}'";
                }
                else if (valor is DateOnly dt)
                {
                    valor = $"'{dt:yyyy-MM-dd}'";
                }

                var parte = $"{valor}";
                partes.Add(parte);
            }
        }
        var colunas = atributos.Where(p => p.Name != "Id").Select(p => p.Name.ToLower());
        var sql = $"INSERT INTO {typeof(T).Name.ToLower()} ({string.Join(", ", colunas)}) VALUES ({string.Join(", ", partes)}) RETURNING id";

        using var command = _conn.CreateCommand();
        command.CommandText = sql;

        var idGerado = (int)command.ExecuteScalar();

        idProp.SetValue(obj, Convert.ToInt32(idGerado));

        
    }


    public void Update(T obj)
    {
        var atributos = typeof(T).GetProperties();

        var idProp = typeof(T).GetProperty("Id");

        if (idProp == null)
        {
            throw new Exception("A classe deve possuir uma propriedade Id");
        }

        var id = idProp.GetValue(obj);
        List<string> partes = new List<string>();

        foreach (var prop in atributos)
        {
            if (prop.Name != "Id")
            {
                var valor = prop.GetValue(obj);

                if (valor == null)
                {

                    valor = "NULL";
                }
                else if (valor is string)
                {
                    valor = $"'{valor}'";
                }

                var parte = $"{prop.Name.ToLower()} = {valor}";
                partes.Add(parte);
            }
        }
        string set = string.Join(", ", partes);
        var sql = $"UPDATE {typeof(T).Name.ToLower()} SET {set} WHERE id = {id}";

        using var command = _conn.CreateCommand();
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        var sql = $"DELETE FROM {typeof(T).Name.ToLower()} WHERE id = {id}";

        using var command = _conn.CreateCommand();
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }

    public T GetById(int id)
    {
        var sql = $"SELECT * FROM {typeof(T).Name.ToLower()} WHERE id = {id}";

        using var command = _conn.CreateCommand();
        command.CommandText = sql;
        using var reader = command.ExecuteReader();

        if (reader.Read())
        {
            var obj = Activator.CreateInstance<T>();
            var atributos = typeof(T).GetProperties();

            foreach (var prop in atributos)
            {
                var valor = reader[prop.Name.ToLower()];
                if (valor != DBNull.Value)
                {
                    prop.SetValue(obj, Convert.ChangeType(valor, prop.PropertyType));
                }
            }
            return obj;
        }
        else
        {
            throw new Exception($"{typeof(T).Name} com id {id} não encontrado");
        }
    }

    public List<T> GetAll()
    {
        var sql = $"SELECT * FROM {typeof(T).Name.ToLower()}";
        using var command = _conn.CreateCommand();
        command.CommandText = sql;
        using var reader = command.ExecuteReader();
        List<T> lista = new List<T>();
        var atributos = typeof(T).GetProperties();
        while (reader.Read())
        {
            var obj = Activator.CreateInstance<T>();

            foreach (var prop in atributos)
            {
                var valor = reader[prop.Name.ToLower()];
                if (valor != DBNull.Value)
                {
                    prop.SetValue(obj, Convert.ChangeType(valor, prop.PropertyType));
                }
            }
            lista.Add(obj);
        }
        return lista;
    }
}