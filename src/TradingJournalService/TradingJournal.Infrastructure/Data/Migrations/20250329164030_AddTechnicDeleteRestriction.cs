using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradingJournal.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTechnicDeleteRestriction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlansTechnics_TradingTechnics_TechnicsId",
                table: "PlansTechnics");

            migrationBuilder.AddForeignKey(
                name: "FK_PlansTechnics_TradingTechnics_TechnicsId",
                table: "PlansTechnics",
                column: "TechnicsId",
                principalTable: "TradingTechnics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlansTechnics_TradingTechnics_TechnicsId",
                table: "PlansTechnics");

            migrationBuilder.AddForeignKey(
                name: "FK_PlansTechnics_TradingTechnics_TechnicsId",
                table: "PlansTechnics",
                column: "TechnicsId",
                principalTable: "TradingTechnics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
