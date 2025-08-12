using Npgsql;

 namespace robot_controller_api.Persistence {
 public static class RobotCommandDataAccess
 {
 // Connection string is usually set in a config file for the ease
//of change.
 private const string CONNECTION_STRING ="Host=localhost;Username=postgres;Password=12345;Database=sit331";
//  private static string CONNECTION_STRING = Environment.GetEnvironmentVariable("DefaultConnection");

 public static List<RobotCommand> GetRobotCommands()
 {
 var robotCommands = new List<RobotCommand>();
 using var conn = new NpgsqlConnection(CONNECTION_STRING);
 conn.Open();
 using var cmd = new NpgsqlCommand("SELECT * FROM robot_command", conn);

using var dr = cmd.ExecuteReader();
 while (dr.Read())
            {
                var command = new RobotCommand
                {
                    Id = dr.GetInt32(dr.GetOrdinal("id")),
                    Name = dr.GetString(dr.GetOrdinal("name")),
                    Description = dr.IsDBNull(dr.GetOrdinal("description")) ? null : dr.GetString(dr.GetOrdinal("description")),
                    IsMoveCommand = dr.GetBoolean(dr.GetOrdinal("is_move_command")),
                    CreatedDate = dr.GetDateTime(dr.GetOrdinal("created_date")),
                    ModifiedDate = dr.IsDBNull(dr.GetOrdinal("modified_date")) ? (DateTime?)null : dr.GetDateTime(dr.GetOrdinal("modified_date")),
                };

                robotCommands.Add(command);
            }
 return robotCommands;
 }




        public static void AddRobotCommand(RobotCommand command)
        {
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();

            var query = @"
            INSERT INTO robot_command (""name"", description, is_move_command, created_date, modified_date)
            VALUES (@name, @description, @is_move_command, @created_date, @modified_date)";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@name", command.Name);
            cmd.Parameters.AddWithValue("@description", (object?)command.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@is_move_command", command.IsMoveCommand);
            cmd.Parameters.AddWithValue("@created_date", command.CreatedDate);
            cmd.Parameters.AddWithValue("@modified_date", command.ModifiedDate);

            cmd.ExecuteNonQuery();
        }

    public static bool UpdateRobotCommand(int id, RobotCommand command)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        var query = @"
            UPDATE robot_command
            SET ""name"" = @name,
                description = @description,
                is_move_command = @is_move_command,
                modified_date = @modified_date
            WHERE id = @id";

        using var cmd = new NpgsqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@name", command.Name);
        cmd.Parameters.AddWithValue("@description", (object?)command.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@is_move_command", command.IsMoveCommand);
        cmd.Parameters.AddWithValue("@modified_date", command.ModifiedDate);
        cmd.Parameters.AddWithValue("@id", id);

        return cmd.ExecuteNonQuery() > 0;
    }

    public static bool DeleteRobotCommand(int id)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        using var cmd = new NpgsqlCommand("DELETE FROM robot_command WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);

        return cmd.ExecuteNonQuery() > 0;
    }
 }
}