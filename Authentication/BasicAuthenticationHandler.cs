
// using Microsoft.AspNetCore.Authentication;
// using Microsoft.Extensions.Logging;
// using Microsoft.Extensions.Options;
// using Microsoft.AspNetCore.Http;
// using System.Text.Encodings.Web;
// using System.Threading.Tasks;
// using System.Text;
// using System.Security.Claims;
// using System;
// using System.Linq;
// using System.Net.Http.Headers;
// using Npgsql;
// using robot_controller_api.Models;
// using Microsoft.AspNetCore.Identity;
// using System.Security.Cryptography;
// using robot_controller_api.AuthUtils;
// using Microsoft.Extensions.Configuration;

// namespace robot_controller_api.Authentication
// {
//     //     public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
//     //     {
//             private readonly string _connectionString = "Host=localhost;Username=postgres;Password=12345;Database=sit331";
//             // private readonly IConfiguration _configuration;  


//             // public BasicAuthenticationHandler(
//             //     IOptionsMonitor<AuthenticationSchemeOptions> options,
//             //     ILoggerFactory logger,
//             //     UrlEncoder encoder,
//             //     ISystemClock clock,
//             //     IConfiguration configuration  
//             // ) : base(options, logger, encoder, clock)
//             // {
//             //     _configuration = configuration;
//             // }

//     //         protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
//     //         {
//     //             if (!Request.Headers.ContainsKey("Authorization"))
//     //             {
//     //                 return AuthenticateResult.Fail("Missing Authorization Header");
//     //             }

//     //             try
//     //             {
//     //                 var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
//     //                 var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
//     //                 var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);

//     //                 if (credentials.Length != 2)
//     //                 {
//     //                     return AuthenticateResult.Fail("Invalid Authorization Header");
//     //                 }

//     //                 var email = credentials[0];
//     //                 var password = credentials[1];


//     //                 UserModel user = null;

//     //                 using (var connection = new NpgsqlConnection(_connectionString))
//     //                 {
//     //                     await connection.OpenAsync();
//     //                     var command = new NpgsqlCommand("SELECT * FROM users WHERE email = @Email", connection);
//     //                     command.Parameters.AddWithValue("@Email", email);

//     //                     using (var reader = await command.ExecuteReaderAsync())
//     //                     {
//     //                         if (await reader.ReadAsync())
//     //                         {
//     //                             user = new UserModel
//     //                             {
//     //                                 Id = reader.GetInt32(0),
//     //                                 Email = reader.GetString(1),
//     //                                 FirstName = reader.GetString(2),
//     //                                 LastName = reader.GetString(3),
//     //                                 PasswordHash = reader.GetString(4),
//     //                                 Description = reader.GetString(5),
//     //                                 Role = reader.GetString(6),
//     //                                 CreatedDate = reader.GetDateTime(7),
//     //                                 ModifiedDate = reader.GetDateTime(8)
//     //                             };
//     //                         }
//     //                     }
//     //                 }

//     //                 if (user == null)
//     //                 {
//     //                     Response.StatusCode = 401;
//     //                     return AuthenticateResult.Fail("Authentication failed: User not found");
//     //                 }


//     //                 var passwordService = new PasswordService(_configuration); // Now correctly initialized


//     //                 if (!passwordService.VerifyPassword(password, user.PasswordHash))
//     //                 {
//     //                     Response.StatusCode = 401;
//     //                     return AuthenticateResult.Fail("Authentication failed: Invalid password");
//     //                 }


//     //                 var claims = new[]
//     //                 {
//     //                     new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
//     //                     new Claim(ClaimTypes.Email, user.Email),
//     //                     new Claim(ClaimTypes.Role, user.Role)
//     //                 };

//     //                 var identity = new ClaimsIdentity(claims, Scheme.Name);
//     //                 var principal = new ClaimsPrincipal(identity);
//     //                 var ticket = new AuthenticationTicket(principal, Scheme.Name);

//     //                 return AuthenticateResult.Success(ticket);
//     //             }
//     //             catch (Exception ex)
//     //             {
//     //                 return AuthenticateResult.Fail($"Authentication error: {ex.Message}");
//     //             }
//     //         }
//     //     }
//     // }

