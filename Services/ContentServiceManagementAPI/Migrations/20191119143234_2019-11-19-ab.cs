using Microsoft.EntityFrameworkCore.Migrations;

namespace ContentServiceManagementAPI.Migrations
{
    public partial class _20191119ab : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsBulkSms",
                table: "Service",
                newName: "IsVasSystemService");

            migrationBuilder.AddColumn<string>(
                name: "VasSystemServiceCode",
                table: "Service",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VasSystemServiceCode",
                table: "Service");

            migrationBuilder.RenameColumn(
                name: "IsVasSystemService",
                table: "Service",
                newName: "IsBulkSms");
        }
    }
}
