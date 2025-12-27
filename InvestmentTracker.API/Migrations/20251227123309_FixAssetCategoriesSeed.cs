using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvestmentTracker.API.Migrations
{
    /// <inheritdoc />
    public partial class FixAssetCategoriesSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO AssetCategories (Name, IsActive) VALUES
                    ('Stocks', 1),
                    ('ETFs / Index Funds', 1),
                    ('Bonds', 1),
                    ('Crypto', 1),
                    ('Real Estate', 1),
                    ('Commodities', 1),
                    ('Cash / Savings', 1),
                    ('P2P Lending', 1);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
