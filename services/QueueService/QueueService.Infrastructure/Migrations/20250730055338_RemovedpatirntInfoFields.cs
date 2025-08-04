using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QueueService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovedpatirntInfoFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PatientName",
                table: "QueueItems",
                newName: "PatientInfo_Name");

            migrationBuilder.RenameColumn(
                name: "PatientGender",
                table: "QueueItems",
                newName: "PatientInfo_Gender");

            migrationBuilder.RenameColumn(
                name: "PatientAge",
                table: "QueueItems",
                newName: "PatientInfo_Age");

            migrationBuilder.RenameColumn(
                name: "DoctorSpecialization",
                table: "QueueItems",
                newName: "DoctorInfo_Specialization");

            migrationBuilder.RenameColumn(
                name: "DoctorName",
                table: "QueueItems",
                newName: "DoctorInfo_Name");

            migrationBuilder.AlterColumn<string>(
                name: "PatientInfo_Name",
                table: "QueueItems",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "PatientInfo_Gender",
                table: "QueueItems",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "DoctorInfo_Specialization",
                table: "QueueItems",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "DoctorInfo_Name",
                table: "QueueItems",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PatientInfo_Name",
                table: "QueueItems",
                newName: "PatientName");

            migrationBuilder.RenameColumn(
                name: "PatientInfo_Gender",
                table: "QueueItems",
                newName: "PatientGender");

            migrationBuilder.RenameColumn(
                name: "PatientInfo_Age",
                table: "QueueItems",
                newName: "PatientAge");

            migrationBuilder.RenameColumn(
                name: "DoctorInfo_Specialization",
                table: "QueueItems",
                newName: "DoctorSpecialization");

            migrationBuilder.RenameColumn(
                name: "DoctorInfo_Name",
                table: "QueueItems",
                newName: "DoctorName");

            migrationBuilder.AlterColumn<string>(
                name: "PatientName",
                table: "QueueItems",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PatientGender",
                table: "QueueItems",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DoctorSpecialization",
                table: "QueueItems",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DoctorName",
                table: "QueueItems",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
