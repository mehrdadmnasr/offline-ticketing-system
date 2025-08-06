using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OfflineTicketingSystemAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeedTickets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Tickets",
                columns: new[] { "Id", "AssignedToUserId", "CreatedAt", "CreatedByUserId", "Description", "Priority", "Status", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("2715a263-b102-4095-b024-9de11732d995"), new Guid("eeba8aae-f6b8-46c9-99cc-05446790868f"), new DateTime(2025, 8, 6, 4, 35, 7, 459, DateTimeKind.Utc).AddTicks(8203), new Guid("2f6f2733-0745-4fe7-9291-91d6c3bc8e39"), "A new feature is required for the user dashboard.", 1, 1, "Add new feature request", new DateTime(2025, 8, 6, 5, 35, 7, 459, DateTimeKind.Utc).AddTicks(8218) },
                    { new Guid("6e5ac745-171a-48c9-a8ff-7af416ace6da"), null, new DateTime(2025, 8, 6, 5, 35, 7, 459, DateTimeKind.Utc).AddTicks(8192), new Guid("2f6f2733-0745-4fe7-9291-91d6c3bc8e39"), "The application is unable to connect to the production database.", 2, 0, "Fix database connection issue", new DateTime(2025, 8, 6, 5, 35, 7, 459, DateTimeKind.Utc).AddTicks(8194) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: new Guid("2715a263-b102-4095-b024-9de11732d995"));

            migrationBuilder.DeleteData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: new Guid("6e5ac745-171a-48c9-a8ff-7af416ace6da"));
        }
    }
}
