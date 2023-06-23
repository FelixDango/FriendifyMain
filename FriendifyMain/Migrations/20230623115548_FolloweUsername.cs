using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FriendifyMain.Migrations
{
    /// <inheritdoc />
    public partial class FolloweUsername : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Followers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Username",
                table: "Followers");
        }
    }
}
