using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace psts.web.Migrations
{
    /// <inheritdoc />
    public partial class BaselineDBCompleteAddAppSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PstsBillingRateResolutionSchedule_AspNetUsers_ChangedBy",
                table: "PstsBillingRateResolutionSchedule");

            migrationBuilder.DropForeignKey(
                name: "FK_PstsBillingRateResolutionSchedule_AspNetUsers_EmployeeId",
                table: "PstsBillingRateResolutionSchedule");

            migrationBuilder.DropForeignKey(
                name: "FK_PstsBillingRateResolutionSchedule_PstsClientProfiles_Client~",
                table: "PstsBillingRateResolutionSchedule");

            migrationBuilder.DropForeignKey(
                name: "FK_PstsBillingRateResolutionSchedule_PstsProjectDefinitions_Pr~",
                table: "PstsBillingRateResolutionSchedule");

            migrationBuilder.DropForeignKey(
                name: "FK_PstsBillingRateResolutionSchedule_PstsTaskDefinitions_TaskId",
                table: "PstsBillingRateResolutionSchedule");

            migrationBuilder.DropForeignKey(
                name: "FK_PstsTimeTransactions_AspNetUsers_EnteredBy",
                table: "PstsTimeTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_PstsTimeTransactions_AspNetUsers_WorkCompletedBy",
                table: "PstsTimeTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_PstsTimeTransactions_PstsTaskDefinitions_TaskId",
                table: "PstsTimeTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_PstsTimeTransactions_PstsTimeTransactions_RelatedId",
                table: "PstsTimeTransactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PstsTimeTransactions",
                table: "PstsTimeTransactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PstsBillingRateResolutionSchedule",
                table: "PstsBillingRateResolutionSchedule");

            migrationBuilder.RenameTable(
                name: "PstsTimeTransactions",
                newName: "PstsTimeTransactionss");

            migrationBuilder.RenameTable(
                name: "PstsBillingRateResolutionSchedule",
                newName: "pstsBillingRateResolutionSchedules");

            migrationBuilder.RenameIndex(
                name: "IX_PstsTimeTransactions_WorkCompletedBy",
                table: "PstsTimeTransactionss",
                newName: "IX_PstsTimeTransactionss_WorkCompletedBy");

            migrationBuilder.RenameIndex(
                name: "IX_PstsTimeTransactions_TaskId",
                table: "PstsTimeTransactionss",
                newName: "IX_PstsTimeTransactionss_TaskId");

            migrationBuilder.RenameIndex(
                name: "IX_PstsTimeTransactions_RelatedId",
                table: "PstsTimeTransactionss",
                newName: "IX_PstsTimeTransactionss_RelatedId");

            migrationBuilder.RenameIndex(
                name: "IX_PstsTimeTransactions_EnteredBy",
                table: "PstsTimeTransactionss",
                newName: "IX_PstsTimeTransactionss_EnteredBy");

            migrationBuilder.RenameIndex(
                name: "IX_PstsBillingRateResolutionSchedule_TaskId",
                table: "pstsBillingRateResolutionSchedules",
                newName: "IX_pstsBillingRateResolutionSchedules_TaskId");

            migrationBuilder.RenameIndex(
                name: "IX_PstsBillingRateResolutionSchedule_ProjectId",
                table: "pstsBillingRateResolutionSchedules",
                newName: "IX_pstsBillingRateResolutionSchedules_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_PstsBillingRateResolutionSchedule_EmployeeId",
                table: "pstsBillingRateResolutionSchedules",
                newName: "IX_pstsBillingRateResolutionSchedules_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_PstsBillingRateResolutionSchedule_ClientId",
                table: "pstsBillingRateResolutionSchedules",
                newName: "IX_pstsBillingRateResolutionSchedules_ClientId");

            migrationBuilder.RenameIndex(
                name: "IX_PstsBillingRateResolutionSchedule_ChangedBy",
                table: "pstsBillingRateResolutionSchedules",
                newName: "IX_pstsBillingRateResolutionSchedules_ChangedBy");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PstsTimeTransactionss",
                table: "PstsTimeTransactionss",
                column: "TransactionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_pstsBillingRateResolutionSchedules",
                table: "pstsBillingRateResolutionSchedules",
                column: "BillingRateNum");

            migrationBuilder.CreateTable(
                name: "AppSettingss",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OIDCEnabledByDefault = table.Column<bool>(type: "boolean", nullable: false),
                    ManagerApprovalForAdjustments = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSettingss", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_pstsBillingRateResolutionSchedules_AspNetUsers_ChangedBy",
                table: "pstsBillingRateResolutionSchedules",
                column: "ChangedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_pstsBillingRateResolutionSchedules_AspNetUsers_EmployeeId",
                table: "pstsBillingRateResolutionSchedules",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_pstsBillingRateResolutionSchedules_PstsClientProfiles_Clien~",
                table: "pstsBillingRateResolutionSchedules",
                column: "ClientId",
                principalTable: "PstsClientProfiles",
                principalColumn: "ClientId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_pstsBillingRateResolutionSchedules_PstsProjectDefinitions_P~",
                table: "pstsBillingRateResolutionSchedules",
                column: "ProjectId",
                principalTable: "PstsProjectDefinitions",
                principalColumn: "ProjectId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_pstsBillingRateResolutionSchedules_PstsTaskDefinitions_Task~",
                table: "pstsBillingRateResolutionSchedules",
                column: "TaskId",
                principalTable: "PstsTaskDefinitions",
                principalColumn: "TaskId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PstsTimeTransactionss_AspNetUsers_EnteredBy",
                table: "PstsTimeTransactionss",
                column: "EnteredBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PstsTimeTransactionss_AspNetUsers_WorkCompletedBy",
                table: "PstsTimeTransactionss",
                column: "WorkCompletedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PstsTimeTransactionss_PstsTaskDefinitions_TaskId",
                table: "PstsTimeTransactionss",
                column: "TaskId",
                principalTable: "PstsTaskDefinitions",
                principalColumn: "TaskId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PstsTimeTransactionss_PstsTimeTransactionss_RelatedId",
                table: "PstsTimeTransactionss",
                column: "RelatedId",
                principalTable: "PstsTimeTransactionss",
                principalColumn: "TransactionId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_pstsBillingRateResolutionSchedules_AspNetUsers_ChangedBy",
                table: "pstsBillingRateResolutionSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_pstsBillingRateResolutionSchedules_AspNetUsers_EmployeeId",
                table: "pstsBillingRateResolutionSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_pstsBillingRateResolutionSchedules_PstsClientProfiles_Clien~",
                table: "pstsBillingRateResolutionSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_pstsBillingRateResolutionSchedules_PstsProjectDefinitions_P~",
                table: "pstsBillingRateResolutionSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_pstsBillingRateResolutionSchedules_PstsTaskDefinitions_Task~",
                table: "pstsBillingRateResolutionSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_PstsTimeTransactionss_AspNetUsers_EnteredBy",
                table: "PstsTimeTransactionss");

            migrationBuilder.DropForeignKey(
                name: "FK_PstsTimeTransactionss_AspNetUsers_WorkCompletedBy",
                table: "PstsTimeTransactionss");

            migrationBuilder.DropForeignKey(
                name: "FK_PstsTimeTransactionss_PstsTaskDefinitions_TaskId",
                table: "PstsTimeTransactionss");

            migrationBuilder.DropForeignKey(
                name: "FK_PstsTimeTransactionss_PstsTimeTransactionss_RelatedId",
                table: "PstsTimeTransactionss");

            migrationBuilder.DropTable(
                name: "AppSettingss");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PstsTimeTransactionss",
                table: "PstsTimeTransactionss");

            migrationBuilder.DropPrimaryKey(
                name: "PK_pstsBillingRateResolutionSchedules",
                table: "pstsBillingRateResolutionSchedules");

            migrationBuilder.RenameTable(
                name: "PstsTimeTransactionss",
                newName: "PstsTimeTransactions");

            migrationBuilder.RenameTable(
                name: "pstsBillingRateResolutionSchedules",
                newName: "PstsBillingRateResolutionSchedule");

            migrationBuilder.RenameIndex(
                name: "IX_PstsTimeTransactionss_WorkCompletedBy",
                table: "PstsTimeTransactions",
                newName: "IX_PstsTimeTransactions_WorkCompletedBy");

            migrationBuilder.RenameIndex(
                name: "IX_PstsTimeTransactionss_TaskId",
                table: "PstsTimeTransactions",
                newName: "IX_PstsTimeTransactions_TaskId");

            migrationBuilder.RenameIndex(
                name: "IX_PstsTimeTransactionss_RelatedId",
                table: "PstsTimeTransactions",
                newName: "IX_PstsTimeTransactions_RelatedId");

            migrationBuilder.RenameIndex(
                name: "IX_PstsTimeTransactionss_EnteredBy",
                table: "PstsTimeTransactions",
                newName: "IX_PstsTimeTransactions_EnteredBy");

            migrationBuilder.RenameIndex(
                name: "IX_pstsBillingRateResolutionSchedules_TaskId",
                table: "PstsBillingRateResolutionSchedule",
                newName: "IX_PstsBillingRateResolutionSchedule_TaskId");

            migrationBuilder.RenameIndex(
                name: "IX_pstsBillingRateResolutionSchedules_ProjectId",
                table: "PstsBillingRateResolutionSchedule",
                newName: "IX_PstsBillingRateResolutionSchedule_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_pstsBillingRateResolutionSchedules_EmployeeId",
                table: "PstsBillingRateResolutionSchedule",
                newName: "IX_PstsBillingRateResolutionSchedule_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_pstsBillingRateResolutionSchedules_ClientId",
                table: "PstsBillingRateResolutionSchedule",
                newName: "IX_PstsBillingRateResolutionSchedule_ClientId");

            migrationBuilder.RenameIndex(
                name: "IX_pstsBillingRateResolutionSchedules_ChangedBy",
                table: "PstsBillingRateResolutionSchedule",
                newName: "IX_PstsBillingRateResolutionSchedule_ChangedBy");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PstsTimeTransactions",
                table: "PstsTimeTransactions",
                column: "TransactionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PstsBillingRateResolutionSchedule",
                table: "PstsBillingRateResolutionSchedule",
                column: "BillingRateNum");

            migrationBuilder.AddForeignKey(
                name: "FK_PstsBillingRateResolutionSchedule_AspNetUsers_ChangedBy",
                table: "PstsBillingRateResolutionSchedule",
                column: "ChangedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PstsBillingRateResolutionSchedule_AspNetUsers_EmployeeId",
                table: "PstsBillingRateResolutionSchedule",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PstsBillingRateResolutionSchedule_PstsClientProfiles_Client~",
                table: "PstsBillingRateResolutionSchedule",
                column: "ClientId",
                principalTable: "PstsClientProfiles",
                principalColumn: "ClientId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PstsBillingRateResolutionSchedule_PstsProjectDefinitions_Pr~",
                table: "PstsBillingRateResolutionSchedule",
                column: "ProjectId",
                principalTable: "PstsProjectDefinitions",
                principalColumn: "ProjectId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PstsBillingRateResolutionSchedule_PstsTaskDefinitions_TaskId",
                table: "PstsBillingRateResolutionSchedule",
                column: "TaskId",
                principalTable: "PstsTaskDefinitions",
                principalColumn: "TaskId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PstsTimeTransactions_AspNetUsers_EnteredBy",
                table: "PstsTimeTransactions",
                column: "EnteredBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PstsTimeTransactions_AspNetUsers_WorkCompletedBy",
                table: "PstsTimeTransactions",
                column: "WorkCompletedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PstsTimeTransactions_PstsTaskDefinitions_TaskId",
                table: "PstsTimeTransactions",
                column: "TaskId",
                principalTable: "PstsTaskDefinitions",
                principalColumn: "TaskId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PstsTimeTransactions_PstsTimeTransactions_RelatedId",
                table: "PstsTimeTransactions",
                column: "RelatedId",
                principalTable: "PstsTimeTransactions",
                principalColumn: "TransactionId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
