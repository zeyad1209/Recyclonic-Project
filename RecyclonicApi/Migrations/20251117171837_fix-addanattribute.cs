using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecyclonicApi.Migrations
{
    /// <inheritdoc />
    public partial class fixaddanattribute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "RecycleRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "RecycleRequests");
        }
    }
}
