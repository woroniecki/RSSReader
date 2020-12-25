using Microsoft.EntityFrameworkCore.Migrations;

namespace RSSReader.Migrations
{
    public partial class changedrowfromAdresstoUrlinBlogtable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Adress",
                table: "Blogs",
                newName: "Url");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Url",
                table: "Blogs",
                newName: "Adress");
        }
    }
}
