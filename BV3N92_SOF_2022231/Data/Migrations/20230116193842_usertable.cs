using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Data.Migrations
{
	public partial class usertable : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<int>(
				name: "Age",
				table: "AspNetUsers",
				type: "int",
				nullable: true);

			migrationBuilder.AddColumn<string>(
				name: "Bio",
				table: "AspNetUsers",
				type: "nvarchar(500)",
				maxLength: 500,
				nullable: true);

			migrationBuilder.AddColumn<string>(
				name: "Discriminator",
				table: "AspNetUsers",
				type: "nvarchar(max)",
				nullable: false,
				defaultValue: "");

			migrationBuilder.AddColumn<string>(
				name: "FirstName",
				table: "AspNetUsers",
				type: "nvarchar(20)",
				maxLength: 20,
				nullable: true);

			migrationBuilder.AddColumn<int>(
				name: "Gender",
				table: "AspNetUsers",
				type: "int",
				nullable: true);

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

			migrationBuilder.CreateTable(
				name: "Picture",
				columns: table => new
				{
					PictureId = table.Column<string>(type: "nvarchar(450)", nullable: false),
					PhotoContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
					PhotoData = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
					SiteUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Picture", x => x.PictureId);
					table.ForeignKey(
						name: "FK_Picture_AspNetUsers_SiteUserId",
						column: x => x.SiteUserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id");
				});

			migrationBuilder.CreateIndex(
				name: "IX_Picture_SiteUserId",
				table: "Picture",
				column: "SiteUserId");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "Picture");

			migrationBuilder.DropColumn(
				name: "Age",
				table: "AspNetUsers");

			migrationBuilder.DropColumn(
				name: "Bio",
				table: "AspNetUsers");

			migrationBuilder.DropColumn(
				name: "Discriminator",
				table: "AspNetUsers");

			migrationBuilder.DropColumn(
				name: "FirstName",
				table: "AspNetUsers");

			migrationBuilder.DropColumn(
				name: "Gender",
				table: "AspNetUsers");

			migrationBuilder.DropColumn(
				name: "Height",
				table: "AspNetUsers");

			migrationBuilder.DropColumn(
				name: "LastName",
				table: "AspNetUsers");
		}
	}
}
