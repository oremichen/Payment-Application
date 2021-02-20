using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ContentServiceManagementAPI.Migrations
{
    public partial class addedProperyForMap : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
              name: "MapContentToServiceProvider");

            migrationBuilder.DropTable(
                name: "MapServiceToClient");

            migrationBuilder.CreateTable(
                name: "MapContentToServiceProvider",
                columns: table => new
                {
                    MapContentToServiceProviderId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ContentProviderId = table.Column<int>(nullable: false),
                    ContentProviderName = table.Column<string>(nullable: true),
                    ContentId = table.Column<long>(nullable: false),
                    ServiceProviderId = table.Column<long>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    MappedBy = table.Column<string>(nullable: true),
                    RateId = table.Column<long>(nullable: true),
                    RateName = table.Column<string>(nullable: true),
                    DedicatedRateId = table.Column<long>(nullable: true),
                    RateType = table.Column<string>(nullable: true),
                    DedicatedRateName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapContentToServiceProvider", x => x.MapContentToServiceProviderId);
                });

            migrationBuilder.CreateTable(
                name: "MapServiceToClient",
                columns: table => new
                {
                    MapServiceToClientId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClientId = table.Column<int>(nullable: false),
                    ServiceId = table.Column<long>(nullable: false),
                    ServiceProviderId = table.Column<long>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    MappedBy = table.Column<string>(nullable: true),
                    ServiceProviderName = table.Column<string>(nullable: true),
                    RateId = table.Column<long>(nullable: false),
                    RateName = table.Column<string>(nullable: true),
                    DedicatedRateId = table.Column<long>(nullable: true),
                    DedicatedRateName = table.Column<string>(nullable: true),
                    RateType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapServiceToClient", x => x.MapServiceToClientId);
                });

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
                            
        }
    }
}
