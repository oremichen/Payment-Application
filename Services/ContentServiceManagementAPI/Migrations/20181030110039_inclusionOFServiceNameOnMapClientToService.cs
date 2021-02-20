using Microsoft.EntityFrameworkCore.Migrations;

namespace ContentServiceManagementAPI.Migrations
{
    public partial class inclusionOFServiceNameOnMapClientToService : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ServiceName",
                table: "MapServiceToClient",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceName",
                table: "MapServiceToClient");
        }
    }
}
