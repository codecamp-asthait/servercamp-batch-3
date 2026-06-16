using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dukaan.Media.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddImagePathToMediaMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                schema: "media",
                table: "MediaMetadata",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                schema: "media",
                table: "MediaMetadata");
        }
    }
}
