using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TradingJournal.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Createtables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TradingPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FromTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    ToTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    SelectedDays = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradingPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TradingPlans_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TradingTechnics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradingTechnics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TradingTechnics_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Trades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Symbol = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    PositionType = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Volume = table.Column<double>(type: "double precision", nullable: false),
                    EntryPrice = table.Column<double>(type: "double precision", nullable: false),
                    ClosePrice = table.Column<double>(type: "double precision", nullable: false),
                    StopLossPrice = table.Column<double>(type: "double precision", nullable: false),
                    EntryDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CloseDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Commission = table.Column<double>(type: "double precision", nullable: false),
                    Swap = table.Column<double>(type: "double precision", nullable: false),
                    Pips = table.Column<double>(type: "double precision", nullable: false),
                    NetProfit = table.Column<double>(type: "double precision", nullable: false),
                    GrossProfit = table.Column<double>(type: "double precision", nullable: false),
                    Balance = table.Column<double>(type: "double precision", nullable: false),
                    TradingPlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trades_TradingPlans_TradingPlanId",
                        column: x => x.TradingPlanId,
                        principalTable: "TradingPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlansTechnics",
                columns: table => new
                {
                    TechnicsId = table.Column<Guid>(type: "uuid", nullable: false),
                    TradingPlansId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlansTechnics", x => new { x.TechnicsId, x.TradingPlansId });
                    table.ForeignKey(
                        name: "FK_PlansTechnics_TradingPlans_TradingPlansId",
                        column: x => x.TradingPlansId,
                        principalTable: "TradingPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlansTechnics_TradingTechnics_TechnicsId",
                        column: x => x.TechnicsId,
                        principalTable: "TradingTechnics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TradingTechnicImages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Path = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TradingTechnicId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradingTechnicImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TradingTechnicImages_TradingTechnics_TradingTechnicId",
                        column: x => x.TradingTechnicId,
                        principalTable: "TradingTechnics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlansTechnics_TradingPlansId",
                table: "PlansTechnics",
                column: "TradingPlansId");

            migrationBuilder.CreateIndex(
                name: "IX_Trades_TradingPlanId",
                table: "Trades",
                column: "TradingPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_TradingPlans_UserId",
                table: "TradingPlans",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TradingTechnicImages_TradingTechnicId",
                table: "TradingTechnicImages",
                column: "TradingTechnicId");

            migrationBuilder.CreateIndex(
                name: "IX_TradingTechnics_UserId",
                table: "TradingTechnics",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlansTechnics");

            migrationBuilder.DropTable(
                name: "Trades");

            migrationBuilder.DropTable(
                name: "TradingTechnicImages");

            migrationBuilder.DropTable(
                name: "TradingPlans");

            migrationBuilder.DropTable(
                name: "TradingTechnics");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
