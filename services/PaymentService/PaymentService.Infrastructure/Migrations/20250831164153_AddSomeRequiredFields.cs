using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PaymentService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSomeRequiredFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RegistrationFees",
                keyColumn: "Id",
                keyValue: new Guid("49467a7b-107a-4073-98c4-49aa79e147f3"));

            migrationBuilder.DeleteData(
                table: "RegistrationFees",
                keyColumn: "Id",
                keyValue: new Guid("4c475205-a593-4bd8-b079-871ec76dd8cd"));

            migrationBuilder.DeleteData(
                table: "RegistrationFees",
                keyColumn: "Id",
                keyValue: new Guid("9e22eced-e8e9-435d-9643-06ae81bafd8f"));

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DoctorId",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNumber",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PayerEmail",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PayerName",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PayerPhone",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentGatewayTransactionId",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceiptNumber",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "Department",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "InvoiceNumber",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PayerEmail",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PayerName",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PayerPhone",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PaymentGatewayTransactionId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "ReceiptNumber",
                table: "Payments");

            migrationBuilder.InsertData(
                table: "RegistrationFees",
                columns: new[] { "Id", "Amount", "CreatedAt", "Currency", "Description", "FeeType", "IsActive" },
                values: new object[,]
                {
                    { new Guid("49467a7b-107a-4073-98c4-49aa79e147f3"), 300.00m, new DateTime(2025, 8, 31, 6, 13, 8, 494, DateTimeKind.Utc).AddTicks(245), "INR", "Consultation Fee", "CONSULTATION", true },
                    { new Guid("4c475205-a593-4bd8-b079-871ec76dd8cd"), 500.00m, new DateTime(2025, 8, 31, 6, 13, 8, 494, DateTimeKind.Utc).AddTicks(236), "INR", "New Patient Registration Fee", "NEW_PATIENT", true },
                    { new Guid("9e22eced-e8e9-435d-9643-06ae81bafd8f"), 1000.00m, new DateTime(2025, 8, 31, 6, 13, 8, 494, DateTimeKind.Utc).AddTicks(242), "INR", "Emergency Registration Fee", "EMERGENCY", true }
                });
        }
    }
}
