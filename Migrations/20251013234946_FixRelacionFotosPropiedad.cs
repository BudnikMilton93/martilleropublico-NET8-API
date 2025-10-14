using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APITemplate.Migrations
{
    /// <inheritdoc />
    public partial class FixRelacionFotosPropiedad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.CreateTable(
                name: "BARRIOS",
                columns: table => new
                {
                    Id_barrio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ciudad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Provincia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BARRIOS", x => x.Id_barrio);
                });

            migrationBuilder.CreateTable(
                name: "USUARIOS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Id_Rol = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USUARIOS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PROPIEDADES",
                columns: table => new
                {
                    Id_propiedad = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id_tipo = table.Column<int>(type: "int", nullable: false),
                    Id_barrio = table.Column<int>(type: "int", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Subtitulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Direccion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Superficie_terreno = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Superficie_construida = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Antiguedad = table.Column<int>(type: "int", nullable: true),
                    Habitaciones = table.Column<int>(type: "int", nullable: true),
                    Sanitario = table.Column<int>(type: "int", nullable: true),
                    Cochera = table.Column<int>(type: "int", nullable: true),
                    Marca = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Modelo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Fabricacion = table.Column<int>(type: "int", nullable: true),
                    Kilometraje = table.Column<int>(type: "int", nullable: true),
                    Patente = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    EsDestacada = table.Column<bool>(type: "bit", nullable: false),
                    Servicios_incluidos = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Fecha_Alta = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PROPIEDADES", x => x.Id_propiedad);
                    table.ForeignKey(
                        name: "FK_PROPIEDADES_BARRIOS_Id_barrio",
                        column: x => x.Id_barrio,
                        principalTable: "BARRIOS",
                        principalColumn: "Id_barrio",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CARACTERISTICAS_PROPIEDAD",
                columns: table => new
                {
                    Id_caracteristica = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id_propiedad = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    PROPIEDADESId_propiedad = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CARACTERISTICAS_PROPIEDAD", x => x.Id_caracteristica);
                    table.ForeignKey(
                        name: "FK_CARACTERISTICAS_PROPIEDAD_PROPIEDADES_PROPIEDADESId_propiedad",
                        column: x => x.PROPIEDADESId_propiedad,
                        principalTable: "PROPIEDADES",
                        principalColumn: "Id_propiedad");
                });

            migrationBuilder.CreateTable(
                name: "FOTOS_PROPIEDAD",
                columns: table => new
                {
                    Id_foto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id_propiedad = table.Column<int>(type: "int", nullable: false),
                    Nombre_archivo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Ruta_archivo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Es_principal = table.Column<bool>(type: "bit", nullable: false),
                    Orden_visualizacion = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Fecha_subida = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    PROPIEDADESId_propiedad = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FOTOS_PROPIEDAD", x => x.Id_foto);
                    table.ForeignKey(
                        name: "FK_FOTOS_PROPIEDAD_PROPIEDADES_PROPIEDADESId_propiedad",
                        column: x => x.PROPIEDADESId_propiedad,
                        principalTable: "PROPIEDADES",
                        principalColumn: "Id_propiedad");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CARACTERISTICAS_PROPIEDAD_PROPIEDADESId_propiedad",
                table: "CARACTERISTICAS_PROPIEDAD",
                column: "PROPIEDADESId_propiedad");

            migrationBuilder.CreateIndex(
                name: "IX_FOTOS_PROPIEDAD_PROPIEDADESId_propiedad",
                table: "FOTOS_PROPIEDAD",
                column: "PROPIEDADESId_propiedad");

            migrationBuilder.CreateIndex(
                name: "IX_PROPIEDADES_Id_barrio",
                table: "PROPIEDADES",
                column: "Id_barrio");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CARACTERISTICAS_PROPIEDAD");

            migrationBuilder.DropTable(
                name: "FOTOS_PROPIEDAD");

            migrationBuilder.DropTable(
                name: "USUARIOS");

            migrationBuilder.DropTable(
                name: "PROPIEDADES");

            migrationBuilder.DropTable(
                name: "BARRIOS");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Apellido = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Id_Rol = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }
    }
}
