using Microsoft.EntityFrameworkCore.Migrations;

namespace Video_Library_Api.Migrations
{
    public partial class AddDirectoryInformationAndEntries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DirectoryInfos",
                columns: table => new
                {
                    DirectoryHash = table.Column<string>(nullable: false),
                    Path = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectoryInfos", x => x.DirectoryHash);
                });

            migrationBuilder.CreateTable(
                name: "DirectoryEntries",
                columns: table => new
                {
                    FilePath = table.Column<string>(nullable: false),
                    DirectoryHash = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    VideoId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectoryEntries", x => x.FilePath);
                    table.ForeignKey(
                        name: "FK_DirectoryEntries_DirectoryInfos_DirectoryHash",
                        column: x => x.DirectoryHash,
                        principalTable: "DirectoryInfos",
                        principalColumn: "DirectoryHash",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DirectoryEntries_DirectoryHash",
                table: "DirectoryEntries",
                column: "DirectoryHash");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DirectoryEntries");

            migrationBuilder.DropTable(
                name: "DirectoryInfos");
        }
    }
}
