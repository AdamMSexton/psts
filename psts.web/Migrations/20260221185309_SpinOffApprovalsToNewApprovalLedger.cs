using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace psts.web.Migrations
{
    /// <inheritdoc />
    public partial class SpinOffApprovalsToNewApprovalLedger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "PstsTimeAdjustmentApprovalLedger",
                columns: table => new
                {
                    ApprovedTransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionNum = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ApprovalAuthority = table.Column<string>(type: "text", nullable: false),
                    Approved = table.Column<bool>(type: "boolean", nullable: false),
                    Disapproved = table.Column<bool>(type: "boolean", nullable: false),
                    ApprovalTimeStamp = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    ApprovingUserId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PstsTimeAdjustmentApprovalLedger", x => x.ApprovedTransactionId);
                    table.ForeignKey(
                        name: "FK_PstsTimeAdjustmentApprovalLedger_AspNetUsers_ApprovingUserId",
                        column: x => x.ApprovingUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PstsTimeAdjustmentApprovalLedger_PstsTimeTransactionss_Appr~",
                        column: x => x.ApprovedTransactionId,
                        principalTable: "PstsTimeTransactionss",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PstsTimeAdjustmentApprovalLedger_ApprovingUserId",
                table: "PstsTimeAdjustmentApprovalLedger",
                column: "ApprovingUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PstsTimeAdjustmentApprovalLedger");

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
    }
}
