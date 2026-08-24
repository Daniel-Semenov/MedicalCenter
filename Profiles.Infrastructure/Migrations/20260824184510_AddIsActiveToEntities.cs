using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Profiles.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveToEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Receptionists",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Patients",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Doctors",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Receptionists");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Doctors");
        }
    }
}
