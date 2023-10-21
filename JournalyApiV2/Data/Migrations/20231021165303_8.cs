using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace JournalyApiV2.Data.Migrations
{
    /// <inheritdoc />
    public partial class _8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SyncedRecords_RecordType_RecordType",
                table: "SyncedRecords");

            migrationBuilder.RenameIndex("IX_SyncedRecords_RecordType", "IX_SyncedRecords_RecordTypeId",
                "SyncedRecords");

            migrationBuilder.RenameColumn("RecordType", "SyncedRecords", "RecordTypeId");

            migrationBuilder.AddForeignKey("FK_SyncedRecords_RecordType_RecordTypeId", "SyncedRecords", "RecordTypeId",
                "RecordType", principalColumn: "Id");

            migrationBuilder.AddColumn<bool>(
                name: "IsVoid",
                table: "SyncedRecords",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVoid",
                table: "SyncedRecords");

            migrationBuilder.DropColumn(
                name: "RecordTypeId",
                table: "SyncedRecords");

            migrationBuilder.AddColumn<short>(
                name: "RecordType",
                table: "SyncedRecords",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.CreateTable(
                name: "RecordType",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordType", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SyncedRecords_RecordType",
                table: "SyncedRecords",
                column: "RecordType");

            migrationBuilder.AddForeignKey(
                name: "FK_SyncedRecords_RecordType_RecordType",
                table: "SyncedRecords",
                column: "RecordType",
                principalTable: "RecordType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
