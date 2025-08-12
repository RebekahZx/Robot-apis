using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using robot_controller_api.Authentication;
using robot_controller_api.Helper; 
using robot_controller_api.AuthUtils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSingleton<DbHelper>();
builder.Services.AddSingleton<PasswordService>();
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));


// Add authentication services
builder.Services.AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

// Add configuration services
var config = builder.Configuration;
builder.Services.AddSingleton<IConfiguration>(config);

// Add Swagger generation services
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Robot Controller API",
        Description = "New backend service.",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Shrinivas Anavekar",
            Email = "s225255918@deakin.edu.au"
        }
    });

    // Enable XML comments for Swagger
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});


 builder.Services.AddAuthorization(options =>
        {

            options.AddPolicy("AdminOnly", policy =>
                policy.RequireClaim(ClaimTypes.Role, "Admin"));


            options.AddPolicy("UserOnly", policy =>
                policy.RequireClaim(ClaimTypes.Role, "Admin", "User"));
                
               

            //     options.AddPolicy("DeveloperOnly", policy =>
            // policy.RequireClaim("developer", "true","Admin"));


        });
        
var app = builder.Build();


app.UseStaticFiles();


app.UseSwagger();
app.UseSwaggerUI(setup => setup.InjectStylesheet("/styles/theme-muted.css"));


app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();


app.MapControllers();


app.Run();
