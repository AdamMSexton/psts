using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace psts.web.Migrations
{
    /// <inheritdoc />
    public partial class SpinOffApprovalsToNewApprovalLedger2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PstsTimeAdjustmentApprovalLedger_AspNetUsers_ApprovingUserId",
                table: "PstsTimeAdjustmentApprovalLedger");

            migrationBuilder.DropForeignKey(
                name: "FK_PstsTimeAdjustmentApprovalLedger_PstsTimeTransactionss_Appr~",
                table: "PstsTimeAdjustmentApprovalLedger");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PstsTimeAdjustmentApprovalLedger",
                table: "PstsTimeAdjustmentApprovalLedger");

            migrationBuilder.DropColumn(
                name: "Approved",
                table: "PstsTimeAdjustmentApprovalLedger");

            migrationBuilder.DropColumn(
                name: "Disapproved",
                table: "PstsTimeAdjustmentApprovalLedger");

            migrationBuilder.RenameTable(
                name: "PstsTimeAdjustmentApprovalLedger",
                newName: "pstsTimeAdjustmentApprovalLedgers");

            migrationBuilder.RenameColumn(
                name: "ApprovalTimeStamp",
                table: "pstsTimeAdjustmentApprovalLedgers",
                newName: "DecisionTimeStamp");

            migrationBuilder.RenameColumn(
                name: "ApprovedTransactionId",
                table: "pstsTimeAdjustmentApprovalLedgers",
                newName: "SubjectTransactionId");

            migrationBuilder.RenameIndex(
                name: "IX_PstsTimeAdjustmentApprovalLedger_ApprovingUserId",
                table: "pstsTimeAdjustmentApprovalLedgers",
                newName: "IX_pstsTimeAdjustmentApprovalLedgers_ApprovingUserId");

            migrationBuilder.AddColumn<short>(
                name: "Decision",
                table: "pstsTimeAdjustmentApprovalLedgers",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "pstsTimeAdjustmentApprovalLedgers",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_pstsTimeAdjustmentApprovalLedgers",
                table: "pstsTimeAdjustmentApprovalLedgers",
                column: "SubjectTransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_pstsTimeAdjustmentApprovalLedgers_AspNetUsers_ApprovingUser~",
                table: "pstsTimeAdjustmentApprovalLedgers",
                column: "ApprovingUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_pstsTimeAdjustmentApprovalLedgers_PstsTimeTransactionss_Sub~",
                table: "pstsTimeAdjustmentApprovalLedgers",
                column: "SubjectTransactionId",
                principalTable: "PstsTimeTransactionss",
                principalColumn: "TransactionId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_pstsTimeAdjustmentApprovalLedgers_AspNetUsers_ApprovingUser~",
                table: "pstsTimeAdjustmentApprovalLedgers");

            migrationBuilder.DropForeignKey(
                name: "FK_pstsTimeAdjustmentApprovalLedgers_PstsTimeTransactionss_Sub~",
                table: "pstsTimeAdjustmentApprovalLedgers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_pstsTimeAdjustmentApprovalLedgers",
                table: "pstsTimeAdjustmentApprovalLedgers");

            migrationBuilder.DropColumn(
                name: "Decision",
                table: "pstsTimeAdjustmentApprovalLedgers");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "pstsTimeAdjustmentApprovalLedgers");

            migrationBuilder.RenameTable(
                name: "pstsTimeAdjustmentApprovalLedgers",
                newName: "PstsTimeAdjustmentApprovalLedger");

            migrationBuilder.RenameColumn(
                name: "DecisionTimeStamp",
                table: "PstsTimeAdjustmentApprovalLedger",
                newName: "ApprovalTimeStamp");

            migrationBuilder.RenameColumn(
                name: "SubjectTransactionId",
                table: "PstsTimeAdjustmentApprovalLedger",
                newName: "ApprovedTransactionId");

            migrationBuilder.RenameIndex(
                name: "IX_pstsTimeAdjustmentApprovalLedgers_ApprovingUserId",
                table: "PstsTimeAdjustmentApprovalLedger",
                newName: "IX_PstsTimeAdjustmentApprovalLedger_ApprovingUserId");

            migrationBuilder.AddColumn<bool>(
                name: "Approved",
                table: "PstsTimeAdjustmentApprovalLedger",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Disapproved",
                table: "PstsTimeAdjustmentApprovalLedger",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PstsTimeAdjustmentApprovalLedger",
                table: "PstsTimeAdjustmentApprovalLedger",
                column: "ApprovedTransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_PstsTimeAdjustmentApprovalLedger_AspNetUsers_ApprovingUserId",
                table: "PstsTimeAdjustmentApprovalLedger",
                column: "ApprovingUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PstsTimeAdjustmentApprovalLedger_PstsTimeTransactionss_Appr~",
                table: "PstsTimeAdjustmentApprovalLedger",
                column: "ApprovedTransactionId",
                principalTable: "PstsTimeTransactionss",
                principalColumn: "TransactionId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
