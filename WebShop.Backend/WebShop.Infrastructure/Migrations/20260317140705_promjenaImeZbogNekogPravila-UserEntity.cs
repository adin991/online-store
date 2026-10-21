using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class promjenaImeZbogNekogPravilaUserEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deliverys_Users_UserId",
                table: "Deliverys");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Deliverys",
                newName: "UserEntityId");

            migrationBuilder.RenameIndex(
                name: "IX_Deliverys_UserId",
                table: "Deliverys",
                newName: "IX_Deliverys_UserEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Deliverys_Users_UserEntityId",
                table: "Deliverys",
                column: "UserEntityId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deliverys_Users_UserEntityId",
                table: "Deliverys");

            migrationBuilder.RenameColumn(
                name: "UserEntityId",
                table: "Deliverys",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Deliverys_UserEntityId",
                table: "Deliverys",
                newName: "IX_Deliverys_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Deliverys_Users_UserId",
                table: "Deliverys",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
