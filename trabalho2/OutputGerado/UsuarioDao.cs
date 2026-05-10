using System;
using System.Collections.Generic;
using Npgsql; // Usando ADO.NET com PostgreSQL

public class UsuarioDao
{
    private readonly string _connectionString;

    public UsuarioDao(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Insert(Usuario entity)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        string sql = "INSERT INTO usuario (nome, email) VALUES (@nome, @email)";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@nome", entity.Nome ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@email", entity.Email ?? (object)DBNull.Value);
        cmd.ExecuteNonQuery();
    }

    public void Update(Usuario entity)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        string sql = "UPDATE usuario SET nome = @nome, email = @email WHERE id = @id";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", entity.Id ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@nome", entity.Nome ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@email", entity.Email ?? (object)DBNull.Value);
        cmd.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        string sql = "DELETE FROM usuario WHERE id = @id";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    public List<Usuario> GetAll()
    {
        var list = new List<Usuario>();
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        string sql = "SELECT * FROM usuario";
        using var cmd = new NpgsqlCommand(sql, conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var entity = new Usuario();
            if (!reader.IsDBNull(reader.GetOrdinal("id")))
                entity.Id = (int)reader["id"];
            if (!reader.IsDBNull(reader.GetOrdinal("nome")))
                entity.Nome = (string)reader["nome"];
            if (!reader.IsDBNull(reader.GetOrdinal("email")))
                entity.Email = (string)reader["email"];
            list.Add(entity);
        }
        return list;
    }
}
