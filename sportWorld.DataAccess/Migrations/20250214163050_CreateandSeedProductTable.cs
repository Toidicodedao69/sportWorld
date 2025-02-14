using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace sportWorld.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class CreateandSeedProductTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Categories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ListPrice = table.Column<double>(type: "float", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Price20 = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Brand", "Description", "ListPrice", "Name", "Price", "Price20" },
                values: new object[,]
                {
                    { 1, "Yonex", "The Yonex Nanoflare 700 Pro is designed to make your clears—those deep, high shots that push your opponent to the backcourt—easier and more consistent. Its unique frame design helps you hit these shots with less effort, giving you better control of the game.", 300.0, "Yonex Nanoflare 700 PRO", 289.0, 260.0 },
                    { 2, "Victor", "The New Victor Thruster F Ultra (2024) is made to be user friendly, yet being able to product powerful and pin point accuracy smashes. \r\n\r\nVictor's signature Free Core Handle is used to maximise the racquet's shock absorption. This provides you with a comfortable hitting experience.", 290.0, "Victor Thruster F Ultra", 279.0, 250.0 },
                    { 3, "Yonex", "The new update for the Yonex Comfort Z Performance badminton shoes features a couple of upgrades to make the shoes more comfortable, with increased performance.", 250.0, "Yonex Power Cushion Comfort Z 3 (Black/Mint)", 239.0, 210.0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);
        }
    }
}
