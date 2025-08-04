using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QueueService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSkippedAndCancelledAtFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledAt",
                table: "QueueItems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DoctorName",
                table: "QueueItems",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DoctorSpecialization",
                table: "QueueItems",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PatientAge",
                table: "QueueItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PatientGender",
                table: "QueueItems",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PatientName",
                table: "QueueItems",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "SkippedAt",
                table: "QueueItems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_QueueItems_CancelledAt",
                table: "QueueItems",
                column: "CancelledAt");

            migrationBuilder.CreateIndex(
                name: "IX_QueueItems_SkippedAt",
                table: "QueueItems",
                column: "SkippedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_QueueItems_CancelledAt",
                table: "QueueItems");

            migrationBuilder.DropIndex(
                name: "IX_QueueItems_SkippedAt",
                table: "QueueItems");

            migrationBuilder.DropColumn(
                name: "CancelledAt",
                table: "QueueItems");

            migrationBuilder.DropColumn(
                name: "DoctorName",
                table: "QueueItems");

            migrationBuilder.DropColumn(
                name: "DoctorSpecialization",
                table: "QueueItems");

            migrationBuilder.DropColumn(
                name: "PatientAge",
                table: "QueueItems");

            migrationBuilder.DropColumn(
                name: "PatientGender",
                table: "QueueItems");

            migrationBuilder.DropColumn(
                name: "PatientName",
                table: "QueueItems");

            migrationBuilder.DropColumn(
                name: "SkippedAt",
                table: "QueueItems");
        }
    }
}
