using Microsoft.EntityFrameworkCore.Migrations;

namespace Video_Library_Api.Migrations
{
    public partial class RemoveUrlFieldsFromVideo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GifPreview",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "Thumbnail",
                table: "Videos");


            migrationBuilder.AddColumn<int>(
                name: "From",
                table: "VideosTags",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "To",
                table: "VideosTags",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.DropPrimaryKey(
                name: "PK_VideosTags",
                table: "VideosTags");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VideosTags",
                table: "VideosTags",
                columns: new[] { "VideoId", "TagId", "From", "To" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GifPreview",
                table: "Videos",
                maxLength: 4096,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Thumbnail",
                table: "Videos",
                maxLength: 4096,
                nullable: false,
                defaultValue: "");

            migrationBuilder.DropColumn(
                name: "From",
                table: "VideosTags");

            migrationBuilder.DropColumn(
                name: "To",
                table: "VideosTags");

            migrationBuilder.DropColumn(
                name: "FinishedProcessing",
                table: "Videos");
        }
    }
}
