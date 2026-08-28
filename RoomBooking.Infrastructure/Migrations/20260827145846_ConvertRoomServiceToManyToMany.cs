#nullable disable

namespace RoomBooking.Infrastructure.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <inheritdoc />
    public partial class ConvertRoomServiceToManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomServices_Rooms_RoomId",
                table: "RoomServices");

            migrationBuilder.DropIndex(
                name: "IX_RoomServices_RoomId",
                table: "RoomServices");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "RoomServices");

            migrationBuilder.CreateTable(
                name: "RoomRoomService",
                columns: table => new
                {
                    RoomsId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ServicesId = table.Column<Guid>(type: "TEXT", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomRoomService", x => new { x.RoomsId, x.ServicesId });
                    table.ForeignKey(
                        name: "FK_RoomRoomService_RoomServices_ServicesId",
                        column: x => x.ServicesId,
                        principalTable: "RoomServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoomRoomService_Rooms_RoomsId",
                        column: x => x.RoomsId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoomRoomService_ServicesId",
                table: "RoomRoomService",
                column: "ServicesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoomRoomService");

            migrationBuilder.AddColumn<Guid>(
                name: "RoomId",
                table: "RoomServices",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_RoomServices_RoomId",
                table: "RoomServices",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomServices_Rooms_RoomId",
                table: "RoomServices",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
