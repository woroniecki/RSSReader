using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataLayer.Migrations
{
    public partial class add_userpostdata_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserPostDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstDateOpen = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastDateOpen = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Readed = table.Column<bool>(type: "INTEGER", nullable: false),
                    Favourite = table.Column<bool>(type: "INTEGER", nullable: false),
                    PostId = table.Column<int>(type: "INTEGER", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPostDatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPostDatas_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserPostDatas_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserPostDatas_PostId",
                table: "UserPostDatas",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPostDatas_UserId",
                table: "UserPostDatas",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPostDatas");
        }
    }
}
