using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradingJournal.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Addonetomanyusertradingtechnicandplan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TradingTechnics_UserId",
                table: "TradingTechnics");

            migrationBuilder.DropIndex(
                name: "IX_TradingPlans_UserId",
                table: "TradingPlans");

            migrationBuilder.CreateIndex(
                name: "IX_TradingTechnics_UserId",
                table: "TradingTechnics",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TradingPlans_UserId",
                table: "TradingPlans",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TradingTechnics_UserId",
                table: "TradingTechnics");

            migrationBuilder.DropIndex(
                name: "IX_TradingPlans_UserId",
                table: "TradingPlans");

            migrationBuilder.CreateIndex(
                name: "IX_TradingTechnics_UserId",
                table: "TradingTechnics",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TradingPlans_UserId",
                table: "TradingPlans",
                column: "UserId",
                unique: true);
        }
    }
}
