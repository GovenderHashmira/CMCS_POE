using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMCS_POE.Migrations
{
    /// <inheritdoc />
    public partial class AddTotalPaymentToClaim : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "DocumentUploads");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "DocumentUploads");

            migrationBuilder.AddColumn<string>(
                name: "DocumentName",
                table: "DocumentUploads",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DocumentPath",
                table: "DocumentUploads",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPayment",
                table: "Claims",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentName",
                table: "DocumentUploads");

            migrationBuilder.DropColumn(
                name: "DocumentPath",
                table: "DocumentUploads");

            migrationBuilder.DropColumn(
                name: "TotalPayment",
                table: "Claims");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "DocumentUploads",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "DocumentUploads",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }
    }
}
