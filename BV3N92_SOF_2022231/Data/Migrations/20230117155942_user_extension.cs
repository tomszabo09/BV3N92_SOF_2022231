using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Data.Migrations
{
	public partial class user_extension : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<int>(
				name: "Orientation",
				table: "AspNetUsers",
				type: "int",
				nullable: true);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "Orientation",
				table: "AspNetUsers");
		}
	}
}
