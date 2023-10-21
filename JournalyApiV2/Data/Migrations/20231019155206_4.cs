using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace JournalyApiV2.Data.Migrations
{
    /// <inheritdoc />
    public partial class _4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            // Seed the IconType table with default values
            migrationBuilder.InsertData(
                table: "RecordType",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "Activity" }
            );
            
            migrationBuilder.InsertData(
                table: "RecordType",
                columns: new[] { "Id", "Name" },
                values: new object[] { 2, "JournalEntry" }
            );
            
            migrationBuilder.InsertData(
                table: "RecordType",
                columns: new[] { "Id", "Name" },
                values: new object[] { 3, "Emotion" }
            );
            
            migrationBuilder.InsertData(
                table: "RecordType",
                columns: new[] { "Id", "Name" },
                values: new object[] { 4, "Category" }
            );
            
            migrationBuilder.CreateTable(
                name: "SyncedRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DeviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecordId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecordType = table.Column<short>(type: "smallint", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SyncedRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SyncedRecords_RecordType_RecordType",
                        column: x => x.RecordType,
                        principalTable: "RecordType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SyncedRecords_RecordType",
                table: "SyncedRecords",
                column: "RecordType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SyncedRecords");

            migrationBuilder.DropTable(
                name: "RecordType");
        }
    }
}
