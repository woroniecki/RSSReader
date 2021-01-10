using Microsoft.EntityFrameworkCore.Migrations;

namespace RSSReader.Migrations
{
    public partial class removed_authtoken_from_refreshTokenTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthToken",
                table: "RefreshToken");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthToken",
                table: "RefreshToken",
                type: "TEXT",
                nullable: true);
        }
    }
}
