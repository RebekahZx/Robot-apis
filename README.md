# Robot Controller API

[![build](https://github.com/RebekahZx/Robot-apis/actions/workflows/build.yml/badge.svg)](https://github.com/RebekahZx/Robot-apis/actions/workflows/build.yml)

A REST API for a robot simulator, written in ASP.NET Core 6 on PostgreSQL. It manages
three things:

- **Robot commands**, such as `MOVE` and `LEFT`;
- **Maps**, which are rectangular grids the robot moves on;
- **Users**.

Requests authenticate with HTTP Basic auth, and endpoints are protected by
role-based policies. Built for Deakin University unit SIT331.

## Endpoints

"Public" means no login is required. "User" means any logged-in user (role `User`
or `Admin`). "Admin" means role `Admin`.

**Robot commands** (`/api/robot-commands`)

| Method | Path | Access | Notes |
|---|---|---|---|
| GET | `/api/robot-commands` | Public | All commands |
| GET | `/api/robot-commands/move` | Public | Movement commands only |
| GET | `/api/robot-commands/{id}` | Admin | One command |
| POST | `/api/robot-commands` | User | Create |
| PUT | `/api/robot-commands/{id}` | User | Update |
| DELETE | `/api/robot-commands/{id}` | Admin | Delete |

**Maps** (`/api/maps`)

| Method | Path | Access | Notes |
|---|---|---|---|
| GET | `/api/maps` | Public | All maps |
| GET | `/api/maps/square` | Public | Maps where rows = columns |
| GET | `/api/maps/{id}` | Public | One map |
| GET | `/api/maps/{id}/{x}-{y}` | Public | Is (x, y) on the map? |
| POST | `/api/maps` | User | Create; returns 201 with `Location` |
| PUT | `/api/maps/{id}` | User | Update |
| DELETE | `/api/maps/{id}` | Admin | Delete |

**Users** (`/users`)

| Method | Path | Access | Notes |
|---|---|---|---|
| POST | `/users` | Public | Sign up. New accounts get role `User`; only an admin can create an admin |
| GET | `/users`, `/users/admin`, `/users/{id}` | Admin | List all users, list admins, get one user |
| PUT | `/users/{id}` | User | Update name and description |
| PATCH | `/users/{id}` | User | Update email and password |
| DELETE | `/users/{id}` | Admin | Delete |

Password hashes are never included in responses. Swagger UI, generated from the XML doc
comments, is at `/swagger`.

## How it works

- **Authentication.** `Authentication/BasicAuthenticationHandler.cs` decodes the
  `Authorization: Basic` header and looks the user up by email. It verifies the
  password with `PasswordService` and issues `Name`, `Email` and `Role` claims.
- **Authorization.** `Program.cs` defines two policies on the role claim: `AdminOnly`
  and `UserOnly` (Admin or User).
- **Data access.** Npgsql with parameterised SQL throughout (no string concatenation),
  in `Persistance/` and the controllers. The schema is in `schema.sql`.

## Run locally

You need the .NET 6 SDK and PostgreSQL.

```bash
createdb sit331
psql -d sit331 -f schema.sql

dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Username=postgres;Password=<your-password>;Database=sit331"
dotnet user-secrets set "AppSettings:Pepper" "<random string>"
dotnet user-secrets set "AppSettings:Salt" "<random string>"

dotnet run        # Swagger UI at https://localhost:7166/swagger
```

### Configuration

Secrets are not stored in `appsettings.json`. Set them with user secrets, as above, or with
environment variables:

- `ConnectionStrings__DefaultConnection`
- `AppSettings__Pepper`
- `AppSettings__Salt`

The app refuses to start if any of them is missing. Changing the pepper or salt invalidates
existing password hashes.

### Creating the first admin

Sign up with `POST /users`, then promote that account in SQL:

```sql
UPDATE users SET role = 'Admin' WHERE email = 'you@example.com';
```

## Testing

`Minimal-robot-api.postman_collection.json` holds 20 requests covering every endpoint,
with status-code assertions. Run it against a local instance with Basic auth set to an
admin account:

```bash
npx newman run Minimal-robot-api.postman_collection.json -e Local.postman_environment.json -k
```

The stored run in `Minimal-robot-api.postman_test_run.json` (May 2025) recorded 45 passing
and 3 failing assertions. `newman-report.html` is the matching HTML report. GitHub Actions
builds the project on every push.

## Known limitations

- **Password hashing.** Passwords are hashed with a single SHA-256 over
  password + pepper + one static salt, which is fast and the same for every user. A
  per-user salted KDF would be the right fix. The Argon2 packages are referenced, and an
  Argon2 version is sketched in `AuthUtils/PasswordService.cs`.
- **User edits.** `PUT` and `PATCH /users/{id}` let any logged-in user edit any user;
  they should be limited to the account owner or an admin.
- **Transport.** Basic auth sends credentials on every request, so it is only safe over
  HTTPS.
- **Unused code.** `Persistance/UserRepository.cs`, `Persistance/RobotDbContext.cs` (EF Core)
  and `Helper/DbHelper.cs` are not used by the controllers.

## Author

Shrinivas Anavekar · MIT License
