using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvestmentTracker.API.Migrations
{
    /// <inheritdoc />
    public partial class ConvertAssetStatusToEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StatusInt",
                table: "Assets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(@"
                UPDATE Assets
                SET StatusInt =
                    CASE
                        WHEN Status = 'Active' THEN 0
                        WHEN Status = 'Inactive' THEN 1
                        ELSE 0
                    END;
            ");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Assets");

            migrationBuilder.RenameColumn(
                name: "StatusInt",
                table: "Assets",
                newName: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Assets",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
