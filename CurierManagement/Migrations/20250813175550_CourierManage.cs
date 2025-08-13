using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CurierManagement.Migrations
{
    /// <inheritdoc />
    public partial class CourierManage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Orders");

            migrationBuilder.AddColumn<string>(
                name: "WebAddressInMap",
                table: "Orders",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WebAddressInMap",
                table: "Orders");

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Orders",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Orders",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
