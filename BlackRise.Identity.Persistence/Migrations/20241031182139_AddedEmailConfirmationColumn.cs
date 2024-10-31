using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlackRise.Identity.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedEmailConfirmationColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailConfirmationCode",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EmailConfirmationCodeExpiry",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("4a9d216c-4f7f-429d-9a28-a084526ce818"),
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2024, 10, 31, 18, 21, 39, 136, DateTimeKind.Utc).AddTicks(1991), new DateTime(2024, 10, 31, 18, 21, 39, 136, DateTimeKind.Utc).AddTicks(1991) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("920c0369-9b15-493d-b576-d806a271f748"),
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2024, 10, 31, 18, 21, 39, 136, DateTimeKind.Utc).AddTicks(1997), new DateTime(2024, 10, 31, 18, 21, 39, 136, DateTimeKind.Utc).AddTicks(1998) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("c8347abf-f51e-4dda-ba8d-2b3393a4aa63"),
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2024, 10, 31, 18, 21, 39, 136, DateTimeKind.Utc).AddTicks(1979), new DateTime(2024, 10, 31, 18, 21, 39, 136, DateTimeKind.Utc).AddTicks(1983) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("23e09a08-ebde-4c5d-94f9-6a222c1a6362"),
                columns: new[] { "CreatedDate", "EmailConfirmationCode", "EmailConfirmationCodeExpiry", "ModifiedDate", "PasswordHash" },
                values: new object[] { new DateTime(2024, 10, 31, 18, 21, 39, 222, DateTimeKind.Utc).AddTicks(783), null, null, new DateTime(2024, 10, 31, 18, 21, 39, 222, DateTimeKind.Utc).AddTicks(791), "AQAAAAIAAYagAAAAEJxnv5czkxLkUdqojeB06feWEr9src2UsJDXJ4nj57aH1Y8ztskLUltH6IEkNnm9iA==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("3e96af2d-c6f2-4f9e-843e-ada493a79a17"),
                columns: new[] { "CreatedDate", "EmailConfirmationCode", "EmailConfirmationCodeExpiry", "ModifiedDate", "PasswordHash" },
                values: new object[] { new DateTime(2024, 10, 31, 18, 21, 39, 312, DateTimeKind.Utc).AddTicks(5660), null, null, new DateTime(2024, 10, 31, 18, 21, 39, 312, DateTimeKind.Utc).AddTicks(5667), "AQAAAAIAAYagAAAAEGtKF4xRnbOFe4MUxOTU9XNpMNSnzu8xO3/dx/uvu7kve9JYGWB3ZQ/n9BJKOcSjPA==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("912c3a8a-d59d-4b7d-876a-3dd93a21c461"),
                columns: new[] { "CreatedDate", "EmailConfirmationCode", "EmailConfirmationCodeExpiry", "ModifiedDate", "PasswordHash" },
                values: new object[] { new DateTime(2024, 10, 31, 18, 21, 39, 136, DateTimeKind.Utc).AddTicks(3995), null, null, new DateTime(2024, 10, 31, 18, 21, 39, 136, DateTimeKind.Utc).AddTicks(3998), "AQAAAAIAAYagAAAAEMUPBhu4VGA5hGbZUfC4AJ3QRvuHf3a/zkASxTQ7I9tqPRzfujIOHpLDKs1kk/BS/w==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailConfirmationCode",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EmailConfirmationCodeExpiry",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("4a9d216c-4f7f-429d-9a28-a084526ce818"),
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2024, 10, 24, 19, 39, 2, 724, DateTimeKind.Utc).AddTicks(3155), new DateTime(2024, 10, 24, 19, 39, 2, 724, DateTimeKind.Utc).AddTicks(3156) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("920c0369-9b15-493d-b576-d806a271f748"),
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2024, 10, 24, 19, 39, 2, 724, DateTimeKind.Utc).AddTicks(3161), new DateTime(2024, 10, 24, 19, 39, 2, 724, DateTimeKind.Utc).AddTicks(3162) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("c8347abf-f51e-4dda-ba8d-2b3393a4aa63"),
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2024, 10, 24, 19, 39, 2, 724, DateTimeKind.Utc).AddTicks(3144), new DateTime(2024, 10, 24, 19, 39, 2, 724, DateTimeKind.Utc).AddTicks(3147) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("23e09a08-ebde-4c5d-94f9-6a222c1a6362"),
                columns: new[] { "CreatedDate", "ModifiedDate", "PasswordHash" },
                values: new object[] { new DateTime(2024, 10, 24, 19, 39, 2, 808, DateTimeKind.Utc).AddTicks(3205), new DateTime(2024, 10, 24, 19, 39, 2, 808, DateTimeKind.Utc).AddTicks(3212), "AQAAAAIAAYagAAAAEBPuv9UhHrRH4cmpDQt/dsLI2QGddL3kUWIhyp5zjkcSLbWzo7O56qp973gPDHR0NQ==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("3e96af2d-c6f2-4f9e-843e-ada493a79a17"),
                columns: new[] { "CreatedDate", "ModifiedDate", "PasswordHash" },
                values: new object[] { new DateTime(2024, 10, 24, 19, 39, 2, 893, DateTimeKind.Utc).AddTicks(550), new DateTime(2024, 10, 24, 19, 39, 2, 893, DateTimeKind.Utc).AddTicks(558), "AQAAAAIAAYagAAAAEDDNfi3SFT4xMihXKlOqLDsh88AwsAVrejiOHFZ1t5AlFtdnwA/LuYlAJb3GRbJffw==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("912c3a8a-d59d-4b7d-876a-3dd93a21c461"),
                columns: new[] { "CreatedDate", "ModifiedDate", "PasswordHash" },
                values: new object[] { new DateTime(2024, 10, 24, 19, 39, 2, 724, DateTimeKind.Utc).AddTicks(4988), new DateTime(2024, 10, 24, 19, 39, 2, 724, DateTimeKind.Utc).AddTicks(4991), "AQAAAAIAAYagAAAAEKdrzXLVXcXJM2Z74Fxa8Yby63n1TlHVlh/N/cR51qgFo9s6KUjJnZgW41xSdWZ40Q==" });
        }
    }
}
