using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace JournalyApiV2.Data.Migrations
{
    /// <inheritdoc />
    public partial class _9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MedStatus",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedStatus", x => x.Id);
                });
            
            migrationBuilder.InsertData(
                table: "MedStatus",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "Upcoming" }
            );
            
            migrationBuilder.InsertData(
                table: "MedStatus",
                columns: new[] { "Id", "Name" },
                values: new object[] { 2, "Missed" }
            );
            
            migrationBuilder.InsertData(
                table: "MedStatus",
                columns: new[] { "Id", "Name" },
                values: new object[] { 3, "Skipped" }
            );
            
            migrationBuilder.InsertData(
                table: "MedStatus",
                columns: new[] { "Id", "Name" },
                values: new object[] { 4, "Taken" }
            );
            
            migrationBuilder.CreateTable(
                name: "MedicationInstance",
                columns: table => new
                {
                    Uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    MedicationUuid = table.Column<Guid>(type: "uuid", nullable: false),
                    ScheduleUuid = table.Column<Guid>(type: "uuid", nullable: true),
                    Dose = table.Column<short>(type: "smallint", nullable: false),
                    ScheduledTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActualTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MedStatus = table.Column<short>(type: "smallint", nullable: false),
                    ScheduleId = table.Column<int>(type: "integer", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    Owner = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicationInstance", x => x.Uuid);
                    table.ForeignKey(
                        name: "FK_MedicationInstance_MedSchedule_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "MedSchedule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MedicationInstance_MedStatus_MedStatus",
                        column: x => x.MedStatus,
                        principalTable: "MedStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MedicationInstance_Medication_MedicationUuid",
                        column: x => x.MedicationUuid,
                        principalTable: "Medication",
                        principalColumn: "Uuid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MedicationInstance_MedicationUuid",
                table: "MedicationInstance",
                column: "MedicationUuid");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationInstance_MedStatus",
                table: "MedicationInstance",
                column: "MedStatus");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationInstance_ScheduleId",
                table: "MedicationInstance",
                column: "ScheduleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MedicationInstance");

            migrationBuilder.DropTable(
                name: "MedStatus");
        }
    }
}
