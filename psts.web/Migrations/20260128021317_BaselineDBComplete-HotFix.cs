using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace psts.web.Migrations
{
    /// <inheritdoc />
    public partial class BaselineDBCompleteHotFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "LoginPassAllowed",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OIDCAllowed",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoginPassAllowed",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "OIDCAllowed",
                table: "AspNetUsers");
        }
    }
}
