using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ContentServiceManagementAPI.Migrations
{
    public partial class serviceupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ActiveDate",
                table: "Service",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Caategory",
                table: "Service",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Channel",
                table: "Service",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDate",
                table: "Service",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Industry",
                table: "Service",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Keyword",
                table: "Service",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Service",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ServiceCode",
                table: "Service",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShortCode",
                table: "Service",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActiveDate",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "Caategory",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "Channel",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "Industry",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "Keyword",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "ServiceCode",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "ShortCode",
                table: "Service");
        }
    }
}
