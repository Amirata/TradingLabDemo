using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AuthService.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "7ac8ea68-19f7-48bc-881a-941398e7f01c", null, "Admin", "ADMIN" },
                    { "f1454956-9642-4a2a-b959-bccaaab6c82e", null, "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "1dffb982-b5e9-48dc-95d2-a17ff4916c4a", 0, "eadf5db0-503d-4a6c-b1f4-42b71fb889a8", "admin@gmail.com", true, true, null, "ADMIN@GMAIL.COM", "ADMIN", "AQAAAAIAAYagAAAAEFWUnaTYUvoZlcY+wGbcb6M4/89rvF/xnNkYqz6+I/HMpuEzyojUW/zHTO1yF8FG8w==", null, false, "7SI46PSRSCBOO3FYR422OBYHQRBRMT2S", false, "admin" },
                    { "dd714910-d4b6-46d7-a20c-47bc00c24049", 0, "d0da5874-a831-4a43-ae21-0f0005a9ffbc", "Ali@gmail.com", false, true, null, "ALI@GMAIL.COM", "ALI", "AQAAAAIAAYagAAAAEOyOmCQur0Drt34ZJmkNgsGUY71w7rljxCSx7NWU67mg+p9ytY2Oh68+vaib3jhIfQ==", null, false, "K6VJPVSUAV65GDWEYA7W5PMSFFKC2N4D", false, "ali" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "7ac8ea68-19f7-48bc-881a-941398e7f01c", "1dffb982-b5e9-48dc-95d2-a17ff4916c4a" },
                    { "f1454956-9642-4a2a-b959-bccaaab6c82e", "dd714910-d4b6-46d7-a20c-47bc00c24049" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "7ac8ea68-19f7-48bc-881a-941398e7f01c", "1dffb982-b5e9-48dc-95d2-a17ff4916c4a" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "f1454956-9642-4a2a-b959-bccaaab6c82e", "dd714910-d4b6-46d7-a20c-47bc00c24049" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7ac8ea68-19f7-48bc-881a-941398e7f01c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f1454956-9642-4a2a-b959-bccaaab6c82e");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1dffb982-b5e9-48dc-95d2-a17ff4916c4a");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dd714910-d4b6-46d7-a20c-47bc00c24049");
        }
    }
}
