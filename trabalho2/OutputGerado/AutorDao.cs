using System;
using System.Collections.Generic;
using Npgsql; // Usando ADO.NET com PostgreSQL

public class AutorDao
{
    private readonly string _connectionString;

    public AutorDao(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Insert(Autor entity)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        string sql = "INSERT INTO autor (nome) VALUES (@nome)";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@nome", entity.Nome ?? (object)DBNull.Value);
        cmd.ExecuteNonQuery();
    }

    public void Update(Autor entity)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        string sql = "UPDATE autor SET nome = @nome WHERE id = @id";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", entity.Id ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@nome", entity.Nome ?? (object)DBNull.Value);
        cmd.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        string sql = "DELETE FROM autor WHERE id = @id";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    public List<Autor> GetAll()
    {
        var list = new List<Autor>();
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        string sql = "SELECT * FROM autor";
        using var cmd = new NpgsqlCommand(sql, conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var entity = new Autor();
            if (!reader.IsDBNull(reader.GetOrdinal("id")))
                entity.Id = (int)reader["id"];
            if (!reader.IsDBNull(reader.GetOrdinal("nome")))
                entity.Nome = (string)reader["nome"];
            list.Add(entity);
        }
        return list;
    }
}
