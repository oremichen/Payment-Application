using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ContentServiceManagementAPI.Migrations
{
    public partial class ab20190805 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "VasLicenseActiveDate",
                table: "ServiceProvider",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VasLicenseExpiryDate",
                table: "ServiceProvider",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VasLicenseId",
                table: "ServiceProvider",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VasLicenseActiveDate",
                table: "ServiceProvider");

            migrationBuilder.DropColumn(
                name: "VasLicenseExpiryDate",
                table: "ServiceProvider");

            migrationBuilder.DropColumn(
                name: "VasLicenseId",
                table: "ServiceProvider");
        }
    }
}
