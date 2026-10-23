using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EngCalc.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialProduction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Materials",
                columns: table => new
                {
                    MaterialId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Specification = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Grade = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ProductForm = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    MinTensileStrength = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinYieldStrength = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaxTemperatureLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materials", x => x.MaterialId);
                });

            migrationBuilder.CreateTable(
                name: "MaterialsTemperatureProperties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaterialId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Temperature = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AllowableStress = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    YieldStress = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialsTemperatureProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialsTemperatureProperties_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "MaterialId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaterialsTemperatureProperties_MaterialId_Temperature",
                table: "MaterialsTemperatureProperties",
                columns: new[] { "MaterialId", "Temperature" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MaterialsTemperatureProperties");

            migrationBuilder.DropTable(
                name: "Materials");
        }
    }
}

