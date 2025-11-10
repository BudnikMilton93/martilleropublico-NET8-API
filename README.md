# 🧭 Martillero Público API

API REST desarrollada en **.NET 8 (ASP.NET Core Web API)** para la gestión de operaciones del sistema **Martillero Público**.  
Incluye autenticación mediante **JWT**, cache con **Redis**, y despliegue automatizado en **Azure** usando **Docker** y **CI/CD**.

---

## 🧱 Tech Stack

| Tecnología | Descripción |
|-------------|--------------|
| **.NET 8 (ASP.NET Core)** | Framework backend principal |
| **C#** | Lenguaje de programación utilizado |
| **SQL Server (Azure)** | Base de datos relacional desplegada en Azure |
| **Entity Framework Core 8** | ORM para mapeo de entidades y manejo de datos |
| **Redis (Azure Cache for Redis)** | Cache en memoria para optimizar rendimiento |
| **JWT Authentication** | Sistema de autenticación segura basada en tokens |
| **SHA256 / bcrypt** | Hashing y protección de contraseñas |
| **HTTPS + SSL/TLS** | Comunicación segura entre cliente y servidor |
| **Swagger / OpenAPI** | Documentación interactiva de endpoints |
| **Docker + Azure Container Registry (ACR)** | Contenerización y almacenamiento de imágenes |
| **Azure App Service** | Ejecución del contenedor en producción |

---

## ⚙️ Pre-requisitos

- **Docker Desktop** instalado (sin necesidad de `sudo`).
- **Docker Compose** instalado.
- **Puertos libres:** `8080`, `1433`, `6379`.
- **Acceso a Azure App Service** y **Azure Container Registry**.
- **Conexión a SQL Server** configurada (local o Azure SQL Database).

---

## 📂 Estructura del Proyecto
<pre> ```MartilleroPublico.API/
│
├── Bussines/
│   └── DTOs/
│   └── Interfaces/
│       └── IFotosPropiedadService.cs
│       └── IPropiedadesService.cs
│   └── Services/
│       └── AuthService.cs
│       └── FotosPropiedadService.cs
│       └── JwtService.cs
│       └── PropiedadesService.cs
│       └── S3Service.cs
│
├── Controllers/
│ └── AuthController.cs
│ └── CacheTestController.cs
│ └── PropiedadesController.cs
│
├── Data/
│   └── Interfaces/
│       └── IBaseRepository.cs
│       └── IFotosPropiedadRepository.cs
│       └── IPropiedadesRepository.cs
│       └── IUsuarioRepository.cs
│   └── Repositories/
│       └── IBaseRepository.cs
│       └── IFotosPropiedadRepository.cs
│       └── IPropiedadesRepository.cs
│       └── IUsuarioRepository.cs
│ └── AppDbContext.cs
│
├── Helpers/
│ └── CacheService.cs
│ └── DbConnectionHelper.cs
│ └── ICacheService.cs
│ └── PasswordHasher.cs
│ └── RefreshToken.cs
│
├── Models/
│ └── Modelos.cs
│
├── Services/
│   └── Interfaces/
│       └── IAuthService.cs
│       └── IJwtService.cs
│
├── Test/
│   └── Helpers/
│       └── PasswordHasherTests.cs
│
├── appsettings.json
├── appsettings.Development.json
├── Program.cs
├── Dockerfile
└── docker-compose.yml
``` </pre>
---

## 🐳 Cómo ejecutar la app localmente

### Con Docker
bash
docker build -t martilleroacr2025.azurecr.io/martilleropublico-api:latest .
docker run -d -p 8080:8080 martilleroacr2025.azurecr.io/martilleropublico-api:latest

Esto levantará:
  - API en http://localhost:8080
  - Redis en localhost:6379
  - SQL Server en localhost:1433

---

## 🧰 Variables de entorno

ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__DefaultConnection="Server=tcp:martilleropublico.database.windows.net,1433;Initial Catalog=MartilleroDB;Persist Security Info=False;User ID=admin;Password=********;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
Redis__ConnectionString="martillero-redis.redis.cache.windows.net:6380,password=********,ssl=True,abortConnect=False"
Jwt__Key="clave-secreta-para-jwt"
Jwt__Issuer="MartilleroPublicoAPI"
Jwt__Audience="MartilleroPublicoClient"

---

## 🔑 Autenticación

La API utiliza JWT (JSON Web Tokens) para el manejo de sesiones y autorización.
  - Login Endpoint: /api/auth/login
  - Header:
      Authorization: Bearer <token>
  - Renovación automática: mediante validación del token activo.

---

## 🧠 Comandos Docker importantes

Actualizar imagen y reiniciar contenedor:
docker build -t martilleroacr2025.azurecr.io/martilleropublico-api:latest .
docker push martilleroacr2025.azurecr.io/martilleropublico-api:latest
docker restart martillero-api

---

## ☁️ Despliegue en Azure

Subir imagen a Azure Container Registry (ACR):
docker login martilleroacr2025.azurecr.io
docker push martilleroacr2025.azurecr.io/martilleropublico-api:latest

---

## 💡 Decisiones técnicas

  - Clean Architecture + SOLID: para lograr mantenibilidad y separación de responsabilidades.
  - Redis Cache: reducir tiempos de respuesta y mejorar eficiencia.
  - Entity Framework Core: por integración nativa con .NET y facilidad de mantenimiento.
  - Docker: para asegurar portabilidad entre entornos.
  - HTTPS + JWT: para garantizar seguridad en las comunicaciones y autenticación.

---

## 🧩 Áreas de mejora

- Implementar migraciones automáticas para la base de datos.
- Mejorar manejo de errores con middleware global.
- Añadir logging estructurado con Serilog o Application Insights.
- Integrar seed data para pruebas locales.
- Incluir test de integración y E2E con escenarios reales.
- Bitbucket + Azure DevOps: CI/CD y control de versiones.
- Frameworks para testing unitario: xUnit / MSTest / NUnit. 	
