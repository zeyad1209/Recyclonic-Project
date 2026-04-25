using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecyclonicApi.Migrations
{
    /// <inheritdoc />
    public partial class updates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EwasteItems_EwasteItemBrands_BrandId",
                table: "EwasteItems");

            migrationBuilder.DropForeignKey(
                name: "FK_EwasteItems_EwasteItemTypes_TypeId",
                table: "EwasteItems");

            migrationBuilder.DropTable(
                name: "EwasteItemBrands");

            migrationBuilder.DropTable(
                name: "EwasteItemTypes");

            migrationBuilder.DropIndex(
                name: "IX_EwasteItems_BrandId",
                table: "EwasteItems");

            migrationBuilder.DropIndex(
                name: "IX_EwasteItems_TypeId",
                table: "EwasteItems");

            migrationBuilder.DropColumn(
                name: "BrandId",
                table: "EwasteItems");

            migrationBuilder.DropColumn(
                name: "Model",
                table: "EwasteItems");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "EwasteItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BrandId",
                table: "EwasteItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Model",
                table: "EwasteItems",
                type: "nvarchar(45)",
                maxLength: 45,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "TypeId",
                table: "EwasteItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "EwasteItemBrands",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EwasteItemBrands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EwasteItemTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EwasteItemTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EwasteItems_BrandId",
                table: "EwasteItems",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_EwasteItems_TypeId",
                table: "EwasteItems",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_EwasteItems_EwasteItemBrands_BrandId",
                table: "EwasteItems",
                column: "BrandId",
                principalTable: "EwasteItemBrands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EwasteItems_EwasteItemTypes_TypeId",
                table: "EwasteItems",
                column: "TypeId",
                principalTable: "EwasteItemTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
