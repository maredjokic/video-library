using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Video_Library_Api.Migrations
{
    public partial class AddCodecNameToVideo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<int>(
                name: "Tags_Id_sequence",
                startValue: 81L);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Tags",
                nullable: false,
                defaultValueSql: "nextval('\"Tags_Id_sequence\"')",
                oldClrType: typeof(int))
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);
            
            migrationBuilder.AddColumn<string>(
                name: "CodecName",
                table: "Videos",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(
                name: "Tags_Id_sequence");

            migrationBuilder.DropColumn(
                name: "CodecName",
                table: "Videos");
            
            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Tags",
                nullable: false,
                oldClrType: typeof(int),
                oldDefaultValueSql: "nextval('\"Tags_Id_sequence\"')")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);
        }
    }
}
