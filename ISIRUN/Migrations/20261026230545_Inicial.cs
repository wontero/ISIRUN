using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISIRUN.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Carreras",
                columns: table => new
                {
                    IdCarrera = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Distancia = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    PrecioInscripcion = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carreras", x => x.IdCarrera);
                });

            migrationBuilder.CreateTable(
                name: "Equipos",
                columns: table => new
                {
                    IdEquipo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipos", x => x.IdEquipo);
                });

            migrationBuilder.CreateTable(
                name: "Organizaciones",
                columns: table => new
                {
                    IdOrganizacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Actividad = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false),
                    ContrasenaHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizaciones", x => x.IdOrganizacion);
                });

            migrationBuilder.CreateTable(
                name: "CategoriasEdad",
                columns: table => new
                {
                    IdCategoria = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCarrera = table.Column<int>(type: "int", nullable: false),
                    EdadMinima = table.Column<int>(type: "int", nullable: false),
                    EdadMaxima = table.Column<int>(type: "int", nullable: false),
                    Premios = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriasEdad", x => x.IdCategoria);
                    table.CheckConstraint("CK_CategoriasEdad_Rango", "[EdadMinima] >= 0 AND [EdadMinima] <= [EdadMaxima]");
                    table.ForeignKey(
                        name: "FK_CategoriasEdad_Carreras_IdCarrera",
                        column: x => x.IdCarrera,
                        principalTable: "Carreras",
                        principalColumn: "IdCarrera",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Corredores",
                columns: table => new
                {
                    IdCorredor = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false),
                    ContrasenaHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FechaNac = table.Column<DateTime>(type: "date", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KmsTotales = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    IdEquipo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Corredores", x => x.IdCorredor);
                    table.ForeignKey(
                        name: "FK_Corredores_Equipos_IdEquipo",
                        column: x => x.IdEquipo,
                        principalTable: "Equipos",
                        principalColumn: "IdEquipo",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "OrgCarreras",
                columns: table => new
                {
                    IdCarrera = table.Column<int>(type: "int", nullable: false),
                    IdOrganizacion = table.Column<int>(type: "int", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "date", nullable: false),
                    TamanoEquipo = table.Column<int>(type: "int", nullable: false),
                    EsCreadora = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrgCarreras", x => new { x.IdCarrera, x.IdOrganizacion });
                    table.ForeignKey(
                        name: "FK_OrgCarreras_Carreras_IdCarrera",
                        column: x => x.IdCarrera,
                        principalTable: "Carreras",
                        principalColumn: "IdCarrera",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrgCarreras_Organizaciones_IdOrganizacion",
                        column: x => x.IdOrganizacion,
                        principalTable: "Organizaciones",
                        principalColumn: "IdOrganizacion",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sponsors",
                columns: table => new
                {
                    IdCarrera = table.Column<int>(type: "int", nullable: false),
                    IdOrganizacion = table.Column<int>(type: "int", nullable: false),
                    MontoAportado = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    TipoParticipacion = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sponsors", x => new { x.IdCarrera, x.IdOrganizacion });
                    table.ForeignKey(
                        name: "FK_Sponsors_Carreras_IdCarrera",
                        column: x => x.IdCarrera,
                        principalTable: "Carreras",
                        principalColumn: "IdCarrera",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sponsors_Organizaciones_IdOrganizacion",
                        column: x => x.IdOrganizacion,
                        principalTable: "Organizaciones",
                        principalColumn: "IdOrganizacion",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Actividades",
                columns: table => new
                {
                    IdActividad = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCorredor = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    DistanciaKm = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    Tiempo = table.Column<TimeSpan>(type: "time", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Notas = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Fecha = table.Column<DateTime>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actividades", x => x.IdActividad);
                    table.ForeignKey(
                        name: "FK_Actividades_Corredores_IdCorredor",
                        column: x => x.IdCorredor,
                        principalTable: "Corredores",
                        principalColumn: "IdCorredor",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Amistades",
                columns: table => new
                {
                    IdCorredor = table.Column<int>(type: "int", nullable: false),
                    IdAmigo = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Amistades", x => new { x.IdCorredor, x.IdAmigo });
                    table.CheckConstraint("CK_Amistades_PersonasDistintas", "[IdCorredor] <> [IdAmigo]");
                    table.ForeignKey(
                        name: "FK_Amistades_Corredores_IdAmigo",
                        column: x => x.IdAmigo,
                        principalTable: "Corredores",
                        principalColumn: "IdCorredor");
                    table.ForeignKey(
                        name: "FK_Amistades_Corredores_IdCorredor",
                        column: x => x.IdCorredor,
                        principalTable: "Corredores",
                        principalColumn: "IdCorredor",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Resultados",
                columns: table => new
                {
                    IdCorredor = table.Column<int>(type: "int", nullable: false),
                    IdCarrera = table.Column<int>(type: "int", nullable: false),
                    IdCategoria = table.Column<int>(type: "int", nullable: false),
                    Tiempo = table.Column<TimeSpan>(type: "time", nullable: true),
                    Ranking = table.Column<int>(type: "int", nullable: true),
                    FechaInscripcion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resultados", x => new { x.IdCorredor, x.IdCarrera });
                    table.ForeignKey(
                        name: "FK_Resultados_Carreras_IdCarrera",
                        column: x => x.IdCarrera,
                        principalTable: "Carreras",
                        principalColumn: "IdCarrera",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Resultados_CategoriasEdad_IdCategoria",
                        column: x => x.IdCategoria,
                        principalTable: "CategoriasEdad",
                        principalColumn: "IdCategoria");
                    table.ForeignKey(
                        name: "FK_Resultados_Corredores_IdCorredor",
                        column: x => x.IdCorredor,
                        principalTable: "Corredores",
                        principalColumn: "IdCorredor",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LikesActividad",
                columns: table => new
                {
                    IdActividad = table.Column<int>(type: "int", nullable: false),
                    IdCorredor = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LikesActividad", x => new { x.IdActividad, x.IdCorredor });
                    table.ForeignKey(
                        name: "FK_LikesActividad_Actividades_IdActividad",
                        column: x => x.IdActividad,
                        principalTable: "Actividades",
                        principalColumn: "IdActividad",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LikesActividad_Corredores_IdCorredor",
                        column: x => x.IdCorredor,
                        principalTable: "Corredores",
                        principalColumn: "IdCorredor");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Actividades_IdCorredor",
                table: "Actividades",
                column: "IdCorredor");

            migrationBuilder.CreateIndex(
                name: "IX_Amistades_IdAmigo",
                table: "Amistades",
                column: "IdAmigo");

            migrationBuilder.CreateIndex(
                name: "IX_CategoriasEdad_IdCarrera",
                table: "CategoriasEdad",
                column: "IdCarrera");

            migrationBuilder.CreateIndex(
                name: "IX_Corredores_Email",
                table: "Corredores",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Corredores_IdEquipo",
                table: "Corredores",
                column: "IdEquipo");

            migrationBuilder.CreateIndex(
                name: "IX_LikesActividad_IdCorredor",
                table: "LikesActividad",
                column: "IdCorredor");

            migrationBuilder.CreateIndex(
                name: "IX_Organizaciones_Email",
                table: "Organizaciones",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrgCarreras_IdOrganizacion",
                table: "OrgCarreras",
                column: "IdOrganizacion");

            migrationBuilder.CreateIndex(
                name: "IX_Resultados_IdCarrera",
                table: "Resultados",
                column: "IdCarrera");

            migrationBuilder.CreateIndex(
                name: "IX_Resultados_IdCategoria",
                table: "Resultados",
                column: "IdCategoria");

            migrationBuilder.CreateIndex(
                name: "IX_Sponsors_IdOrganizacion",
                table: "Sponsors",
                column: "IdOrganizacion");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Amistades");

            migrationBuilder.DropTable(
                name: "LikesActividad");

            migrationBuilder.DropTable(
                name: "OrgCarreras");

            migrationBuilder.DropTable(
                name: "Resultados");

            migrationBuilder.DropTable(
                name: "Sponsors");

            migrationBuilder.DropTable(
                name: "Actividades");

            migrationBuilder.DropTable(
                name: "CategoriasEdad");

            migrationBuilder.DropTable(
                name: "Organizaciones");

            migrationBuilder.DropTable(
                name: "Corredores");

            migrationBuilder.DropTable(
                name: "Carreras");

            migrationBuilder.DropTable(
                name: "Equipos");
        }
    }
}
