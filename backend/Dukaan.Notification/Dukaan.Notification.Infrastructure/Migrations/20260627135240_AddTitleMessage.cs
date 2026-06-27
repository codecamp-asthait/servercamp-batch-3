using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dukaan.Notification.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTitleMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Message",
                schema: "notification",
                table: "Notifications",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                schema: "notification",
                table: "Notifications",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Message",
                schema: "notification",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Title",
                schema: "notification",
                table: "Notifications");
        }
    }
}
