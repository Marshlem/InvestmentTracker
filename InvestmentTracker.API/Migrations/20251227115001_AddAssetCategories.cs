using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvestmentTracker.API.Migrations
{
    /// <inheritdoc />
    public partial class AddAssetCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetCategories", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Assets",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(@"
                INSERT INTO AssetCategories (Name, IsActive)
                SELECT DISTINCT Category, 1 FROM Assets;
            ");

            migrationBuilder.Sql(@"
                UPDATE Assets a
                JOIN AssetCategories c ON a.Category = c.Name
                SET a.CategoryId = c.Id;
            ");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Assets",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assets_CategoryId",
                table: "Assets",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_AssetCategories_CategoryId",
                table: "Assets",
                column: "CategoryId",
                principalTable: "AssetCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Assets");

            migrationBuilder.InsertData(
                table: "AssetCategories",
                columns: ["Name", "IsActive"],
                values: new object[,]
                {
                    { "Stocks", true },
                    { "ETFs / Index Funds", true },
                    { "Bonds", true },
                    { "Crypto", true },
                    { "Real Estate", true },
                    { "Commodities", true },
                    { "Cash / Savings", true },
                    { "P2P Lending", true }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_AssetCategories_CategoryId",
                table: "Assets");

            migrationBuilder.DropTable(
                name: "AssetCategories");

            migrationBuilder.DropIndex(
                name: "IX_Assets_CategoryId",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Assets");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Assets",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
