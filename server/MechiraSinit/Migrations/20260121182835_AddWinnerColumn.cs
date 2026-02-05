using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MechiraSinit.Migrations
{
    /// <inheritdoc />
    public partial class AddWinnerColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsPurchased",
                table: "Purchases",
                newName: "IsPaid");

            migrationBuilder.AddColumn<DateTime>(
                name: "PurchaseDate",
                table: "Purchases",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "WinnerUserId",
                table: "Gifts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PurchaseDate",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "WinnerUserId",
                table: "Gifts");

            migrationBuilder.RenameColumn(
                name: "IsPaid",
                table: "Purchases",
                newName: "IsPurchased");
        }
    }
}
