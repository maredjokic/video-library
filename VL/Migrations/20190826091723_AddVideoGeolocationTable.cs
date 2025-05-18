using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

namespace Video_Library_Api.Migrations
{
    public partial class AddVideoGeolocationTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", "'postgis', '', ''");

            migrationBuilder.CreateTable(
                name: "VideoGeolocation",
                columns: table => new
                {
                    VideoId = table.Column<string>(maxLength: 16, nullable: false),
                    CameraLine = table.Column<LineString>(type: "geography", nullable: true),
                    FilmedArea = table.Column<Polygon>(type: "geography", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoGeolocation", x => x.VideoId);
                    table.ForeignKey(
                        name: "FK_VideoGeolocation_Videos_VideoId",
                        column: x => x.VideoId,
                        principalTable: "Videos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VideoGeolocation");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:postgis", "'postgis', '', ''");
        }
    }
}
