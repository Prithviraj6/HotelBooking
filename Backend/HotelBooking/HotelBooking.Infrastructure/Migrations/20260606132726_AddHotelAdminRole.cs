using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHotelAdminRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ManagedHotelId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_ManagedHotelId",
                table: "Users",
                column: "ManagedHotelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Hotels_ManagedHotelId",
                table: "Users",
                column: "ManagedHotelId",
                principalTable: "Hotels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Hotels_ManagedHotelId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_ManagedHotelId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ManagedHotelId",
                table: "Users");
        }
    }
}
