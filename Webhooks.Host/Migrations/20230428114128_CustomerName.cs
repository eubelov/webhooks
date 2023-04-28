using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Webhooks.Host.Migrations
{
    /// <inheritdoc />
    public partial class CustomerName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                schema: "webhooks",
                table: "Subscriptions",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerName",
                schema: "webhooks",
                table: "Subscriptions");
        }
    }
}
