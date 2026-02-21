using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace psts.web.Migrations
{
    /// <inheritdoc />
    public partial class RestructureSettingsToKeyValuePair : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AppSettingss",
                table: "AppSettingss");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "AppSettingss");

            migrationBuilder.DropColumn(
                name: "DisableAccountAfterXDaysStale",
                table: "AppSettingss");

            migrationBuilder.DropColumn(
                name: "MakeOIDCAvailable",
                table: "AppSettingss");

            migrationBuilder.DropColumn(
                name: "ManagerApprovalForAdjustments",
                table: "AppSettingss");

            migrationBuilder.DropColumn(
                name: "OIDCEnabledByDefault",
                table: "AppSettingss");

            migrationBuilder.AddColumn<string>(
                name: "Setting",
                table: "AppSettingss",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "AppSettingss",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppSettingss",
                table: "AppSettingss",
                column: "Setting");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AppSettingss",
                table: "AppSettingss");

            migrationBuilder.DropColumn(
                name: "Setting",
                table: "AppSettingss");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "AppSettingss");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "AppSettingss",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "DisableAccountAfterXDaysStale",
                table: "AppSettingss",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "MakeOIDCAvailable",
                table: "AppSettingss",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ManagerApprovalForAdjustments",
                table: "AppSettingss",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OIDCEnabledByDefault",
                table: "AppSettingss",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppSettingss",
                table: "AppSettingss",
                column: "Id");
        }
    }
}
