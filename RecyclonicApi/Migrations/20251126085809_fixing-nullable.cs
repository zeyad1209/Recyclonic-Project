using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecyclonicApi.Migrations
{
    /// <inheritdoc />
    public partial class fixingnullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deliveries_AspNetUsers_DeliveryUserId",
                table: "Deliveries");

            migrationBuilder.AlterColumn<decimal>(
                name: "weight",
                table: "EwasteItems",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<Guid>(
                name: "DeliveryUserId",
                table: "Deliveries",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Deliveries_AspNetUsers_DeliveryUserId",
                table: "Deliveries",
                column: "DeliveryUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deliveries_AspNetUsers_DeliveryUserId",
                table: "Deliveries");

            migrationBuilder.AlterColumn<decimal>(
                name: "weight",
                table: "EwasteItems",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)",
                oldPrecision: 18,
                oldScale: 3);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeliveryUserId",
                table: "Deliveries",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Deliveries_AspNetUsers_DeliveryUserId",
                table: "Deliveries",
                column: "DeliveryUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
