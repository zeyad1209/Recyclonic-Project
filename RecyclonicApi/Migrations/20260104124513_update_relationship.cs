using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecyclonicApi.Migrations
{
    /// <inheritdoc />
    public partial class update_relationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MarketplaceItems_AspNetUsers_SellerId",
                table: "MarketplaceItems");

            migrationBuilder.CreateTable(
                name: "ApplicationUserMarketplaceItem",
                columns: table => new
                {
                    FavoritedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FavouriteItemsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserMarketplaceItem", x => new { x.FavoritedById, x.FavouriteItemsId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserMarketplaceItem_AspNetUsers_FavoritedById",
                        column: x => x.FavoritedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserMarketplaceItem_MarketplaceItems_FavouriteItemsId",
                        column: x => x.FavouriteItemsId,
                        principalTable: "MarketplaceItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserMarketplaceItem_FavouriteItemsId",
                table: "ApplicationUserMarketplaceItem",
                column: "FavouriteItemsId");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketplaceItems_AspNetUsers_SellerId",
                table: "MarketplaceItems",
                column: "SellerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MarketplaceItems_AspNetUsers_SellerId",
                table: "MarketplaceItems");

            migrationBuilder.DropTable(
                name: "ApplicationUserMarketplaceItem");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketplaceItems_AspNetUsers_SellerId",
                table: "MarketplaceItems",
                column: "SellerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
