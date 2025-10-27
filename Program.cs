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
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy
                .WithOrigins("https://www.zmpropiedades.com", "http://localhost:4200") // <-- dirección de tu frontend
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

#endregion


#region Servicios básicos
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddAuthorization();
builder.Services.AddControllers();

builder.Services.AddSingleton<ICacheService, CacheService>();
#endregion

#region Redis

try
{
    // Configuración de Redis
    var redisConfig = new ConfigurationOptions
    {
        EndPoints = { "martillero-redis.redis.cache.windows.net:6380" },
        Password = builder.Configuration["Redis:Password"], // la tomamos desde variables de entorno
        Ssl = true,
        AbortOnConnectFail = false
    };

    // Registrar Redis como Singleton
    builder.Services.AddSingleton<IConnectionMultiplexer>(
        ConnectionMultiplexer.Connect(redisConfig)
    );
}
catch (Exception ex)
{
    Console.WriteLine("Redis no disponible: " + ex.Message);
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
