using APITemplate.Business.Interfaces;
using APITemplate.Business.Services;
using APITemplate.Bussines.Interfaces;
using APITemplate.Bussines.Services;
using APITemplate.Data;
using APITemplate.Data.Interfaces;
using APITemplate.Data.Repositories;
using APITemplate.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets<Program>();

#region Culture Info
var cultureInfo = new CultureInfo("es-AR"); 
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
#endregion

#region CORS
//TODO: Consultar al chat acerca de esto en PRD
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:4200") // <-- dirección de tu frontend
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});
#endregion

#region Servicios básicos
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<AuthService>();
builder.Services.AddAuthorization();
builder.Services.AddControllers();

// Redis cache
builder.Services.AddSingleton<ICacheService, CacheService>();
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")));
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
#endregion

app.UseCors("AllowFrontend");
app.Run();
