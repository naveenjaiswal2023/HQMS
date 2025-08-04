using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QueueService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovedpatirntInfoFields1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoctorInfo_Name",
                table: "QueueItems");

            migrationBuilder.DropColumn(
                name: "DoctorInfo_Specialization",
                table: "QueueItems");

            migrationBuilder.DropColumn(
                name: "PatientInfo_Age",
                table: "QueueItems");

            migrationBuilder.DropColumn(
                name: "PatientInfo_Gender",
                table: "QueueItems");

            migrationBuilder.DropColumn(
                name: "PatientInfo_Name",
                table: "QueueItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DoctorInfo_Name",
                table: "QueueItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DoctorInfo_Specialization",
                table: "QueueItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PatientInfo_Age",
                table: "QueueItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PatientInfo_Gender",
                table: "QueueItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PatientInfo_Name",
                table: "QueueItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
