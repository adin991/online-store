using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebShop.Infrastructure.Migrations
{
    public partial class SeedProductCategories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Brisanje u ispravnom redoslijedu FK zavisnosti
            migrationBuilder.Sql("DELETE FROM AdStatuss");
            migrationBuilder.Sql("DELETE FROM SCItems");
            migrationBuilder.Sql("DELETE FROM Comments");
            migrationBuilder.Sql("DELETE FROM Ads");
            migrationBuilder.Sql("DELETE FROM Products");
            migrationBuilder.Sql("DELETE FROM ProductCategories");

            // Resetuj identity counter da kategorije krénu od 1
            migrationBuilder.Sql("DBCC CHECKIDENT ('ProductCategories', RESEED, 0)");

            // Ubaci 8 kategorija
            migrationBuilder.Sql(@"
                INSERT INTO ProductCategories (Name, IsEnabled, IsDeleted, CreatedAtUtc)
                VALUES
                ('Electronics', 1, 0, GETUTCDATE()),
                ('Cars',        1, 0, GETUTCDATE()),
                ('Fashion',     1, 0, GETUTCDATE()),
                ('Home',        1, 0, GETUTCDATE()),
                ('Gaming',      1, 0, GETUTCDATE()),
                ('Books',       1, 0, GETUTCDATE()),
                ('Sports',      1, 0, GETUTCDATE()),
                ('Beauty',      1, 0, GETUTCDATE())
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM ProductCategories");
        }
    }
}
