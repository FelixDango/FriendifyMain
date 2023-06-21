using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FriendifyMain.Migrations
{
    /// <inheritdoc />
    public partial class MessagesModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReceiverUsername",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiverUsername",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "Messages");
        }
    }
}
