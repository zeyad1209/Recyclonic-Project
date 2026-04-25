using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecyclonicApi.Migrations
{
    /// <inheritdoc />
    public partial class fix_something_inmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecycleRequests_AspNetUsers_UserId",
                table: "RecycleRequests");

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                table: "RecycleRequests",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecycleRequests_EmployeeId",
                table: "RecycleRequests",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_RecycleRequests_AspNetUsers_EmployeeId",
                table: "RecycleRequests",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RecycleRequests_AspNetUsers_UserId",
                table: "RecycleRequests",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecycleRequests_AspNetUsers_EmployeeId",
                table: "RecycleRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_RecycleRequests_AspNetUsers_UserId",
                table: "RecycleRequests");

            migrationBuilder.DropIndex(
                name: "IX_RecycleRequests_EmployeeId",
                table: "RecycleRequests");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "RecycleRequests");

            migrationBuilder.AddForeignKey(
                name: "FK_RecycleRequests_AspNetUsers_UserId",
                table: "RecycleRequests",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
