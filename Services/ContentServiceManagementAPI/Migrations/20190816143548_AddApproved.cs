using Microsoft.EntityFrameworkCore.Migrations;

namespace ContentServiceManagementAPI.Migrations
{
    public partial class AddApproved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Approved",
                table: "ServiceProvider",
                nullable: true,
                oldClrType: typeof(bool),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Approved",
                table: "ServiceProvider",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
