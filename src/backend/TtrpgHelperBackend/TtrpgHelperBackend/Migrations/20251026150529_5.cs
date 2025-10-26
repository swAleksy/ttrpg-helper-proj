using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TtrpgHelperBackend.Migrations
{
    /// <inheritdoc />
    public partial class _5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Backgrounds",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { 1, "Dawno dawno temu w dupe", "" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Backgrounds",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
