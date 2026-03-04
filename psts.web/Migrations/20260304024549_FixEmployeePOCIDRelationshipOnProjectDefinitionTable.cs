using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace psts.web.Migrations
{
    /// <inheritdoc />
    public partial class FixEmployeePOCIDRelationshipOnProjectDefinitionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PstsProjectDefinitions_EmployeePOCId",
                table: "PstsProjectDefinitions");

            migrationBuilder.CreateIndex(
                name: "IX_PstsProjectDefinitions_EmployeePOCId",
                table: "PstsProjectDefinitions",
                column: "EmployeePOCId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PstsProjectDefinitions_EmployeePOCId",
                table: "PstsProjectDefinitions");

            migrationBuilder.CreateIndex(
                name: "IX_PstsProjectDefinitions_EmployeePOCId",
                table: "PstsProjectDefinitions",
                column: "EmployeePOCId",
                unique: true);
        }
    }
}
