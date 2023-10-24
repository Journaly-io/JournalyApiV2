using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JournalyApiV2.Data.Migrations
{
    /// <inheritdoc />
    public partial class _16 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicationInstance_MedSchedule_ScheduleUuid",
                table: "MedicationInstance");

            migrationBuilder.AlterColumn<Guid>(
                name: "ScheduleUuid",
                table: "MedicationInstance",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicationInstance_MedSchedule_ScheduleUuid",
                table: "MedicationInstance",
                column: "ScheduleUuid",
                principalTable: "MedSchedule",
                principalColumn: "Uuid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicationInstance_MedSchedule_ScheduleUuid",
                table: "MedicationInstance");

            migrationBuilder.AlterColumn<Guid>(
                name: "ScheduleUuid",
                table: "MedicationInstance",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicationInstance_MedSchedule_ScheduleUuid",
                table: "MedicationInstance",
                column: "ScheduleUuid",
                principalTable: "MedSchedule",
                principalColumn: "Uuid",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
