using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Data.Migrations
{
	public partial class added_id_tolikes : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<string>(
				name: "LikedById",
				table: "LikedUsers",
				type: "nvarchar(450)",
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(max)");

			migrationBuilder.AddColumn<string>(
				name: "Id",
				table: "LikedUsers",
				type: "nvarchar(450)",
				nullable: false,
				defaultValue: "");

			migrationBuilder.AlterColumn<string>(
				name: "DislikedById",
				table: "DislikedUsers",
				type: "nvarchar(450)",
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(max)");

			migrationBuilder.AddColumn<string>(
				name: "Id",
				table: "DislikedUsers",
				type: "nvarchar(450)",
				nullable: false,
				defaultValue: "");

			migrationBuilder.AddPrimaryKey(
				name: "PK_LikedUsers",
				table: "LikedUsers",
				column: "Id");

			migrationBuilder.AddPrimaryKey(
				name: "PK_DislikedUsers",
				table: "DislikedUsers",
				column: "Id");

			migrationBuilder.CreateIndex(
				name: "IX_LikedUsers_LikedById",
				table: "LikedUsers",
				column: "LikedById");

			migrationBuilder.CreateIndex(
				name: "IX_DislikedUsers_DislikedById",
				table: "DislikedUsers",
				column: "DislikedById");

			migrationBuilder.AddForeignKey(
				name: "FK_DislikedUsers_AspNetUsers_DislikedById",
				table: "DislikedUsers",
				column: "DislikedById",
				principalTable: "AspNetUsers",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);

			migrationBuilder.AddForeignKey(
				name: "FK_LikedUsers_AspNetUsers_LikedById",
				table: "LikedUsers",
				column: "LikedById",
				principalTable: "AspNetUsers",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_DislikedUsers_AspNetUsers_DislikedById",
				table: "DislikedUsers");

			migrationBuilder.DropForeignKey(
				name: "FK_LikedUsers_AspNetUsers_LikedById",
				table: "LikedUsers");

			migrationBuilder.DropPrimaryKey(
				name: "PK_LikedUsers",
				table: "LikedUsers");

			migrationBuilder.DropIndex(
				name: "IX_LikedUsers_LikedById",
				table: "LikedUsers");

			migrationBuilder.DropPrimaryKey(
				name: "PK_DislikedUsers",
				table: "DislikedUsers");

			migrationBuilder.DropIndex(
				name: "IX_DislikedUsers_DislikedById",
				table: "DislikedUsers");

			migrationBuilder.DropColumn(
				name: "Id",
				table: "LikedUsers");

			migrationBuilder.DropColumn(
				name: "Id",
				table: "DislikedUsers");

			migrationBuilder.AlterColumn<string>(
				name: "LikedById",
				table: "LikedUsers",
				type: "nvarchar(max)",
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(450)");

			migrationBuilder.AlterColumn<string>(
				name: "DislikedById",
				table: "DislikedUsers",
				type: "nvarchar(max)",
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(450)");
		}
	}
}
