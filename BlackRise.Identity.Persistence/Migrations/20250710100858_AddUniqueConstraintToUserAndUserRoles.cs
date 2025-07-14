using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlackRise.Identity.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraintToUserAndUserRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppleId",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsProfileCreated",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSocialLogin",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsProfileCompleted",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("4a9d216c-4f7f-429d-9a28-a084526ce818"),
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 7, 10, 10, 8, 57, 887, DateTimeKind.Utc).AddTicks(9547), new DateTime(2025, 7, 10, 10, 8, 57, 887, DateTimeKind.Utc).AddTicks(9548) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("920c0369-9b15-493d-b576-d806a271f748"),
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 7, 10, 10, 8, 57, 887, DateTimeKind.Utc).AddTicks(9551), new DateTime(2025, 7, 10, 10, 8, 57, 887, DateTimeKind.Utc).AddTicks(9552) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("c8347abf-f51e-4dda-ba8d-2b3393a4aa63"),
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 7, 10, 10, 8, 57, 887, DateTimeKind.Utc).AddTicks(9542), new DateTime(2025, 7, 10, 10, 8, 57, 887, DateTimeKind.Utc).AddTicks(9543) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("23e09a08-ebde-4c5d-94f9-6a222c1a6362"),
                columns: new[] { "AppleId", "CreatedDate", "IsProfileCreated", "IsSocialLogin", "ModifiedDate", "PasswordHash" },
                values: new object[] { null, new DateTime(2025, 7, 10, 10, 8, 57, 932, DateTimeKind.Utc).AddTicks(976), false, false, new DateTime(2025, 7, 10, 10, 8, 57, 932, DateTimeKind.Utc).AddTicks(977), "AQAAAAIAAYagAAAAENvjHLdpI8tGD7M0VeULj98RoR1ha6NAADdi8bZok/inHyYLauJ2kyuQxaCdHQu5tQ==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("3e96af2d-c6f2-4f9e-843e-ada493a79a17"),
                columns: new[] { "AppleId", "CreatedDate", "IsProfileCreated", "IsSocialLogin", "ModifiedDate", "PasswordHash" },
                values: new object[] { null, new DateTime(2025, 7, 10, 10, 8, 57, 974, DateTimeKind.Utc).AddTicks(8858), false, false, new DateTime(2025, 7, 10, 10, 8, 57, 974, DateTimeKind.Utc).AddTicks(8860), "AQAAAAIAAYagAAAAEKl2KvFyhuiau/kMnmzKO80juzWoLSkK9QN5E3JiStSULiAOaBjMALczWMFcfC252Q==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("912c3a8a-d59d-4b7d-876a-3dd93a21c461"),
                columns: new[] { "AppleId", "CreatedDate", "IsProfileCreated", "IsSocialLogin", "ModifiedDate", "PasswordHash" },
                values: new object[] { null, new DateTime(2025, 7, 10, 10, 8, 57, 888, DateTimeKind.Utc).AddTicks(999), false, false, new DateTime(2025, 7, 10, 10, 8, 57, 888, DateTimeKind.Utc).AddTicks(1000), "AQAAAAIAAYagAAAAEKXicol8fwCGM3eg1yzo2OHMjQxjc//+3bRfgpkqWW2arum2qRH2WiawLQ+ls7VUZw==" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId_RoleId",
                table: "UserRoles",
                columns: new[] { "UserId", "RoleId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_UserRoles_UserId_RoleId",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "AppleId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsProfileCreated",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsSocialLogin",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsProfileCompleted",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("4a9d216c-4f7f-429d-9a28-a084526ce818"),
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2024, 11, 3, 7, 31, 11, 303, DateTimeKind.Utc).AddTicks(4987), new DateTime(2024, 11, 3, 7, 31, 11, 303, DateTimeKind.Utc).AddTicks(4988) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("920c0369-9b15-493d-b576-d806a271f748"),
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2024, 11, 3, 7, 31, 11, 303, DateTimeKind.Utc).AddTicks(4994), new DateTime(2024, 11, 3, 7, 31, 11, 303, DateTimeKind.Utc).AddTicks(4995) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("c8347abf-f51e-4dda-ba8d-2b3393a4aa63"),
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2024, 11, 3, 7, 31, 11, 303, DateTimeKind.Utc).AddTicks(4977), new DateTime(2024, 11, 3, 7, 31, 11, 303, DateTimeKind.Utc).AddTicks(4979) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("23e09a08-ebde-4c5d-94f9-6a222c1a6362"),
                columns: new[] { "CreatedDate", "ModifiedDate", "PasswordHash" },
                values: new object[] { new DateTime(2024, 11, 3, 7, 31, 11, 404, DateTimeKind.Utc).AddTicks(3026), new DateTime(2024, 11, 3, 7, 31, 11, 404, DateTimeKind.Utc).AddTicks(3031), "AQAAAAIAAYagAAAAEM3Qs3TFwtvRkqbrYYQHqz9eK8lSzvtuRSnHtSTTa+Dihzoch1f0rXB5v3dbh1E9cg==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("3e96af2d-c6f2-4f9e-843e-ada493a79a17"),
                columns: new[] { "CreatedDate", "ModifiedDate", "PasswordHash" },
                values: new object[] { new DateTime(2024, 11, 3, 7, 31, 11, 492, DateTimeKind.Utc).AddTicks(4032), new DateTime(2024, 11, 3, 7, 31, 11, 492, DateTimeKind.Utc).AddTicks(4039), "AQAAAAIAAYagAAAAEAgem6F/EBbyD/Ljjc0nrvDc8ZNUisLQHk7qEA6Pkeaafs6DOg48maNpL4HMuphFVg==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("912c3a8a-d59d-4b7d-876a-3dd93a21c461"),
                columns: new[] { "CreatedDate", "ModifiedDate", "PasswordHash" },
                values: new object[] { new DateTime(2024, 11, 3, 7, 31, 11, 303, DateTimeKind.Utc).AddTicks(6913), new DateTime(2024, 11, 3, 7, 31, 11, 303, DateTimeKind.Utc).AddTicks(6916), "AQAAAAIAAYagAAAAELPI2jXdT+Gr8tOihc1UWZEa5MI4ZLKLuOYNfqMWSgUI/bN9/y33ksTNWYjPsQ7grA==" });
        }
    }
}
