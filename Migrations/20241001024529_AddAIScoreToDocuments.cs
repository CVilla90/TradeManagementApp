using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradeManagementApp.Migrations
{
    /// <inheritdoc />
    public partial class AddAIScoreToDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "AIScore",
                table: "Documents",
                type: "REAL",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AIScore",
                table: "Documents");
        }
    }
}
