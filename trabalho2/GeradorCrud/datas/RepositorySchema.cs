using System.Data.SqlTypes;
using System.Security.AccessControl;
using Npgsql;

public class RepositorySchema
{
    readonly private DatabaseConnection _connection;

    public RepositorySchema(DatabaseConnection dataBase)
    {
        _connection = dataBase;
    }

    public List<TableInfo> ListTables()
    {
        var tables = new List<TableInfo>();

        var conn = _connection.GetConnection();
        string queryTable = "SELECT table_name FROM information_schema.tables WHERE table_schema = 'public'";
        var cmdTable = new NpgsqlCommand(queryTable, (NpgsqlConnection)conn);
        var readerTables = cmdTable.ExecuteReader();

        var tableNames = new List<string>();

        while (readerTables.Read())
        {
            tableNames.Add(readerTables.GetString(0));
        }
        readerTables.Close();

        foreach (var name in tableNames)
        {

            var tableInfo = new TableInfo { Nome = name };
            string queryCols = $@"
                    SELECT 
                    cols.column_name, 
                    cols.data_type,
                    CASE WHEN pk.column_name IS NOT NULL THEN 'True' ELSE 'False' END as is_primary
                    FROM information_schema.columns cols
                    LEFT JOIN (
                        SELECT kcu.column_name, kcu.table_name
                        FROM information_schema.table_constraints tc
                        JOIN information_schema.key_column_usage kcu 
                        ON tc.constraint_name = kcu.constraint_name
                        WHERE tc.constraint_type = 'PRIMARY KEY' AND tc.table_name = '{name}'
                    ) pk ON cols.column_name = pk.column_name AND cols.table_name = pk.table_name
                    WHERE cols.table_name = '{name}'";

            var cmdCols = new NpgsqlCommand(queryCols, (NpgsqlConnection)conn);
            var readerCols = cmdCols.ExecuteReader();

            while (readerCols.Read())
            {
                tableInfo.Columns.Add(new ColumnInfo
                {
                    Nome = readerCols.GetString(0),
                    Tipo = readerCols.GetString(1),
                    EhChavePrimaria = readerCols.GetString(2) == "True"
                });
            }
            readerCols.Close();

            tables.Add(tableInfo);
        }
        return tables;

    }

}