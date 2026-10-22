using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EngCalc.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMaxTemperatureColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MaxTemperatureLimit",
                table: "Materials",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxTemperatureLimit",
                table: "Materials");
        }
    }
}

