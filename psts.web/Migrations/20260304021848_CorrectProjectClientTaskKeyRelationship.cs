using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace psts.web.Migrations
{
    /// <inheritdoc />
    public partial class CorrectProjectClientTaskKeyRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PstsTaskDefinitions_PstsProjectDefinitions_ProjectId",
                table: "PstsTaskDefinitions");

            migrationBuilder.DropIndex(
                name: "IX_PstsTaskDefinitions_ProjectId",
                table: "PstsTaskDefinitions");

            migrationBuilder.DropIndex(
                name: "IX_PstsProjectDefinitions_ClientId",
                table: "PstsProjectDefinitions");

            migrationBuilder.CreateIndex(
                name: "IX_PstsProjectDefinitions_ClientId",
                table: "PstsProjectDefinitions",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_PstsTaskDefinitions_PstsProjectDefinitions_TaskId",
                table: "PstsTaskDefinitions",
                column: "TaskId",
                principalTable: "PstsProjectDefinitions",
                principalColumn: "ProjectId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PstsTaskDefinitions_PstsProjectDefinitions_TaskId",
                table: "PstsTaskDefinitions");

            migrationBuilder.DropIndex(
                name: "IX_PstsProjectDefinitions_ClientId",
                table: "PstsProjectDefinitions");

            migrationBuilder.CreateIndex(
                name: "IX_PstsTaskDefinitions_ProjectId",
                table: "PstsTaskDefinitions",
                column: "ProjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PstsProjectDefinitions_ClientId",
                table: "PstsProjectDefinitions",
                column: "ClientId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PstsTaskDefinitions_PstsProjectDefinitions_ProjectId",
                table: "PstsTaskDefinitions",
                column: "ProjectId",
                principalTable: "PstsProjectDefinitions",
                principalColumn: "ProjectId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
