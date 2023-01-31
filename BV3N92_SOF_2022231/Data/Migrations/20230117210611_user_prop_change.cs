using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Data.Migrations
{
	public partial class user_prop_change : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "Height",
				table: "AspNetUsers");

			migrationBuilder.DropColumn(
				name: "LastName",
				table: "AspNetUsers");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<int>(
				name: "Height",
				table: "AspNetUsers",
				type: "int",
				nullable: true);

			migrationBuilder.AddColumn<string>(
				name: "LastName",
				table: "AspNetUsers",
				type: "nvarchar(30)",
				maxLength: 30,
				nullable: true);
		}
	}
}
