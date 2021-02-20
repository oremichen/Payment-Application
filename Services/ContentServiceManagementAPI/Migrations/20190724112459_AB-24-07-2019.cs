using Microsoft.EntityFrameworkCore.Migrations;

namespace ContentServiceManagementAPI.Migrations
{
    public partial class AB24072019 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Frequency",
                table: "Service",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "Periodicity",
                table: "Service",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServiceStatus",
                table: "Service",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SubscriptionType",
                table: "Service",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Frequency",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "Periodicity",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "ServiceStatus",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "SubscriptionType",
                table: "Service");
        }
    }
}
