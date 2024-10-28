using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BlackRise.Identity.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    ApplicationRoleId = table.Column<Guid>(type: "uuid", nullable: true),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaims_Roles_ApplicationRoleId",
                        column: x => x.ApplicationRoleId,
                        principalTable: "Roles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RoleClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogins_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UserTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "CreatedBy", "CreatedDate", "IsActive", "IsDeleted", "ModifiedBy", "ModifiedDate", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("4a9d216c-4f7f-429d-9a28-a084526ce818"), "65b48457-7e70-4361-8cc0-7e7c46bc9dac", new Guid("912c3a8a-d59d-4b7d-876a-3dd93a21c461"), new DateTime(2024, 10, 24, 19, 39, 2, 724, DateTimeKind.Utc).AddTicks(3155), true, false, new Guid("912c3a8a-d59d-4b7d-876a-3dd93a21c461"), new DateTime(2024, 10, 24, 19, 39, 2, 724, DateTimeKind.Utc).AddTicks(3156), "Admin", "ADMIN" },
                    { new Guid("920c0369-9b15-493d-b576-d806a271f748"), "02722f18-b994-445e-9cb4-37197481e878", new Guid("912c3a8a-d59d-4b7d-876a-3dd93a21c461"), new DateTime(2024, 10, 24, 19, 39, 2, 724, DateTimeKind.Utc).AddTicks(3161), true, false, new Guid("912c3a8a-d59d-4b7d-876a-3dd93a21c461"), new DateTime(2024, 10, 24, 19, 39, 2, 724, DateTimeKind.Utc).AddTicks(3162), "User", "USER" },
                    { new Guid("c8347abf-f51e-4dda-ba8d-2b3393a4aa63"), "ff3ef92a-9757-468c-a3ec-00a14209fb2c", new Guid("912c3a8a-d59d-4b7d-876a-3dd93a21c461"), new DateTime(2024, 10, 24, 19, 39, 2, 724, DateTimeKind.Utc).AddTicks(3144), true, false, new Guid("912c3a8a-d59d-4b7d-876a-3dd93a21c461"), new DateTime(2024, 10, 24, 19, 39, 2, 724, DateTimeKind.Utc).AddTicks(3147), "SuperAdmin", "SUPERADMIN" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedBy", "CreatedDate", "Email", "EmailConfirmed", "IsActive", "IsDeleted", "LockoutEnabled", "LockoutEnd", "ModifiedBy", "ModifiedDate", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { new Guid("23e09a08-ebde-4c5d-94f9-6a222c1a6362"), 0, "231d6055-699c-4e30-acdd-7e270ae493ac", new Guid("a901293b-f4cd-4b50-93da-158bc435c1f9"), new DateTime(2024, 10, 24, 19, 39, 2, 808, DateTimeKind.Utc).AddTicks(3205), "admin@blackrise.com", true, true, false, false, null, new Guid("8494a3ad-b74b-4407-a9ff-7a3f0c17770d"), new DateTime(2024, 10, 24, 19, 39, 2, 808, DateTimeKind.Utc).AddTicks(3212), "ADMIN@BLACKRISE.COM", "ADMIN@BLACKRISE.COM", "AQAAAAIAAYagAAAAEBPuv9UhHrRH4cmpDQt/dsLI2QGddL3kUWIhyp5zjkcSLbWzo7O56qp973gPDHR0NQ==", null, false, "b43e9e94-17a5-4f93-bb9f-2ebbf412187a", false, "admin@blackrise.com" },
                    { new Guid("3e96af2d-c6f2-4f9e-843e-ada493a79a17"), 0, "8c349594-2eb1-4b0b-9ff5-b8d473013cb9", new Guid("a901293b-f4cd-4b50-93da-158bc435c1f9"), new DateTime(2024, 10, 24, 19, 39, 2, 893, DateTimeKind.Utc).AddTicks(550), "user@blackrise.com", true, true, false, false, null, new Guid("8494a3ad-b74b-4407-a9ff-7a3f0c17770d"), new DateTime(2024, 10, 24, 19, 39, 2, 893, DateTimeKind.Utc).AddTicks(558), "USER@BLACKRISE.COM", "USER@BLACKRISE.COM", "AQAAAAIAAYagAAAAEDDNfi3SFT4xMihXKlOqLDsh88AwsAVrejiOHFZ1t5AlFtdnwA/LuYlAJb3GRbJffw==", null, false, "b98e9771-fb3c-4182-91d9-d7d3ebfd8959", false, "user@blackrise.com" },
                    { new Guid("912c3a8a-d59d-4b7d-876a-3dd93a21c461"), 0, "231d6055-699c-4e30-acdd-7e270ae493ac", new Guid("912c3a8a-d59d-4b7d-876a-3dd93a21c461"), new DateTime(2024, 10, 24, 19, 39, 2, 724, DateTimeKind.Utc).AddTicks(4988), "super-admin@blackrise.com", true, true, false, false, null, new Guid("912c3a8a-d59d-4b7d-876a-3dd93a21c461"), new DateTime(2024, 10, 24, 19, 39, 2, 724, DateTimeKind.Utc).AddTicks(4991), "SUPER-ADMIN@BLACKRISE.COM", "SUPER-ADMIN@BLACKRISE.COM", "AQAAAAIAAYagAAAAEKdrzXLVXcXJM2Z74Fxa8Yby63n1TlHVlh/N/cR51qgFo9s6KUjJnZgW41xSdWZ40Q==", null, false, "b43e9e94-17a5-4f93-bb9f-2ebbf412187a", false, "super-admin@blackrise.com" }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { new Guid("4a9d216c-4f7f-429d-9a28-a084526ce818"), new Guid("23e09a08-ebde-4c5d-94f9-6a222c1a6362") },
                    { new Guid("920c0369-9b15-493d-b576-d806a271f748"), new Guid("3e96af2d-c6f2-4f9e-843e-ada493a79a17") },
                    { new Guid("c8347abf-f51e-4dda-ba8d-2b3393a4aa63"), new Guid("912c3a8a-d59d-4b7d-876a-3dd93a21c461") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_ApplicationRoleId",
                table: "RoleClaims",
                column: "ApplicationRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_RoleId",
                table: "RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Roles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserId",
                table: "UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Users",
                column: "NormalizedUserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoleClaims");

            migrationBuilder.DropTable(
                name: "UserClaims");

            migrationBuilder.DropTable(
                name: "UserLogins");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserTokens");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
