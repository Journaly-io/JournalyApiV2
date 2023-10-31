using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JournalyApiV2.Data.Migrations
{
    /// <inheritdoc />
    public partial class VerificationCodes2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VerificationCode",
                table: "EmailVerificationCode",
                newName: "ShortCode");

            migrationBuilder.AddColumn<string>(
                name: "LongCode",
                table: "EmailVerificationCode",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LongCode",
                table: "EmailVerificationCode");

            migrationBuilder.RenameColumn(
                name: "ShortCode",
                table: "EmailVerificationCode",
                newName: "VerificationCode");
        }
    }
}
