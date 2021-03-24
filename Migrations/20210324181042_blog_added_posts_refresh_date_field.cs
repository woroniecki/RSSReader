using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RSSReader.Migrations
{
    public partial class blog_added_posts_refresh_date_field : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastPostsRefreshDate",
                table: "Blogs",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastPostsRefreshDate",
                table: "Blogs");
        }
    }
}
