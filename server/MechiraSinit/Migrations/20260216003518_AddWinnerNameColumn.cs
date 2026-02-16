using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MechiraSinit.Migrations
{
    /// <inheritdoc />
    public partial class AddWinnerNameColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WinnerUserId",
                table: "Gifts");

            migrationBuilder.AddColumn<string>(
                name: "WinnerName",
                table: "Gifts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Donors",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WinnerName",
                table: "Gifts");

            migrationBuilder.AddColumn<int>(
                name: "WinnerUserId",
                table: "Gifts",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Phone",
                table: "Donors",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
