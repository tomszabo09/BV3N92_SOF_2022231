using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Data.Migrations
{
	public partial class remove_userid : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "UserId",
				table: "AspNetUsers");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<string>(
				name: "UserId",
				table: "AspNetUsers",
				type: "nvarchar(max)",
				nullable: true);
		}
	}
}
