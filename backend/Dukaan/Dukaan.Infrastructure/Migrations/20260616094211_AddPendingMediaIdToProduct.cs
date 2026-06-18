using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dukaan.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPendingMediaIdToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PendingMediaId",
                table: "Products",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PendingMediaId",
                table: "Products");
        }
    }
}
