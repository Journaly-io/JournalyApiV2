using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace JournalyApiV2.Data.Migrations
{
    /// <inheritdoc />
    public partial class _13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicationInstance_MedSchedule_ScheduleId",
                table: "MedicationInstance");

            migrationBuilder.DropForeignKey(
                name: "FK_MedScheduleDays_MedSchedule_MedScheduleId",
                table: "MedScheduleDays");

            migrationBuilder.DropIndex(
                name: "IX_MedScheduleDays_MedScheduleId",
                table: "MedScheduleDays");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MedSchedule",
                table: "MedSchedule");

            migrationBuilder.DropIndex(
                name: "IX_MedicationInstance_ScheduleId",
                table: "MedicationInstance");

            migrationBuilder.DropColumn(
                name: "MedScheduleId",
                table: "MedScheduleDays");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "MedSchedule");

            migrationBuilder.DropColumn(
                name: "ScheduleId",
                table: "MedicationInstance");

            migrationBuilder.AddColumn<Guid>(
                name: "MedScheduleUuid",
                table: "MedScheduleDays",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Uuid",
                table: "MedSchedule",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<Guid>(
                name: "ScheduleUuid",
                table: "MedicationInstance",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MedSchedule",
                table: "MedSchedule",
                column: "Uuid");

            migrationBuilder.CreateIndex(
                name: "IX_MedScheduleDays_MedScheduleUuid",
                table: "MedScheduleDays",
                column: "MedScheduleUuid");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationInstance_ScheduleUuid",
                table: "MedicationInstance",
                column: "ScheduleUuid");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicationInstance_MedSchedule_ScheduleUuid",
                table: "MedicationInstance",
                column: "ScheduleUuid",
                principalTable: "MedSchedule",
                principalColumn: "Uuid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MedScheduleDays_MedSchedule_MedScheduleUuid",
                table: "MedScheduleDays",
                column: "MedScheduleUuid",
                principalTable: "MedSchedule",
                principalColumn: "Uuid",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicationInstance_MedSchedule_ScheduleUuid",
                table: "MedicationInstance");

            migrationBuilder.DropForeignKey(
                name: "FK_MedScheduleDays_MedSchedule_MedScheduleUuid",
                table: "MedScheduleDays");

            migrationBuilder.DropIndex(
                name: "IX_MedScheduleDays_MedScheduleUuid",
                table: "MedScheduleDays");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MedSchedule",
                table: "MedSchedule");

            migrationBuilder.DropIndex(
                name: "IX_MedicationInstance_ScheduleUuid",
                table: "MedicationInstance");

            migrationBuilder.DropColumn(
                name: "MedScheduleUuid",
                table: "MedScheduleDays");

            migrationBuilder.DropColumn(
                name: "Uuid",
                table: "MedSchedule");

            migrationBuilder.AddColumn<int>(
                name: "MedScheduleId",
                table: "MedScheduleDays",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "MedSchedule",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<Guid>(
                name: "ScheduleUuid",
                table: "MedicationInstance",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<int>(
                name: "ScheduleId",
                table: "MedicationInstance",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MedSchedule",
                table: "MedSchedule",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_MedScheduleDays_MedScheduleId",
                table: "MedScheduleDays",
                column: "MedScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationInstance_ScheduleId",
                table: "MedicationInstance",
                column: "ScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicationInstance_MedSchedule_ScheduleId",
                table: "MedicationInstance",
                column: "ScheduleId",
                principalTable: "MedSchedule",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MedScheduleDays_MedSchedule_MedScheduleId",
                table: "MedScheduleDays",
                column: "MedScheduleId",
                principalTable: "MedSchedule",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
