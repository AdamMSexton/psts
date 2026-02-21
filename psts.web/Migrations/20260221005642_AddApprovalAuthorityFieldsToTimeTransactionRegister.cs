using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace psts.web.Migrations
{
    /// <inheritdoc />
    public partial class AddApprovalAuthorityFieldsToTimeTransactionRegister : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApprovalAuthority",
                table: "PstsTimeTransactionss",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovingUserId",
                table: "PstsTimeTransactionss",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PstsTimeTransactionss_ApprovingUserId",
                table: "PstsTimeTransactionss",
                column: "ApprovingUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PstsTimeTransactionss_AspNetUsers_ApprovingUserId",
                table: "PstsTimeTransactionss",
                column: "ApprovingUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PstsTimeTransactionss_AspNetUsers_ApprovingUserId",
                table: "PstsTimeTransactionss");

            migrationBuilder.DropIndex(
                name: "IX_PstsTimeTransactionss_ApprovingUserId",
                table: "PstsTimeTransactionss");

            migrationBuilder.DropColumn(
                name: "ApprovalAuthority",
                table: "PstsTimeTransactionss");

            migrationBuilder.DropColumn(
                name: "ApprovingUserId",
                table: "PstsTimeTransactionss");
        }
    }
}
