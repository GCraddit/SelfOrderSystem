using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Giles_Chen_test_1.Migrations
{
    /// <inheritdoc />
    public partial class AddOrdersAndOrderItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Foodandbevs",
                columns: table => new
                {
                    FoodandbevID = table.Column<int>(type: "int", nullable: false),
                    FoodandbevName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FoodandbevPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FoodandbevDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FoodandbevImagePath = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Foodandbevs", x => x.FoodandbevID);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderID);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    OrderItemID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FoodandbevID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.OrderItemID);
                    table.ForeignKey(
                        name: "FK_OrderItems_Foodandbevs_FoodandbevID",
                        column: x => x.FoodandbevID,
                        principalTable: "Foodandbevs",
                        principalColumn: "FoodandbevID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderID",
                        column: x => x.OrderID,
                        principalTable: "Orders",
                        principalColumn: "OrderID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_FoodandbevID",
                table: "OrderItems",
                column: "FoodandbevID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderID",
                table: "OrderItems",
                column: "OrderID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Foodandbevs");

            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}
