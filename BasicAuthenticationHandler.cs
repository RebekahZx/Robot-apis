// using Microsoft.AspNetCore.Authentication;
// using Microsoft.Extensions.Options;
// using Microsoft.AspNetCore.Identity;
// using System.Security.Claims;
// using System.Text;
// using System.Text.Encodings.Web;
// using robot_controller_api.Persistence;

// namespace robot_controller_api
// {
//     public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
//     {
//         private readonly RobotDbContext _dbContext;

//         public BasicAuthenticationHandler(
//             IOptionsMonitor<AuthenticationSchemeOptions> options,
//             ILoggerFactory logger,
//             UrlEncoder encoder,
//             ISystemClock clock,
//             RobotDbContext dbContext)
//             : base(options, logger, encoder, clock)
//         {
//             _dbContext = dbContext;
//         }

//         protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
//         {
//             Response.Headers.Add("WWW-Authenticate", @"Basic realm=""Access to the robot controller.""");

//             var authHeader = Request.Headers["Authorization"].ToString();
//             if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Basic "))
//             {
//                 Response.StatusCode = 401;
//                 return AuthenticateResult.Fail("Missing or invalid Authorization header.");
//             }

//             var encodedCredentials = authHeader.Substring("Basic ".Length).Trim();
//             var credentialBytes = Convert.FromBase64String(encodedCredentials);
//             var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);

//             if (credentials.Length != 2)
//             {
//                 Response.StatusCode = 401;
//                 return AuthenticateResult.Fail("Invalid Authorization header format.");
//             }

//             var email = credentials[0];
//             var password = credentials[1];

//             var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);

//             if (user == null)
//             {
//                 Response.StatusCode = 401;
//                 return AuthenticateResult.Fail("Authentication failed.");
//             }

//             var hasher = new PasswordHasher<UserModel>();
//             var result = hasher.VerifyHashedPassword(user, user.PasswordHash, password);

//             if (result == PasswordVerificationResult.Failed)
//             {
//                 Response.StatusCode = 401;
//                 return AuthenticateResult.Fail("Authentication failed.");
//             }

//             var claims = new[]
//             {
//                 new Claim("name", $"{user.FirstName} {user.LastName}"),
//                 new Claim(ClaimTypes.Role, user.Role)
//             };

//             var identity = new ClaimsIdentity(claims, "Basic");
//             var principal = new ClaimsPrincipal(identity);
//             var ticket = new AuthenticationTicket(principal, Scheme.Name);

//             return AuthenticateResult.Success(ticket);
//         }
//     }
// }
