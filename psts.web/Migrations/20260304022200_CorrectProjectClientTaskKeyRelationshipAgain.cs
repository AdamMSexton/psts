using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace psts.web.Migrations
{
    /// <inheritdoc />
    public partial class CorrectProjectClientTaskKeyRelationshipAgain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PstsTaskDefinitions_PstsProjectDefinitions_TaskId",
                table: "PstsTaskDefinitions");

            migrationBuilder.CreateIndex(
                name: "IX_PstsTaskDefinitions_ProjectId",
                table: "PstsTaskDefinitions",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_PstsTaskDefinitions_PstsProjectDefinitions_ProjectId",
                table: "PstsTaskDefinitions",
                column: "ProjectId",
                principalTable: "PstsProjectDefinitions",
                principalColumn: "ProjectId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PstsTaskDefinitions_PstsProjectDefinitions_ProjectId",
                table: "PstsTaskDefinitions");

            migrationBuilder.DropIndex(
                name: "IX_PstsTaskDefinitions_ProjectId",
                table: "PstsTaskDefinitions");

            migrationBuilder.AddForeignKey(
                name: "FK_PstsTaskDefinitions_PstsProjectDefinitions_TaskId",
                table: "PstsTaskDefinitions",
                column: "TaskId",
                principalTable: "PstsProjectDefinitions",
                principalColumn: "ProjectId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
