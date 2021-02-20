using Microsoft.EntityFrameworkCore.Migrations;

namespace ContentServiceManagementAPI.Migrations
{
    public partial class ab20190815 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Approved",
                table: "ServiceProvider",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Approved",
                table: "ServiceProvider");
        }
    }
}
