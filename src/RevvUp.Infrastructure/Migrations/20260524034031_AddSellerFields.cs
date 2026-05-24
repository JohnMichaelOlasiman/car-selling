using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RevvUp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSellerFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SellerBio",
                table: "AspNetUsers",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SellerDisplayName",
                table: "AspNetUsers",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SellerLocation",
                table: "AspNetUsers",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SellerPhoneNumber",
                table: "AspNetUsers",
                type: "TEXT",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SellerBio",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SellerDisplayName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SellerLocation",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SellerPhoneNumber",
                table: "AspNetUsers");
        }
    }
}
