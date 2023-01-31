using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Data.Migrations
{
	public partial class added_picture_db : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_AspNetUsers_Picture_ProfilePicturePictureId",
				table: "AspNetUsers");

			migrationBuilder.DropPrimaryKey(
				name: "PK_Picture",
				table: "Picture");

			migrationBuilder.RenameTable(
				name: "Picture",
				newName: "Pictures");

			migrationBuilder.AddPrimaryKey(
				name: "PK_Pictures",
				table: "Pictures",
				column: "PictureId");

			migrationBuilder.AddForeignKey(
				name: "FK_AspNetUsers_Pictures_ProfilePicturePictureId",
				table: "AspNetUsers",
				column: "ProfilePicturePictureId",
				principalTable: "Pictures",
				principalColumn: "PictureId",
				onDelete: ReferentialAction.Cascade);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_AspNetUsers_Pictures_ProfilePicturePictureId",
				table: "AspNetUsers");

			migrationBuilder.DropPrimaryKey(
				name: "PK_Pictures",
				table: "Pictures");

			migrationBuilder.RenameTable(
				name: "Pictures",
				newName: "Picture");

			migrationBuilder.AddPrimaryKey(
				name: "PK_Picture",
				table: "Picture",
				column: "PictureId");

			migrationBuilder.AddForeignKey(
				name: "FK_AspNetUsers_Picture_ProfilePicturePictureId",
				table: "AspNetUsers",
				column: "ProfilePicturePictureId",
				principalTable: "Picture",
				principalColumn: "PictureId",
				onDelete: ReferentialAction.Cascade);
		}
	}
}
