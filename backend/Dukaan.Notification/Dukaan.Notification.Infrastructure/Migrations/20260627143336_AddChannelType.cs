using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dukaan.Notification.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddChannelType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChannelType",
                schema: "notification",
                table: "Notifications",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChannelType",
                schema: "notification",
                table: "Notifications");
        }
    }
}
