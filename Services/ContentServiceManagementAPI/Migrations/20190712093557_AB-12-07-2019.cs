using Microsoft.EntityFrameworkCore.Migrations;

namespace ContentServiceManagementAPI.Migrations
{
    public partial class AB12072019 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ApplicationId",
                table: "ServiceProvider",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ApplicationId",
                table: "Service",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ApplicationId",
                table: "ContentProvider",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ApplicationId",
                table: "Content",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ApplicationId",
                table: "Client",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationId",
                table: "ServiceProvider");

            migrationBuilder.DropColumn(
                name: "ApplicationId",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "ApplicationId",
                table: "ContentProvider");

            migrationBuilder.DropColumn(
                name: "ApplicationId",
                table: "Content");

            migrationBuilder.DropColumn(
                name: "ApplicationId",
                table: "Client");
        }
    }
}
