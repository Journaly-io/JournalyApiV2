using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace JournalyApiV2.Data.Migrations
{
    /// <inheritdoc />
    public partial class _5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Day",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Day", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Day",
                columns: new[] { "Id", "Name" },
                values: new object[] { 0, "Sunday" }
            );
            migrationBuilder.InsertData(
                table: "Day",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "Monday" }
            );
            migrationBuilder.InsertData(
                table: "Day",
                columns: new[] { "Id", "Name" },
                values: new object[] { 2, "Tuesday" }
            );
            migrationBuilder.InsertData(
                table: "Day",
                columns: new[] { "Id", "Name" },
                values: new object[] { 3, "Wednesday" }
            );
            migrationBuilder.InsertData(
                table: "Day",
                columns: new[] { "Id", "Name" },
                values: new object[] { 4, "Thursday" }
            );
            migrationBuilder.InsertData(
                table: "Day",
                columns: new[] { "Id", "Name" },
                values: new object[] { 5, "Friday" }
            );
            migrationBuilder.InsertData(
                table: "Day",
                columns: new[] { "Id", "Name" },
                values: new object[] { 6, "Saturday" }
            );
            
            migrationBuilder.CreateTable(
                name: "MedUnit",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedUnit", x => x.Id);
                });
            
            migrationBuilder.InsertData(
                table: "MedUnit",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "mg" }
            );
            migrationBuilder.InsertData(
                table: "MedUnit",
                columns: new[] { "Id", "Name" },
                values: new object[] { 2, "g" }
            );
            migrationBuilder.InsertData(
                table: "MedUnit",
                columns: new[] { "Id", "Name" },
                values: new object[] { 3, "ug" }
            );
            migrationBuilder.InsertData(
                table: "MedUnit",
                columns: new[] { "Id", "Name" },
                values: new object[] { 4, "drops" }
            );
            migrationBuilder.InsertData(
                table: "MedUnit",
                columns: new[] { "Id", "Name" },
                values: new object[] { 5, "capsules" }
            );
            migrationBuilder.InsertData(
                table: "MedUnit",
                columns: new[] { "Id", "Name" },
                values: new object[] { 6, "chewables" }
            );
            migrationBuilder.InsertData(
                table: "MedUnit",
                columns: new[] { "Id", "Name" },
                values: new object[] { 7, "pills" }
            );
            migrationBuilder.InsertData(
                table: "MedUnit",
                columns: new[] { "Id", "Name" },
                values: new object[] { 8, "oz" }
            );
            
            migrationBuilder.CreateTable(
                name: "Medication",
                columns: table => new
                {
                    Uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: false),
                    Unit = table.Column<short>(type: "smallint", nullable: false),
                    DefaultDose = table.Column<short>(type: "smallint", nullable: false),
                    From = table.Column<DateOnly>(type: "date", nullable: false),
                    Until = table.Column<DateOnly>(type: "date", nullable: false),
                    Forever = table.Column<bool>(type: "boolean", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medication", x => x.Uuid);
                    table.ForeignKey(
                        name: "FK_Medication_MedUnit_Unit",
                        column: x => x.Unit,
                        principalTable: "MedUnit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MedSchedule",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Time = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    EveryOtherDay = table.Column<bool>(type: "boolean", nullable: false),
                    MedicationUuid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedSchedule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedSchedule_Medication_MedicationUuid",
                        column: x => x.MedicationUuid,
                        principalTable: "Medication",
                        principalColumn: "Uuid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MedScheduleDays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MedScheduleId = table.Column<int>(type: "integer", nullable: false),
                    Day = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedScheduleDays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedScheduleDays_Day_Day",
                        column: x => x.Day,
                        principalTable: "Day",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MedScheduleDays_MedSchedule_MedScheduleId",
                        column: x => x.MedScheduleId,
                        principalTable: "MedSchedule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Medication_Unit",
                table: "Medication",
                column: "Unit");

            migrationBuilder.CreateIndex(
                name: "IX_MedSchedule_MedicationUuid",
                table: "MedSchedule",
                column: "MedicationUuid");

            migrationBuilder.CreateIndex(
                name: "IX_MedScheduleDays_Day",
                table: "MedScheduleDays",
                column: "Day");

            migrationBuilder.CreateIndex(
                name: "IX_MedScheduleDays_MedScheduleId",
                table: "MedScheduleDays",
                column: "MedScheduleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MedScheduleDays");

            migrationBuilder.DropTable(
                name: "Day");

            migrationBuilder.DropTable(
                name: "MedSchedule");

            migrationBuilder.DropTable(
                name: "Medication");

            migrationBuilder.DropTable(
                name: "MedUnit");
        }
    }
}
