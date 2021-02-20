using Microsoft.EntityFrameworkCore.Migrations;

namespace ContentServiceManagementAPI.Migrations
{
    public partial class ab20190731 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BillBasisId",
                table: "Service",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "BillPerTransactionCount",
                table: "Service",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "MobileOriginatingCount",
                table: "Service",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "MobileTerminatingCount",
                table: "Service",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillBasisId",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "BillPerTransactionCount",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "MobileOriginatingCount",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "MobileTerminatingCount",
                table: "Service");
        }
    }
}
