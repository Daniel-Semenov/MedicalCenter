using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Appointments.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppointmentResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AppointmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Complaints = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Conclusion = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Recommendations = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentResults", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    SpecializationId = table.Column<Guid>(type: "uuid", nullable: true),
                    OfficeId = table.Column<Guid>(type: "uuid", nullable: false),
                    AppointmentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DoctorSnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MiddleName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    SpecializationId = table.Column<Guid>(type: "uuid", nullable: true),
                    OfficeId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsAtWork = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorSnapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OfficeSnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfficeSnapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PatientSnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MiddleName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientSnapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceSnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SpecializationId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceSnapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SpecializationSnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecializationSnapshots", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentResults_AppointmentId",
                table: "AppointmentResults",
                column: "AppointmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_DoctorId",
                table: "Appointments",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_DoctorId_AppointmentDate_StartTime",
                table: "Appointments",
                columns: new[] { "DoctorId", "AppointmentDate", "StartTime" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_OfficeId",
                table: "Appointments",
                column: "OfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PatientId",
                table: "Appointments",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ServiceId",
                table: "Appointments",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorSnapshots_OfficeId",
                table: "DoctorSnapshots",
                column: "OfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorSnapshots_SpecializationId",
                table: "DoctorSnapshots",
                column: "SpecializationId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceSnapshots_SpecializationId",
                table: "ServiceSnapshots",
                column: "SpecializationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppointmentResults");

            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "DoctorSnapshots");

            migrationBuilder.DropTable(
                name: "OfficeSnapshots");

            migrationBuilder.DropTable(
                name: "PatientSnapshots");

            migrationBuilder.DropTable(
                name: "ServiceSnapshots");

            migrationBuilder.DropTable(
                name: "SpecializationSnapshots");
        }
    }
}
