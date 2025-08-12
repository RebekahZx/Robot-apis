using Microsoft.AspNetCore.Mvc;
using robot_controller_api.Models;
using Npgsql;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using robot_controller_api.AuthUtils;

namespace robot_controller_api.Controllers
{
    [Route("users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly string _connectionString = "Host=localhost;Username=postgres;Password=12345;Database=sit331";
        private readonly PasswordService _passwordService;

        public UserController(PasswordService passwordService)
        {
            _passwordService = passwordService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<UserModel>> GetAllUsers()
        {
            var users = new List<UserModel>();
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            var command = new NpgsqlCommand("SELECT * FROM users", connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                users.Add(new UserModel
                {
                    Id = reader.GetInt32(0),
                    Email = reader.GetString(1),
                    FirstName = reader.GetString(2),
                    LastName = reader.GetString(3),
                    PasswordHash = reader.GetString(4),
                    Description = reader.GetString(5),
                    Role = reader.GetString(6),
                    CreatedDate = reader.GetDateTime(7),
                    ModifiedDate = reader.GetDateTime(8)
                });
            }

            return Ok(users);
        }

        [HttpGet("admin")]
        // [Authorize(Policy = "AdminOnly")]
        public ActionResult<IEnumerable<UserModel>> GetUsersByRole()
        {
            var users = new List<UserModel>();
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            var command = new NpgsqlCommand("SELECT * FROM users WHERE role = @Role", connection);
            command.Parameters.AddWithValue("@Role", "Admin");
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                users.Add(new UserModel
                {
                    Id = reader.GetInt32(0),
                    Email = reader.GetString(1),
                    FirstName = reader.GetString(2),
                    LastName = reader.GetString(3),
                    PasswordHash = reader.GetString(4),
                    Description = reader.GetString(5),
                    Role = reader.GetString(6),
                    CreatedDate = reader.GetDateTime(7),
                    ModifiedDate = reader.GetDateTime(8)
                });
            }

            return Ok(users);
        }

        [HttpGet("{id}")]
        public ActionResult<UserModel> GetUserById(int id)
        {
            UserModel user = null;
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            var command = new NpgsqlCommand("SELECT * FROM users WHERE id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                user = new UserModel
                {
                    Id = reader.GetInt32(0),
                    Email = reader.GetString(1),
                    FirstName = reader.GetString(2),
                    LastName = reader.GetString(3),
                    PasswordHash = reader.GetString(4),
                    Description = reader.GetString(5),
                    Role = reader.GetString(6),
                    CreatedDate = reader.GetDateTime(7),
                    ModifiedDate = reader.GetDateTime(8)
                };
            }

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult<UserModel> CreateUser([FromBody] UserModel user)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            string passwordHash = _passwordService.HashPassword(user.PasswordHash);

            var command = new NpgsqlCommand(@"
                INSERT INTO users (email, first_name, last_name, password_hash, description, role, created_date, modified_date) 
                VALUES (@Email, @FirstName, @LastName, @PasswordHash, @Description, @Role, @CreatedDate, @ModifiedDate) 
                RETURNING id", connection);

            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@FirstName", user.FirstName);
            command.Parameters.AddWithValue("@LastName", user.LastName);
            command.Parameters.AddWithValue("@PasswordHash", passwordHash);
            command.Parameters.AddWithValue("@Description", user.Description ?? string.Empty);
            command.Parameters.AddWithValue("@Role", user.Role ?? "User");
            command.Parameters.AddWithValue("@CreatedDate", DateTime.UtcNow);
            command.Parameters.AddWithValue("@ModifiedDate", DateTime.UtcNow);

            user.Id = (int)command.ExecuteScalar();

            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        [Authorize]
        public IActionResult UpdateUser(int id, [FromBody] UpdateUserModel user)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            var command = new NpgsqlCommand(@"
                UPDATE users 
                SET first_name = @FirstName, 
                    last_name = @LastName, 
                    description = @Description, 
                    modified_date = @ModifiedDate 
                WHERE id = @Id", connection);

            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@FirstName", user.FirstName ?? string.Empty);
            command.Parameters.AddWithValue("@LastName", user.LastName ?? string.Empty);
            command.Parameters.AddWithValue("@Description", user.Description ?? string.Empty);
            command.Parameters.AddWithValue("@ModifiedDate", DateTime.UtcNow);

            var rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected == 0)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        // [Authorize(Policy = "AdminOnly")]
        public IActionResult DeleteUser(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            var command = new NpgsqlCommand("DELETE FROM users WHERE id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            var rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected == 0)
                return NotFound();

            return NoContent();
        }

        [HttpPatch("{id}")]
        [Authorize]
        public IActionResult PatchUser(int id, [FromBody] LoginModel login)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            string passwordHash = _passwordService.HashPassword(login.Password);

            var command = new NpgsqlCommand(@"
                UPDATE users 
                SET email = @Email, password_hash = @PasswordHash, modified_date = @ModifiedDate 
                WHERE id = @Id", connection);

            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Email", login.Email);
            command.Parameters.AddWithValue("@PasswordHash", passwordHash);
            command.Parameters.AddWithValue("@ModifiedDate", DateTime.UtcNow);

            var rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected == 0)
                return NotFound();

            return NoContent();
        }
    }
}
