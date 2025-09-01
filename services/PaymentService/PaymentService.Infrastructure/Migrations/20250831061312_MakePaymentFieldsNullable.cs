using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PaymentService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakePaymentFieldsNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RegistrationFees",
                keyColumn: "Id",
                keyValue: new Guid("579ef168-c2c9-4369-9696-d735df983f7d"));

            migrationBuilder.DeleteData(
                table: "RegistrationFees",
                keyColumn: "Id",
                keyValue: new Guid("aeda5cec-b626-4736-9eae-697458845a3a"));

            migrationBuilder.DeleteData(
                table: "RegistrationFees",
                keyColumn: "Id",
                keyValue: new Guid("b3738cb4-e97f-4777-af71-2a610951755f"));

            migrationBuilder.AlterColumn<string>(
                name: "PaymentGatewayResponse",
                table: "Payments",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AlterColumn<string>(
                name: "FailureReason",
                table: "Payments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<string>(
                name: "PaymentGatewayResponse",
                table: "Payments",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FailureReason",
                table: "Payments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "RegistrationFees",
                columns: new[] { "Id", "Amount", "CreatedAt", "Currency", "Description", "FeeType", "IsActive" },
                values: new object[,]
                {
                    { new Guid("579ef168-c2c9-4369-9696-d735df983f7d"), 1000.00m, new DateTime(2025, 8, 30, 14, 24, 3, 591, DateTimeKind.Utc).AddTicks(779), "INR", "Emergency Registration Fee", "EMERGENCY", true },
                    { new Guid("aeda5cec-b626-4736-9eae-697458845a3a"), 500.00m, new DateTime(2025, 8, 30, 14, 24, 3, 591, DateTimeKind.Utc).AddTicks(773), "INR", "New Patient Registration Fee", "NEW_PATIENT", true },
                    { new Guid("b3738cb4-e97f-4777-af71-2a610951755f"), 300.00m, new DateTime(2025, 8, 30, 14, 24, 3, 591, DateTimeKind.Utc).AddTicks(781), "INR", "Consultation Fee", "CONSULTATION", true }
                });
        }
    }
}
