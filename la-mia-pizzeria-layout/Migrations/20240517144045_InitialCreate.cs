using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Test_MVC_2.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_pizzas",
                table: "pizzas");

            migrationBuilder.RenameTable(
                name: "pizzas",
                newName: "Pizze");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pizze",
                table: "Pizze",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Pizze",
                table: "Pizze");

            migrationBuilder.RenameTable(
                name: "Pizze",
                newName: "pizzas");

            migrationBuilder.AddPrimaryKey(
                name: "PK_pizzas",
                table: "pizzas",
                column: "Id");
        }
    }
}
