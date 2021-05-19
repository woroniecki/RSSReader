using Microsoft.EntityFrameworkCore.Migrations;

namespace DataLayer.Migrations
{
    public partial class subscription_added_filter_readed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPostDatas_Posts_PostId",
                table: "UserPostDatas");

            migrationBuilder.AlterColumn<int>(
                name: "PostId",
                table: "UserPostDatas",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "FilterReaded",
                table: "Subscriptions",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPostDatas_Posts_PostId",
                table: "UserPostDatas",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPostDatas_Posts_PostId",
                table: "UserPostDatas");

            migrationBuilder.DropColumn(
                name: "FilterReaded",
                table: "Subscriptions");

            migrationBuilder.AlterColumn<int>(
                name: "PostId",
                table: "UserPostDatas",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPostDatas_Posts_PostId",
                table: "UserPostDatas",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
