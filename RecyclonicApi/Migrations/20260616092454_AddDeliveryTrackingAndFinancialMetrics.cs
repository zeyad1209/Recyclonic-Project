using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecyclonicApi.Migrations
{
    /// <inheritdoc />
    public partial class AddDeliveryTrackingAndFinancialMetrics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AmountPaidToUser",
                table: "RecycleRequests",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountReceivedFromRecycler",
                table: "RecycleRequests",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelivered",
                table: "RecycleRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountPaidToUser",
                table: "RecycleRequests");

            migrationBuilder.DropColumn(
                name: "AmountReceivedFromRecycler",
                table: "RecycleRequests");

            migrationBuilder.DropColumn(
                name: "IsDelivered",
                table: "RecycleRequests");
        }
    }
}
