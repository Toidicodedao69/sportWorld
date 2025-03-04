using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sportWorld.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddSessionIdtoOrderHeader : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentId",
                table: "OrderHeaders",
                newName: "PaymentIntentId");

            migrationBuilder.AddColumn<string>(
                name: "SessionId",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "OrderHeaders");

            migrationBuilder.RenameColumn(
                name: "PaymentIntentId",
                table: "OrderHeaders",
                newName: "PaymentId");
        }
    }
}
