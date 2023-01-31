using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Data.Migrations
{
	public partial class Match_notify : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<bool>(
				name: "Notified",
				table: "MatchedUser",
				type: "bit",
				nullable: false,
				defaultValue: false);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "Notified",
				table: "MatchedUser");
		}
	}
}
