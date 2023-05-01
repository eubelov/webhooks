using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Webhooks.Host.Migrations
{
    /// <inheritdoc />
    public partial class StatusCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StatusCode",
                schema: "webhooks",
                table: "Invocations",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusCode",
                schema: "webhooks",
                table: "Invocations");
        }
    }
}
