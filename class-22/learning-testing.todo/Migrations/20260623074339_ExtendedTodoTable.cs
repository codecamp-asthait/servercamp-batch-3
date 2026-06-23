using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace learning_testing.Migrations
{
    /// <inheritdoc />
    public partial class ExtendedTodoTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ArchivedAt",
                table: "Todos",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Todos",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArchivedAt",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Todos");
        }
    }
}
