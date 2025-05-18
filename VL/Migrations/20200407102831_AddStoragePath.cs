using Microsoft.EntityFrameworkCore.Migrations;

namespace Video_Library_Api.Migrations
{
    public partial class AddStoragePath : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StoragePath",
                table: "Videos",
                nullable: true,
                defaultValue: null);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StoragePath",
                table: "Videos"
                );
        }
    }
}
