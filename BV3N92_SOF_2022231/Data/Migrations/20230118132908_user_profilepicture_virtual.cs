using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Data.Migrations
{
    public partial class user_profilepicture_virtual : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Pictures_ProfilePicturePictureId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ProfilePicturePictureId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProfilePicturePictureId",
                table: "AspNetUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfilePicturePictureId",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ProfilePicturePictureId",
                table: "AspNetUsers",
                column: "ProfilePicturePictureId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Pictures_ProfilePicturePictureId",
                table: "AspNetUsers",
                column: "ProfilePicturePictureId",
                principalTable: "Pictures",
                principalColumn: "PictureId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
