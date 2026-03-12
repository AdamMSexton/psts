using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace psts.web.Migrations
{
    /// <inheritdoc />
    public partial class AddSettingDescriptor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Descriptor",
                table: "AppSettingss",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Descriptor",
                table: "AppSettingss");
        }
    }
}
