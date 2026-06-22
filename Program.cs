using APITemplate.Business.Interfaces;
using APITemplate.Business.Services;
using APITemplate.Bussines.Interfaces;
using APITemplate.Bussines.Services;
using APITemplate.Data;
using APITemplate.Data.Interfaces;
using APITemplate.Data.Repositories;
using APITemplate.Services;
using APITemplate.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

#region Ambientes
// Configuración unificada para todos los entornos
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables(); // Permite leer desde variables de entorno (PRD)

// Solo usar secretos locales en desarrollo
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

#endregion


#region Culture Info
var cultureInfo = new CultureInfo("es-AR"); 
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
#endregion


#region CORS
var allowedOrigins = builder.Configuration.GetSection("Frontend:AllowedOrigins").Get<string[]>();
var allowedOriginsCsv = builder.Configuration["Frontend:AllowedOriginsCsv"];

if (!string.IsNullOrWhiteSpace(allowedOriginsCsv))
{
    allowedOrigins = allowedOriginsCsv
        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .Where(origin => !string.IsNullOrWhiteSpace(origin))
        .ToArray();
}

allowedOrigins ??=
[
    "https://www.zmpropiedades.com",
    "http://localhost:4200"
];

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy
                .WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

#endregion


#region Servicios b�sicos
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddAuthorization();
builder.Services.AddControllers();
#endregion

#region Redis
var redisConnectionString =
    builder.Configuration.GetConnectionString("Redis")
    ?? builder.Configuration["Redis:ConnectionString"];

if (!string.IsNullOrWhiteSpace(redisConnectionString))
{
    try
    {
        builder.Services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(redisConnectionString)
        );
        builder.Services.AddSingleton<ICacheService, CacheService>();
        Console.WriteLine("Redis habilitado para cache distribuida.");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Redis no disponible, usando cache en memoria local: " + ex.Message);
        builder.Services.AddMemoryCache();
        builder.Services.AddSingleton<ICacheService, InMemoryCacheService>();
    }
}
else
{
    Console.WriteLine("Redis no configurado, usando cache en memoria local.");
    builder.Services.AddMemoryCache();
    builder.Services.AddSingleton<ICacheService, InMemoryCacheService>();
}

#endregion 


#region JWT Authentication

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        NameClaimType = JwtRegisteredClaimNames.Name
    };
});


#endregion


#region Conexión a base de datos y repositorios
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IPropiedadesRepository, PropiedadesRepository>();
builder.Services.AddScoped<IFotosPropiedadRepository, FotosPropiedadRepository>();
#endregion


#region Servicios
builder.Services.AddScoped<IPropiedadesService, PropiedadesService>();
builder.Services.AddScoped<IFotosPropiedadService, FotosPropiedadService>(); ;
builder.Services.AddScoped<S3Service>();
#endregion


var app = builder.Build();

#region Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
#endregion

app.Run();
