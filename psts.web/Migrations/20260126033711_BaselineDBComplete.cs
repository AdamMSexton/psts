using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace psts.web.Migrations
{
    /// <inheritdoc />
    public partial class BaselineDBComplete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PstsClientProfiles",
                columns: table => new
                {
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeePOCId = table.Column<string>(type: "text", nullable: true),
                    ClientName = table.Column<string>(type: "text", nullable: false),
                    ClientPOCfName = table.Column<string>(type: "text", nullable: false),
                    ClientPOClName = table.Column<string>(type: "text", nullable: false),
                    ClientPOCeMail = table.Column<string>(type: "text", nullable: false),
                    ClientPOCtPhone = table.Column<string>(type: "text", nullable: false),
                    ShortCode = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PstsClientProfiles", x => x.ClientId);
                    table.CheckConstraint("CK_Project_ShortCode_Format", "char_length(\"ShortCode\") = 4 AND \"ShortCode\" = upper(\"ShortCode\")");
                    table.ForeignKey(
                        name: "FK_PstsClientProfiles_AspNetUsers_EmployeePOCId",
                        column: x => x.EmployeePOCId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PstsUserProfiles",
                columns: table => new
                {
                    EmployeeId = table.Column<string>(type: "text", nullable: false),
                    FName = table.Column<string>(type: "text", nullable: false),
                    LName = table.Column<string>(type: "text", nullable: false),
                    ManagerId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PstsUserProfiles", x => x.EmployeeId);
                    table.ForeignKey(
                        name: "FK_PstsUserProfiles_AspNetUsers_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PstsUserProfiles_AspNetUsers_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PstsProjectDefinitions",
                columns: table => new
                {
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeePOCId = table.Column<string>(type: "text", nullable: true),
                    ProjectName = table.Column<string>(type: "text", nullable: false),
                    ProjectDescription = table.Column<string>(type: "text", nullable: false),
                    ShortCode = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PstsProjectDefinitions", x => x.ProjectId);
                    table.CheckConstraint("CK_Project_ShortCode_Format", "char_length(\"ShortCode\") = 4 AND \"ShortCode\" = upper(\"ShortCode\")");
                    table.ForeignKey(
                        name: "FK_PstsProjectDefinitions_AspNetUsers_EmployeePOCId",
                        column: x => x.EmployeePOCId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PstsProjectDefinitions_PstsClientProfiles_ClientId",
                        column: x => x.ClientId,
                        principalTable: "PstsClientProfiles",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PstsTaskDefinitions",
                columns: table => new
                {
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskName = table.Column<string>(type: "text", nullable: false),
                    TaskDescription = table.Column<string>(type: "text", nullable: false),
                    ShortCode = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PstsTaskDefinitions", x => x.TaskId);
                    table.CheckConstraint("CK_Project_ShortCode_Format", "char_length(\"ShortCode\") = 4 AND \"ShortCode\" = upper(\"ShortCode\")");
                    table.ForeignKey(
                        name: "FK_PstsTaskDefinitions_PstsProjectDefinitions_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "PstsProjectDefinitions",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PstsBillingRateResolutionSchedule",
                columns: table => new
                {
                    BillingRateNum = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: true),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: true),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: true),
                    EmployeeId = table.Column<string>(type: "text", nullable: true),
                    EffectiveAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    HourlyRate = table.Column<decimal>(type: "numeric", nullable: false),
                    ChangedBy = table.Column<string>(type: "text", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PstsBillingRateResolutionSchedule", x => x.BillingRateNum);
                    table.ForeignKey(
                        name: "FK_PstsBillingRateResolutionSchedule_AspNetUsers_ChangedBy",
                        column: x => x.ChangedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PstsBillingRateResolutionSchedule_AspNetUsers_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PstsBillingRateResolutionSchedule_PstsClientProfiles_Client~",
                        column: x => x.ClientId,
                        principalTable: "PstsClientProfiles",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PstsBillingRateResolutionSchedule_PstsProjectDefinitions_Pr~",
                        column: x => x.ProjectId,
                        principalTable: "PstsProjectDefinitions",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PstsBillingRateResolutionSchedule_PstsTaskDefinitions_TaskId",
                        column: x => x.TaskId,
                        principalTable: "PstsTaskDefinitions",
                        principalColumn: "TaskId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PstsTimeTransactions",
                columns: table => new
                {
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionNum = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    EnteredBy = table.Column<string>(type: "text", nullable: false),
                    WorkCompletedBy = table.Column<string>(type: "text", nullable: false),
                    EnterdTimeStamp = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    WorkCompletedDate = table.Column<DateOnly>(type: "date", nullable: false),
                    RelatedId = table.Column<Guid>(type: "uuid", nullable: true),
                    WorkCompletedHours = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PstsTimeTransactions", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_PstsTimeTransactions_AspNetUsers_EnteredBy",
                        column: x => x.EnteredBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PstsTimeTransactions_AspNetUsers_WorkCompletedBy",
                        column: x => x.WorkCompletedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PstsTimeTransactions_PstsTaskDefinitions_TaskId",
                        column: x => x.TaskId,
                        principalTable: "PstsTaskDefinitions",
                        principalColumn: "TaskId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PstsTimeTransactions_PstsTimeTransactions_RelatedId",
                        column: x => x.RelatedId,
                        principalTable: "PstsTimeTransactions",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PstsBillingRateResolutionSchedule_ChangedBy",
                table: "PstsBillingRateResolutionSchedule",
                column: "ChangedBy",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PstsBillingRateResolutionSchedule_ClientId",
                table: "PstsBillingRateResolutionSchedule",
                column: "ClientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PstsBillingRateResolutionSchedule_EmployeeId",
                table: "PstsBillingRateResolutionSchedule",
                column: "EmployeeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PstsBillingRateResolutionSchedule_ProjectId",
                table: "PstsBillingRateResolutionSchedule",
                column: "ProjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PstsBillingRateResolutionSchedule_TaskId",
                table: "PstsBillingRateResolutionSchedule",
                column: "TaskId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PstsClientProfiles_EmployeePOCId",
                table: "PstsClientProfiles",
                column: "EmployeePOCId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PstsClientProfiles_ShortCode",
                table: "PstsClientProfiles",
                column: "ShortCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PstsProjectDefinitions_ClientId",
                table: "PstsProjectDefinitions",
                column: "ClientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PstsProjectDefinitions_EmployeePOCId",
                table: "PstsProjectDefinitions",
                column: "EmployeePOCId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PstsProjectDefinitions_ShortCode",
                table: "PstsProjectDefinitions",
                column: "ShortCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PstsTaskDefinitions_ProjectId",
                table: "PstsTaskDefinitions",
                column: "ProjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PstsTaskDefinitions_ShortCode",
                table: "PstsTaskDefinitions",
                column: "ShortCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PstsTimeTransactions_EnteredBy",
                table: "PstsTimeTransactions",
                column: "EnteredBy",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PstsTimeTransactions_RelatedId",
                table: "PstsTimeTransactions",
                column: "RelatedId");

            migrationBuilder.CreateIndex(
                name: "IX_PstsTimeTransactions_TaskId",
                table: "PstsTimeTransactions",
                column: "TaskId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PstsTimeTransactions_WorkCompletedBy",
                table: "PstsTimeTransactions",
                column: "WorkCompletedBy",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PstsUserProfiles_ManagerId",
                table: "PstsUserProfiles",
                column: "ManagerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "PstsBillingRateResolutionSchedule");

            migrationBuilder.DropTable(
                name: "PstsTimeTransactions");

            migrationBuilder.DropTable(
                name: "PstsUserProfiles");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "PstsTaskDefinitions");

            migrationBuilder.DropTable(
                name: "PstsProjectDefinitions");

            migrationBuilder.DropTable(
                name: "PstsClientProfiles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
