using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlackRise.Identity.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EmailCodeConfirmTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastEmailCodeConfirmTime",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("4a9d216c-4f7f-429d-9a28-a084526ce818"),
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 8, 5, 11, 24, 29, 83, DateTimeKind.Utc).AddTicks(4668), new DateTime(2025, 8, 5, 11, 24, 29, 83, DateTimeKind.Utc).AddTicks(4669) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("920c0369-9b15-493d-b576-d806a271f748"),
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 8, 5, 11, 24, 29, 83, DateTimeKind.Utc).AddTicks(4673), new DateTime(2025, 8, 5, 11, 24, 29, 83, DateTimeKind.Utc).AddTicks(4673) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("c8347abf-f51e-4dda-ba8d-2b3393a4aa63"),
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 8, 5, 11, 24, 29, 83, DateTimeKind.Utc).AddTicks(4662), new DateTime(2025, 8, 5, 11, 24, 29, 83, DateTimeKind.Utc).AddTicks(4664) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("23e09a08-ebde-4c5d-94f9-6a222c1a6362"),
                columns: new[] { "CreatedDate", "LastEmailCodeConfirmTime", "ModifiedDate", "PasswordHash" },
                values: new object[] { new DateTime(2025, 8, 5, 11, 24, 29, 121, DateTimeKind.Utc).AddTicks(1567), null, new DateTime(2025, 8, 5, 11, 24, 29, 121, DateTimeKind.Utc).AddTicks(1573), "AQAAAAIAAYagAAAAEI/7zB8fyScs8aThg5XpZRfG5VBtXrY6+MwM2mdJA3JWfsFqY5yJrSk+cAknystgrg==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("3e96af2d-c6f2-4f9e-843e-ada493a79a17"),
                columns: new[] { "CreatedDate", "LastEmailCodeConfirmTime", "ModifiedDate", "PasswordHash" },
                values: new object[] { new DateTime(2025, 8, 5, 11, 24, 29, 158, DateTimeKind.Utc).AddTicks(3689), null, new DateTime(2025, 8, 5, 11, 24, 29, 158, DateTimeKind.Utc).AddTicks(3692), "AQAAAAIAAYagAAAAEJCXL9EmUmHP6mpgejCYLAYeWMahZ58kdrfvmypIarq4QvuMGpo01JY6VKEvxBGhfQ==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("912c3a8a-d59d-4b7d-876a-3dd93a21c461"),
                columns: new[] { "CreatedDate", "LastEmailCodeConfirmTime", "ModifiedDate", "PasswordHash" },
                values: new object[] { new DateTime(2025, 8, 5, 11, 24, 29, 83, DateTimeKind.Utc).AddTicks(6328), null, new DateTime(2025, 8, 5, 11, 24, 29, 83, DateTimeKind.Utc).AddTicks(6330), "AQAAAAIAAYagAAAAEOICJ7wpSJlH5jDmP+nV//u9rmVDVF79MeXXNs9qrTrsBwdKn5qcV0BGi0XcFW48fQ==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastEmailCodeConfirmTime",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("4a9d216c-4f7f-429d-9a28-a084526ce818"),
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 7, 28, 12, 30, 11, 465, DateTimeKind.Utc).AddTicks(8685), new DateTime(2025, 7, 28, 12, 30, 11, 465, DateTimeKind.Utc).AddTicks(8686) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("920c0369-9b15-493d-b576-d806a271f748"),
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 7, 28, 12, 30, 11, 465, DateTimeKind.Utc).AddTicks(8690), new DateTime(2025, 7, 28, 12, 30, 11, 465, DateTimeKind.Utc).AddTicks(8691) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("c8347abf-f51e-4dda-ba8d-2b3393a4aa63"),
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 7, 28, 12, 30, 11, 465, DateTimeKind.Utc).AddTicks(8680), new DateTime(2025, 7, 28, 12, 30, 11, 465, DateTimeKind.Utc).AddTicks(8681) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("23e09a08-ebde-4c5d-94f9-6a222c1a6362"),
                columns: new[] { "CreatedDate", "ModifiedDate", "PasswordHash" },
                values: new object[] { new DateTime(2025, 7, 28, 12, 30, 11, 501, DateTimeKind.Utc).AddTicks(7356), new DateTime(2025, 7, 28, 12, 30, 11, 501, DateTimeKind.Utc).AddTicks(7358), "AQAAAAIAAYagAAAAEPNHc4l7YNYkWEqPQPD3m116p+RefdEhNYYhWv2529kUq3iXX123IHviW3O5IHyCag==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("3e96af2d-c6f2-4f9e-843e-ada493a79a17"),
                columns: new[] { "CreatedDate", "ModifiedDate", "PasswordHash" },
                values: new object[] { new DateTime(2025, 7, 28, 12, 30, 11, 537, DateTimeKind.Utc).AddTicks(4832), new DateTime(2025, 7, 28, 12, 30, 11, 537, DateTimeKind.Utc).AddTicks(4833), "AQAAAAIAAYagAAAAEKyVg/oZ7anywXIPMFbwixQG0jxisgBN09kyhtX3DzDCiOB2cCceUnR1ODk5tuB+9A==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("912c3a8a-d59d-4b7d-876a-3dd93a21c461"),
                columns: new[] { "CreatedDate", "ModifiedDate", "PasswordHash" },
                values: new object[] { new DateTime(2025, 7, 28, 12, 30, 11, 466, DateTimeKind.Utc).AddTicks(95), new DateTime(2025, 7, 28, 12, 30, 11, 466, DateTimeKind.Utc).AddTicks(97), "AQAAAAIAAYagAAAAEGhACgv2S6zHSw4oXYnIWIntq2dFRt/dboVf6cAtjnw3dSK/u3V6yrsTIGnl4s6KsQ==" });
        }
    }
}
