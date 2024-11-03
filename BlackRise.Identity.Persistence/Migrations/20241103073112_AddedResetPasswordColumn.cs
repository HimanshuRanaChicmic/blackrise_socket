using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlackRise.Identity.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedResetPasswordColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResetPasswordCode",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResetPasswordCodeExpiry",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

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
                columns: new[] { "CreatedDate", "ModifiedDate", "PasswordHash", "ResetPasswordCode", "ResetPasswordCodeExpiry" },
                values: new object[] { new DateTime(2024, 11, 3, 7, 31, 11, 404, DateTimeKind.Utc).AddTicks(3026), new DateTime(2024, 11, 3, 7, 31, 11, 404, DateTimeKind.Utc).AddTicks(3031), "AQAAAAIAAYagAAAAEM3Qs3TFwtvRkqbrYYQHqz9eK8lSzvtuRSnHtSTTa+Dihzoch1f0rXB5v3dbh1E9cg==", null, null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("3e96af2d-c6f2-4f9e-843e-ada493a79a17"),
                columns: new[] { "CreatedDate", "ModifiedDate", "PasswordHash", "ResetPasswordCode", "ResetPasswordCodeExpiry" },
                values: new object[] { new DateTime(2024, 11, 3, 7, 31, 11, 492, DateTimeKind.Utc).AddTicks(4032), new DateTime(2024, 11, 3, 7, 31, 11, 492, DateTimeKind.Utc).AddTicks(4039), "AQAAAAIAAYagAAAAEAgem6F/EBbyD/Ljjc0nrvDc8ZNUisLQHk7qEA6Pkeaafs6DOg48maNpL4HMuphFVg==", null, null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("912c3a8a-d59d-4b7d-876a-3dd93a21c461"),
                columns: new[] { "CreatedDate", "ModifiedDate", "PasswordHash", "ResetPasswordCode", "ResetPasswordCodeExpiry" },
                values: new object[] { new DateTime(2024, 11, 3, 7, 31, 11, 303, DateTimeKind.Utc).AddTicks(6913), new DateTime(2024, 11, 3, 7, 31, 11, 303, DateTimeKind.Utc).AddTicks(6916), "AQAAAAIAAYagAAAAELPI2jXdT+Gr8tOihc1UWZEa5MI4ZLKLuOYNfqMWSgUI/bN9/y33ksTNWYjPsQ7grA==", null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResetPasswordCode",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ResetPasswordCodeExpiry",
                table: "Users");

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
                columns: new[] { "CreatedDate", "ModifiedDate", "PasswordHash" },
                values: new object[] { new DateTime(2024, 10, 31, 18, 21, 39, 222, DateTimeKind.Utc).AddTicks(783), new DateTime(2024, 10, 31, 18, 21, 39, 222, DateTimeKind.Utc).AddTicks(791), "AQAAAAIAAYagAAAAEJxnv5czkxLkUdqojeB06feWEr9src2UsJDXJ4nj57aH1Y8ztskLUltH6IEkNnm9iA==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("3e96af2d-c6f2-4f9e-843e-ada493a79a17"),
                columns: new[] { "CreatedDate", "ModifiedDate", "PasswordHash" },
                values: new object[] { new DateTime(2024, 10, 31, 18, 21, 39, 312, DateTimeKind.Utc).AddTicks(5660), new DateTime(2024, 10, 31, 18, 21, 39, 312, DateTimeKind.Utc).AddTicks(5667), "AQAAAAIAAYagAAAAEGtKF4xRnbOFe4MUxOTU9XNpMNSnzu8xO3/dx/uvu7kve9JYGWB3ZQ/n9BJKOcSjPA==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("912c3a8a-d59d-4b7d-876a-3dd93a21c461"),
                columns: new[] { "CreatedDate", "ModifiedDate", "PasswordHash" },
                values: new object[] { new DateTime(2024, 10, 31, 18, 21, 39, 136, DateTimeKind.Utc).AddTicks(3995), new DateTime(2024, 10, 31, 18, 21, 39, 136, DateTimeKind.Utc).AddTicks(3998), "AQAAAAIAAYagAAAAEMUPBhu4VGA5hGbZUfC4AJ3QRvuHf3a/zkASxTQ7I9tqPRzfujIOHpLDKs1kk/BS/w==" });
        }
    }
}
