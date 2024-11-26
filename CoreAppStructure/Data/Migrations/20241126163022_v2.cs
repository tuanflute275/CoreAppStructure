using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoreAppStructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "nvarchar(max)",
                table: "Tokens",
                newName: "Token");

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "Tokens",
                type: "ntext",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Token",
                table: "Tokens",
                newName: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "nvarchar(max)",
                table: "Tokens",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "ntext",
                oldMaxLength: 255);
        }
    }
}
