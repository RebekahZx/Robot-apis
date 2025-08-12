using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Npgsql;
using robot_controller_api.Models;

namespace robot_controller_api.Persistence
{
    public class UserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<UserModel> GetAllUsers()
        {
            var users = new List<UserModel>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var command = new NpgsqlCommand("SELECT * FROM users", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new UserModel
                        {
                            Id = reader.GetInt32(0),
                            FirstName = reader.GetString(1),
                            LastName = reader.GetString(2),
                            Email = reader.GetString(3),
                            Role = reader.GetString(4),
                            CreatedDate = reader.GetDateTime(5),
                            ModifiedDate = reader.GetDateTime(6)
                        });
                    }
                }
            }

            return users;
        }

        public UserModel? GetUserById(Guid id)
        {
            UserModel? user = null;

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var command = new NpgsqlCommand("SELECT * FROM users WHERE id = @Id", connection);
                command.Parameters.AddWithValue("Id", id);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new UserModel
                        {
                            Id = reader.GetInt32(0),
                            FirstName = reader.GetString(1),
                            LastName = reader.GetString(2),
                            Email = reader.GetString(3),
                            Role = reader.GetString(4),
                            CreatedDate = reader.GetDateTime(5),
                            ModifiedDate = reader.GetDateTime(6)
                        };
                    }
                }
            }

            return user;
        }

        public List<UserModel> GetAdminUsers()
        {
            var adminUsers = new List<UserModel>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var command = new NpgsqlCommand("SELECT * FROM users WHERE role = 'Admin'", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        adminUsers.Add(new UserModel
                        {
                            Id = reader.GetInt32(0),
                            FirstName = reader.GetString(1),
                            LastName = reader.GetString(2),
                            Email = reader.GetString(3),
                            Role = reader.GetString(4),
                            CreatedDate = reader.GetDateTime(5),
                            ModifiedDate = reader.GetDateTime(6)
                        });
                    }
                }
            }

            return adminUsers;
        }

        public void AddUser(UserModel user)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var command = new NpgsqlCommand("INSERT INTO users (id, first_name, last_name, email, role, created_date, modified_date) VALUES (@Id, @FirstName, @LastName, @Email, @Role, @CreatedDate, @ModifiedDate)", connection);
                command.Parameters.AddWithValue("Id", user.Id);
                command.Parameters.AddWithValue("FirstName", user.FirstName);
                command.Parameters.AddWithValue("LastName", user.LastName);
                command.Parameters.AddWithValue("Email", user.Email);
                command.Parameters.AddWithValue("Role", user.Role);
                command.Parameters.AddWithValue("CreatedDate", user.CreatedDate);
                command.Parameters.AddWithValue("ModifiedDate", user.ModifiedDate);

                command.ExecuteNonQuery();
            }
        }

        public void UpdateUser(UserModel user)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var command = new NpgsqlCommand("UPDATE users SET first_name = @FirstName, last_name = @LastName, email = @Email, role = @Role, created_date = @CreatedDate, modified_date = @ModifiedDate WHERE id = @Id", connection);
                command.Parameters.AddWithValue("FirstName", user.FirstName);
                command.Parameters.AddWithValue("LastName", user.LastName);
                command.Parameters.AddWithValue("Email", user.Email);
                command.Parameters.AddWithValue("Role", user.Role);
                command.Parameters.AddWithValue("CreatedDate", user.CreatedDate);
                command.Parameters.AddWithValue("ModifiedDate", user.ModifiedDate);
                command.Parameters.AddWithValue("Id", user.Id);

                command.ExecuteNonQuery();
            }
        }

        public void DeleteUser(UserModel user)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                var command = new NpgsqlCommand("DELETE FROM users WHERE id = @Id", connection);
                command.Parameters.AddWithValue("Id", user.Id);

                command.ExecuteNonQuery();
            }
        }
    }
}
