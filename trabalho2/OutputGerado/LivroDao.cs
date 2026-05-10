using System;
using System.Collections.Generic;
using Npgsql; // Usando ADO.NET com PostgreSQL

public class LivroDao
{
    private readonly string _connectionString;

    public LivroDao(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Insert(Livro entity)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        string sql = "INSERT INTO livro (titulo, autor_id) VALUES (@titulo, @autor_id)";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@titulo", entity.Titulo ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@autor_id", entity.AutorId ?? (object)DBNull.Value);
        cmd.ExecuteNonQuery();
    }

    public void Update(Livro entity)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        string sql = "UPDATE livro SET titulo = @titulo, autor_id = @autor_id WHERE id = @id";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", entity.Id ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@titulo", entity.Titulo ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@autor_id", entity.AutorId ?? (object)DBNull.Value);
        cmd.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        string sql = "DELETE FROM livro WHERE id = @id";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    public List<Livro> GetAll()
    {
        var list = new List<Livro>();
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        string sql = "SELECT * FROM livro";
        using var cmd = new NpgsqlCommand(sql, conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var entity = new Livro();
            if (!reader.IsDBNull(reader.GetOrdinal("id")))
                entity.Id = (int)reader["id"];
            if (!reader.IsDBNull(reader.GetOrdinal("titulo")))
                entity.Titulo = (string)reader["titulo"];
            if (!reader.IsDBNull(reader.GetOrdinal("autor_id")))
                entity.AutorId = (int)reader["autor_id"];
            list.Add(entity);
        }
        return list;
    }
}
