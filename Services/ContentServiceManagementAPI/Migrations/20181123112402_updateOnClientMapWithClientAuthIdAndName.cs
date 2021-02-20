using Microsoft.EntityFrameworkCore.Migrations;

namespace ContentServiceManagementAPI.Migrations
{
    public partial class updateOnClientMapWithClientAuthIdAndName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ClientAuthId",
                table: "MapServiceToClient",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "ClientName",
                table: "MapServiceToClient",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "ClientAuthId",
            //    table: "MapServiceToClient");

            //migrationBuilder.DropColumn(
            //    name: "ClientName",
            //    table: "MapServiceToClient");
        }
    }
}
