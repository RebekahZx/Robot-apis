using Npgsql;

namespace robot_controller_api.Persistence
{
    public static class MapDataAccess
    {
        private const string CONNECTION_STRING = "Host=localhost;Username=postgres;Password=12345;Database=sit331";

        public static List<Map> GetMaps()
        {
            var maps = new List<Map>();
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();

            using var cmd = new NpgsqlCommand("SELECT * FROM map", conn);
            using var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                var map = new Map
                {
                    Id = dr.GetInt32(dr.GetOrdinal("id")),
                    Columns = dr.GetInt32(dr.GetOrdinal("columns")),
                    Rows = dr.GetInt32(dr.GetOrdinal("rows")),
                    Name = dr.GetString(dr.GetOrdinal("name")),
                    Description = dr.IsDBNull(dr.GetOrdinal("description")) ? null : dr.GetString(dr.GetOrdinal("description")),
                    CreatedDate = dr.GetDateTime(dr.GetOrdinal("created_date")),
                    ModifiedDate = dr.GetDateTime(dr.GetOrdinal("modified_date"))
                };
                maps.Add(map);
            }

            return maps;
        }
        public static Map AddMap(Map map)
{
    

    using (var connection = new NpgsqlConnection(CONNECTION_STRING))
    {
        connection.Open();

        var command = new NpgsqlCommand(
            @"INSERT INTO map (name, rows, columns, created_date, modified_date)
              VALUES (@name, @rows, @columns, @created_date, @modified_date)
              RETURNING id", connection);  // 🔥 This gets the actual inserted id

        command.Parameters.AddWithValue("@name", map.Name);
        command.Parameters.AddWithValue("@rows", map.Rows);
        command.Parameters.AddWithValue("@columns", map.Columns);
        command.Parameters.AddWithValue("@created_date", map.CreatedDate);
        command.Parameters.AddWithValue("@modified_date", map.ModifiedDate);

        var insertedId = (int)command.ExecuteScalar(); // 🚀 fetches id
        map.Id = insertedId;

        return map;
    }
}

public static bool UpdateMap(int id, Map map)
{
    using var conn = new NpgsqlConnection(CONNECTION_STRING);
    conn.Open();

    var query = @"
        UPDATE map
        SET name = @name,
            description = @description,
            rows = @rows,
            columns = @columns,
            modified_date = @modified_date
        WHERE id = @id";

    using var cmd = new NpgsqlCommand(query, conn);
    cmd.Parameters.AddWithValue("@name", map.Name);
    cmd.Parameters.AddWithValue("@description", (object?)map.Description ?? DBNull.Value);
    cmd.Parameters.AddWithValue("@rows", map.Rows);
    cmd.Parameters.AddWithValue("@columns", map.Columns);
    cmd.Parameters.AddWithValue("@modified_date", map.ModifiedDate);
    cmd.Parameters.AddWithValue("@id", id);

    return cmd.ExecuteNonQuery() > 0;
}

        public static bool DeleteMap(int id)
        {
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();

            using var cmd = new NpgsqlCommand("DELETE FROM map WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            return cmd.ExecuteNonQuery() > 0;
        }
        public static Map? GetMapById(int id)
        {
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();

            using var cmd = new NpgsqlCommand("SELECT * FROM map WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("id", id);

            using var dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                return new Map
                {
                    Id = dr.GetInt32(dr.GetOrdinal("id")),
                    Columns = dr.GetInt32(dr.GetOrdinal("columns")),
                    Rows = dr.GetInt32(dr.GetOrdinal("rows")),
                    Name = dr.GetString(dr.GetOrdinal("name")),
                    Description = dr.IsDBNull(dr.GetOrdinal("description")) ? null : dr.GetString(dr.GetOrdinal("description")),
                    CreatedDate = dr.GetDateTime(dr.GetOrdinal("created_date")),
                    ModifiedDate = dr.GetDateTime(dr.GetOrdinal("modified_date"))
                };
            }

            return null;
        }
    }
}
