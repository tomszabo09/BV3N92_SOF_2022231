using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Data.Migrations
{
	public partial class added_tables : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "DislikedUsers",
				columns: table => new
				{
					DislikedById = table.Column<string>(type: "nvarchar(max)", nullable: false),
					WhoDislikedId = table.Column<string>(type: "nvarchar(max)", nullable: false)
				},
				constraints: table =>
				{
				});

			migrationBuilder.CreateTable(
				name: "LikedUsers",
				columns: table => new
				{
					LikedById = table.Column<string>(type: "nvarchar(max)", nullable: false),
					WhoLikedId = table.Column<string>(type: "nvarchar(max)", nullable: false)
				},
				constraints: table =>
				{
				});
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "DislikedUsers");

			migrationBuilder.DropTable(
				name: "LikedUsers");
		}
	}
}
