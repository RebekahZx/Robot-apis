using Npgsql;
using robot_controller_api.Models;
using System.Threading.Tasks;

namespace robot_controller_api.Helper
{
    public class DbHelper
    {
        

        
        
            private readonly string _connectionString = "Host=localhost;Username=postgres;Password=12345;Database=sit331";
        
        

        public async Task<UserModel> GetUserByEmailAsync(string email)
        {
            UserModel user = null;

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new NpgsqlCommand("SELECT * FROM Users WHERE Email = @Email", connection);
                command.Parameters.AddWithValue("@Email", email);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        user = new UserModel
                        {
                            Id = reader.GetInt32(0),
                            Email = reader.GetString(1),
                            FirstName = reader.GetString(2),
                            LastName = reader.GetString(3),
                            PasswordHash = reader.GetString(4),
                            Role = reader.GetString(5),
                            CreatedDate = reader.GetDateTime(6),
                            ModifiedDate = reader.GetDateTime(7)
                        };
                    }
                }
            }

            return user;
        }

        // Fetch user by ID
        public async Task<UserModel> GetUserByIdAsync(int id)
        {
            UserModel user = null;

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new NpgsqlCommand("SELECT * FROM Users WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        user = new UserModel
                        {
                            Id = reader.GetInt32(0),
                            Email = reader.GetString(1),
                            FirstName = reader.GetString(2),
                            LastName = reader.GetString(3),
                            PasswordHash = reader.GetString(4),
                            Role = reader.GetString(5),
                            CreatedDate = reader.GetDateTime(6),
                            ModifiedDate = reader.GetDateTime(7)
                        };
                    }
                }
            }

            return user;
        }
    }
}
