using System;
using System.Collections.Generic;
using Npgsql; // Usando ADO.NET com PostgreSQL

public class EmprestimoDao
{
    private readonly string _connectionString;

    public EmprestimoDao(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Insert(Emprestimo entity)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        string sql = "INSERT INTO emprestimo (usuario_id, livro_id, data_emprestimo) VALUES (@usuario_id, @livro_id, @data_emprestimo)";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@usuario_id", entity.UsuarioId ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@livro_id", entity.LivroId ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@data_emprestimo", entity.DataEmprestimo ?? (object)DBNull.Value);
        cmd.ExecuteNonQuery();
    }

    public void Update(Emprestimo entity)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        string sql = "UPDATE emprestimo SET usuario_id = @usuario_id, livro_id = @livro_id, data_emprestimo = @data_emprestimo WHERE id = @id";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", entity.Id ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@usuario_id", entity.UsuarioId ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@livro_id", entity.LivroId ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@data_emprestimo", entity.DataEmprestimo ?? (object)DBNull.Value);
        cmd.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        string sql = "DELETE FROM emprestimo WHERE id = @id";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    public List<Emprestimo> GetAll()
    {
        var list = new List<Emprestimo>();
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        string sql = "SELECT * FROM emprestimo";
        using var cmd = new NpgsqlCommand(sql, conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var entity = new Emprestimo();
            if (!reader.IsDBNull(reader.GetOrdinal("id")))
                entity.Id = (int)reader["id"];
            if (!reader.IsDBNull(reader.GetOrdinal("usuario_id")))
                entity.UsuarioId = (int)reader["usuario_id"];
            if (!reader.IsDBNull(reader.GetOrdinal("livro_id")))
                entity.LivroId = (int)reader["livro_id"];
            if (!reader.IsDBNull(reader.GetOrdinal("data_emprestimo")))
                entity.DataEmprestimo = (DateTime)reader["data_emprestimo"];
            list.Add(entity);
        }
        return list;
    }
}
