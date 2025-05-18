using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Video_Library_Api.Migrations
{
    public partial class AddInitial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Videos",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 16, nullable: false),
                    FileName = table.Column<string>(maxLength: 255, nullable: false),
                    FormatLongName = table.Column<string>(maxLength: 60, nullable: false),
                    Duration = table.Column<double>(nullable: true),
                    Size = table.Column<long>(nullable: true),
                    BitRate = table.Column<long>(nullable: true),
                    Width = table.Column<int>(nullable: true),
                    Height = table.Column<int>(nullable: true),
                    StreamsJSON = table.Column<string>(maxLength: 4096, nullable: false),
                    Thumbnail = table.Column<string>(maxLength: 4096, nullable: false),
                    GifPreview = table.Column<string>(maxLength: 4096, nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: true),
                    UploadTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Videos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RelatedVideos",
                columns: table => new
                {
                    Video1Id = table.Column<string>(nullable: false),
                    Video2Id = table.Column<string>(nullable: false),
                    Offset1 = table.Column<int>(nullable: false),
                    Offset2 = table.Column<int>(nullable: false),
                    Length = table.Column<int>(nullable: false),
                    Score = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelatedVideos", x => new { x.Video1Id, x.Video2Id });
                    table.ForeignKey(
                        name: "FK_RelatedVideos_Videos_Video1Id",
                        column: x => x.Video1Id,
                        principalTable: "Videos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RelatedVideos_Videos_Video2Id",
                        column: x => x.Video2Id,
                        principalTable: "Videos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VideosTags",
                columns: table => new
                {
                    VideoId = table.Column<string>(nullable: false),
                    TagId = table.Column<int>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideosTags", x => new { x.VideoId, x.TagId});
                    table.ForeignKey(
                        name: "FK_VideosTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VideosTags_Videos_VideoId",
                        column: x => x.VideoId,
                        principalTable: "Videos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RelatedVideos_Video2Id",
                table: "RelatedVideos",
                column: "Video2Id");

            migrationBuilder.CreateIndex(
                name: "IX_VideosTags_TagId",
                table: "VideosTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Name",
                table: "Tags",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RelatedVideos");

            migrationBuilder.DropTable(
                name: "VideosTags");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Videos");

            migrationBuilder.DropIndex(
                name: "IX_Tags_Name",
                table: "Tags");
        }
    }
}
