using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlackRise.Identity.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNormalizedUserNameToMatchEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Update NormalizedUserName to match NormalizedEmail for all users
            migrationBuilder.Sql(@"
                UPDATE ""Users"" 
                SET ""NormalizedUserName"" = ""NormalizedEmail"" 
                WHERE ""NormalizedUserName"" != ""NormalizedEmail"" OR ""NormalizedUserName"" IS NULL
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // This migration cannot be safely reversed as we don't know the original values
            // The Down method is intentionally left empty
        }
    }
}
