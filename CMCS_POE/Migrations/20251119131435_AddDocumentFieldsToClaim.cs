using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMCS_POE.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentFieldsToClaim : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DocumentName",
                table: "Claims",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DocumentPath",
                table: "Claims",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentName",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "DocumentPath",
                table: "Claims");
        }
    }
}
