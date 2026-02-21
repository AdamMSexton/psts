using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace psts.web.Migrations
{
    /// <inheritdoc />
    public partial class FixTypoOnTimeTransactionRegister : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EnterdTimeStamp",
                table: "PstsTimeTransactionss",
                newName: "EnteredTimeStamp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EnteredTimeStamp",
                table: "PstsTimeTransactionss",
                newName: "EnterdTimeStamp");
        }
    }
}
