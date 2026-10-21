using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPriceAndCategoryToAds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Obriši stare oglase koji nemaju Price/Category (kreisani broken handlerom)
            migrationBuilder.Sql("DELETE FROM AdStatuss WHERE AdId IN (SELECT Id FROM Ads)");
            migrationBuilder.Sql("DELETE FROM Ads");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Ads",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "CategoryEntityId",
                table: "Ads",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Ads_CategoryEntityId",
                table: "Ads",
                column: "CategoryEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ads_ProductCategories_CategoryEntityId",
                table: "Ads",
                column: "CategoryEntityId",
                principalTable: "ProductCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ads_ProductCategories_CategoryEntityId",
                table: "Ads");

            migrationBuilder.DropIndex(
                name: "IX_Ads_CategoryEntityId",
                table: "Ads");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Ads");

            migrationBuilder.DropColumn(
                name: "CategoryEntityId",
                table: "Ads");
        }
    }
}
