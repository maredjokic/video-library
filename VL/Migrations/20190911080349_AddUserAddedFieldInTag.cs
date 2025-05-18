using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Video_Library_Api.Migrations
{
    public partial class AddUserAddedFieldInTag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UploadTime",
                table: "Videos",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "UserAdded",
                table: "Tags",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserAdded",
                table: "Tags");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UploadTime",
                table: "Videos",
                nullable: true,
                oldClrType: typeof(DateTime));
        }
    }
}