//     protected override Task<AuthenticateResult> HandleAuthenticateAsync()
//     {
//         if (!Request.Headers.ContainsKey("Authorization"))
//         {
//             return Task.FromResult(AuthenticateResult.Fail("Missing Authorization Header"));
//         }

//         try
//         {
//             var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
//             var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
//             var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);

//             if (credentials.Length != 2)
//             {
//                 return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
//             }

//             var email = credentials[0];
//             var password = credentials[1];

//             UserModel user = null;

//             using (var connection = new NpgsqlConnection(_connectionString))
//             {
//                 connection.Open();
//                 var command = new NpgsqlCommand("SELECT * FROM users WHERE email = @Email", connection);
//                 command.Parameters.AddWithValue("@Email", email);

//                 using (var reader = command.ExecuteReader())
//                 {
//                     if (reader.Read())
//                     {
//                         user = new UserModel
//                         {
//                             Id = reader.GetInt32(0),
//                             Email = reader.GetString(1),
//                             FirstName = reader.GetString(2),
//                             LastName = reader.GetString(3),
//                             PasswordHash = reader.GetString(4),
//                             Description = reader.GetString(5),
//                             Role = reader.GetString(6),
//                             CreatedDate = reader.GetDateTime(7),
//                             ModifiedDate = reader.GetDateTime(8)
//                         };
//                     }
//                 }
//             }

//             if (user == null)
//             {
//                 Response.StatusCode = 401;
//                 return Task.FromResult(AuthenticateResult.Fail("Authentication failed: User not found"));
//             }

//             var passwordService = new PasswordService(_configuration);

//             if (!passwordService.VerifyPassword(password, user.PasswordHash))
//             {
//                 Response.StatusCode = 401;
//                 return Task.FromResult(AuthenticateResult.Fail("Authentication failed: Invalid password"));
//             }

//             var claims = new[]
//             {
//             new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
//             new Claim(ClaimTypes.Email, user.Email),
//             new Claim(ClaimTypes.Role, user.Role)
//         };

//             var identity = new ClaimsIdentity(claims, Scheme.Name);
//             var principal = new ClaimsPrincipal(identity);
//             var ticket = new AuthenticationTicket(principal, Scheme.Name);

//             return Task.FromResult(AuthenticateResult.Success(ticket));
//         }
//         catch (Exception ex)
//         {
//             return Task.FromResult(AuthenticateResult.Fail($"Authentication error: {ex.Message}"));
//         }
//     }
// }


using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using System.Text.Encodings.Web;
using System.Text;
using System.Security.Claims;
using System;
using System.Net.Http.Headers;
using Npgsql;
using robot_controller_api.Models;
using robot_controller_api.AuthUtils;
using Microsoft.Extensions.Configuration;

namespace robot_controller_api.Authentication
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly string _connectionString = "Host=localhost;Username=postgres;Password=12345;Database=sit331";
        private readonly IConfiguration _configuration;

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IConfiguration configuration
        ) : base(options, logger, encoder, clock)
        {
            _configuration = configuration;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return Task.FromResult(AuthenticateResult.Fail("Missing Authorization Header"));
            }

            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);

                if (credentials.Length != 2)
                {
                    return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
                }

                var email = credentials[0];
                var password = credentials[1];

                UserModel user = null;

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new NpgsqlCommand("SELECT * FROM users WHERE email = @Email", connection);
                    command.Parameters.AddWithValue("@Email", email);

                    using (var reader = command.ExecuteReader())
                    {
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
                    }
                }

                if (user == null)
                {
                    Response.StatusCode = 401;
                    return Task.FromResult(AuthenticateResult.Fail("Authentication failed: User not found"));
                }

                var passwordService = new PasswordService(_configuration);

                if (!passwordService.VerifyPassword(password, user.PasswordHash))
                {
                    Response.StatusCode = 401;
                    return Task.FromResult(AuthenticateResult.Fail("Authentication failed: Invalid password"));
                }

                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role),
                    // new Claim("developer", "true")
                };

                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
            catch (Exception ex)
            {
                return Task.FromResult(AuthenticateResult.Fail($"Authentication error: {ex.Message}"));
            }
        }
    }
}
