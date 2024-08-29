using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Food_Delivery.Migrations
{
    /// <inheritdoc />
    public partial class mainModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "category",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__category", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "orderStatus",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orderStatus", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "role",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__role", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "dishes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    categoryId = table.Column<int>(type: "int", nullable: false),
                    calories = table.Column<int>(type: "int", nullable: true),
                    squirrels = table.Column<int>(type: "int", nullable: true),
                    fats = table.Column<int>(type: "int", nullable: true),
                    carbohydrates = table.Column<int>(type: "int", nullable: true),
                    weight = table.Column<int>(type: "int", nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: true),
                    price = table.Column<int>(type: "int", nullable: false),
                    image = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    stopList = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dishes", x => x.id);
                    table.ForeignKey(
                        name: "fk_categoryId_dishes_category",
                        column: x => x.categoryId,
                        principalTable: "category",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "account",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    roleId = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    surname = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    patronymic = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    registrationDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    numberPhone = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    login = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    city = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    street = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    house = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    apartament = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__account", x => x.id);
                    table.ForeignKey(
                        name: "fk_roleId_account_role",
                        column: x => x.roleId,
                        principalTable: "role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "order",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    dateTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    startDesiredDeliveryTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    endDesiredDeliveryTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    accountId = table.Column<int>(type: "int", nullable: true),
                    orderStatusId = table.Column<int>(type: "int", nullable: true),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    surname = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    patronymic = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    city = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    street = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    house = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    apartament = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    numberPhone = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    costPrice = table.Column<int>(type: "int", nullable: false),
                    typePayment = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    prepareChangeMoney = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order", x => x.id);
                    table.ForeignKey(
                        name: "fk_accountId_order_account",
                        column: x => x.accountId,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_orderStatusId_order_orderStatus",
                        column: x => x.orderStatusId,
                        principalTable: "orderStatus",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "shoppingCart",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    accountId = table.Column<int>(type: "int", nullable: true),
                    costPrice = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shoppingCart", x => x.id);
                    table.ForeignKey(
                        name: "fk_accountId_shoppingCart_account",
                        column: x => x.accountId,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "compositionOrder",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    orderId = table.Column<int>(type: "int", nullable: false),
                    dishesId = table.Column<int>(type: "int", nullable: true),
                    nameDishes = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    descriptionDishes = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    calories = table.Column<int>(type: "int", nullable: true),
                    squirrels = table.Column<int>(type: "int", nullable: true),
                    fats = table.Column<int>(type: "int", nullable: true),
                    carbohydrates = table.Column<int>(type: "int", nullable: true),
                    weight = table.Column<int>(type: "int", nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: true),
                    price = table.Column<int>(type: "int", nullable: false),
                    image = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_compositionOrder", x => x.id);
                    table.ForeignKey(
                        name: "fk_dishesId_compositionOrder_dishes",
                        column: x => x.dishesId,
                        principalTable: "dishes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_orderId_compositionOrder_oder",
                        column: x => x.orderId,
                        principalTable: "order",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "compositionCart",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    shoppingCartId = table.Column<int>(type: "int", nullable: false),
                    dishesId = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_compositionCart", x => x.id);
                    table.ForeignKey(
                        name: "fk_dishesId_compositionCart_dishes",
                        column: x => x.dishesId,
                        principalTable: "dishes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_shoppingCartId_compositionCart_shoppingCart",
                        column: x => x.shoppingCartId,
                        principalTable: "shoppingCart",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_account_roleId",
                table: "account",
                column: "roleId");

            migrationBuilder.CreateIndex(
                name: "IX_compositionCart_dishesId",
                table: "compositionCart",
                column: "dishesId");

            migrationBuilder.CreateIndex(
                name: "IX_compositionCart_shoppingCartId",
                table: "compositionCart",
                column: "shoppingCartId");

            migrationBuilder.CreateIndex(
                name: "IX_compositionOrder_dishesId",
                table: "compositionOrder",
                column: "dishesId");

            migrationBuilder.CreateIndex(
                name: "IX_compositionOrder_orderId",
                table: "compositionOrder",
                column: "orderId");

            migrationBuilder.CreateIndex(
                name: "IX_dishes_categoryId",
                table: "dishes",
                column: "categoryId");

            migrationBuilder.CreateIndex(
                name: "IX_order_accountId",
                table: "order",
                column: "accountId");

            migrationBuilder.CreateIndex(
                name: "IX_order_orderStatusId",
                table: "order",
                column: "orderStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_shoppingCart_accountId",
                table: "shoppingCart",
                column: "accountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "compositionCart");

            migrationBuilder.DropTable(
                name: "compositionOrder");

            migrationBuilder.DropTable(
                name: "shoppingCart");

            migrationBuilder.DropTable(
                name: "dishes");

            migrationBuilder.DropTable(
                name: "order");

            migrationBuilder.DropTable(
                name: "category");

            migrationBuilder.DropTable(
                name: "account");

            migrationBuilder.DropTable(
                name: "orderStatus");

            migrationBuilder.DropTable(
                name: "role");
        }
    }
}
