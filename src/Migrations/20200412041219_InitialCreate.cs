using Microsoft.EntityFrameworkCore.Migrations;

namespace OneeChan.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HouseKeeper",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AutoCategoryChannelId = table.Column<long>(nullable: true),
                    AutoVoiceChannelId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HouseKeeper", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServerSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Prefix = table.Column<char>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Guilds",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GuildId = table.Column<long>(nullable: false),
                    HouseKeeperSettingsId = table.Column<int>(nullable: true),
                    ServerSettingsId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guilds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Guilds_HouseKeeper_HouseKeeperSettingsId",
                        column: x => x.HouseKeeperSettingsId,
                        principalTable: "HouseKeeper",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Guilds_ServerSettings_ServerSettingsId",
                        column: x => x.ServerSettingsId,
                        principalTable: "ServerSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Guilds_HouseKeeperSettingsId",
                table: "Guilds",
                column: "HouseKeeperSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_Guilds_ServerSettingsId",
                table: "Guilds",
                column: "ServerSettingsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Guilds");

            migrationBuilder.DropTable(
                name: "HouseKeeper");

            migrationBuilder.DropTable(
                name: "ServerSettings");
        }
    }
}
