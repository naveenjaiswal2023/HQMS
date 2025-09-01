using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PaymentService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSomeMoreRequiredField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RegistrationFees",
                keyColumn: "Id",
                keyValue: new Guid("28db4361-5fe0-48b5-b63c-fb87962ebc58"));

            migrationBuilder.DeleteData(
                table: "RegistrationFees",
                keyColumn: "Id",
                keyValue: new Guid("ca05aa52-74ff-4039-b322-02243eded4ae"));

            migrationBuilder.DeleteData(
                table: "RegistrationFees",
                keyColumn: "Id",
                keyValue: new Guid("fee498b8-bf80-48e9-8bc8-ce3e1af5d569"));

            migrationBuilder.InsertData(
                table: "RegistrationFees",
                columns: new[] { "Id", "Amount", "CreatedAt", "Currency", "Description", "FeeType", "IsActive" },
                values: new object[,]
                {
                    { new Guid("217dba14-e74f-4994-a42d-9162deb8f137"), 500.00m, new DateTime(2025, 8, 31, 16, 44, 12, 399, DateTimeKind.Utc).AddTicks(9619), "INR", "New Patient Registration Fee", "NEW_PATIENT", true },
                    { new Guid("3adbf1d4-b54f-4ef9-87d3-c827fa940a7a"), 1000.00m, new DateTime(2025, 8, 31, 16, 44, 12, 399, DateTimeKind.Utc).AddTicks(9624), "INR", "Emergency Registration Fee", "EMERGENCY", true },
                    { new Guid("659237f0-4ec3-4f5c-b47c-f426f6016b32"), 300.00m, new DateTime(2025, 8, 31, 16, 44, 12, 399, DateTimeKind.Utc).AddTicks(9627), "INR", "Consultation Fee", "CONSULTATION", true }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RegistrationFees",
                keyColumn: "Id",
                keyValue: new Guid("217dba14-e74f-4994-a42d-9162deb8f137"));

            migrationBuilder.DeleteData(
                table: "RegistrationFees",
                keyColumn: "Id",
                keyValue: new Guid("3adbf1d4-b54f-4ef9-87d3-c827fa940a7a"));

            migrationBuilder.DeleteData(
                table: "RegistrationFees",
                keyColumn: "Id",
                keyValue: new Guid("659237f0-4ec3-4f5c-b47c-f426f6016b32"));

            migrationBuilder.InsertData(
                table: "RegistrationFees",
                columns: new[] { "Id", "Amount", "CreatedAt", "Currency", "Description", "FeeType", "IsActive" },
                values: new object[,]
                {
                    { new Guid("28db4361-5fe0-48b5-b63c-fb87962ebc58"), 500.00m, new DateTime(2025, 8, 31, 16, 41, 49, 741, DateTimeKind.Utc).AddTicks(9686), "INR", "New Patient Registration Fee", "NEW_PATIENT", true },
                    { new Guid("ca05aa52-74ff-4039-b322-02243eded4ae"), 300.00m, new DateTime(2025, 8, 31, 16, 41, 49, 741, DateTimeKind.Utc).AddTicks(9768), "INR", "Consultation Fee", "CONSULTATION", true },
                    { new Guid("fee498b8-bf80-48e9-8bc8-ce3e1af5d569"), 1000.00m, new DateTime(2025, 8, 31, 16, 41, 49, 741, DateTimeKind.Utc).AddTicks(9691), "INR", "Emergency Registration Fee", "EMERGENCY", true }
                });
        }
    }
}
