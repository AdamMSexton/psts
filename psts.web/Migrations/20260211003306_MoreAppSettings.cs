using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace psts.web.Migrations
{
    /// <inheritdoc />
    public partial class MoreAppSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastSuccessfulLogin",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "StaleAccountLockoutEnabled",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastSuccessfulLogin",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "StaleAccountLockoutEnabled",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DisableAccountAfterXDaysStale",
                table: "AppSettingss");

            migrationBuilder.DropColumn(
                name: "MakeOIDCAvailable",
                table: "AppSettingss");
        }
    }
}
