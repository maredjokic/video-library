using Microsoft.EntityFrameworkCore.Migrations;

namespace Video_Library_Api.Migrations
{
    public partial class AddProcessesLeftFieldToVideo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProcessesLeft",
                table: "Videos",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProcessesLeft",
                table: "Videos");
        }
    }
}
