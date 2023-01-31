﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Data.Migrations
{
	public partial class removed_connecion : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_Pictures_AspNetUsers_UserId",
				table: "Pictures");

			migrationBuilder.DropIndex(
				name: "IX_Pictures_UserId",
				table: "Pictures");

			migrationBuilder.AlterColumn<string>(
				name: "UserId",
				table: "Pictures",
				type: "nvarchar(max)",
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(450)");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<string>(
				name: "UserId",
				table: "Pictures",
				type: "nvarchar(450)",
				nullable: false,
				oldClrType: typeof(string),
				oldType: "nvarchar(max)");

			migrationBuilder.CreateIndex(
				name: "IX_Pictures_UserId",
				table: "Pictures",
				column: "UserId");

			migrationBuilder.AddForeignKey(
				name: "FK_Pictures_AspNetUsers_UserId",
				table: "Pictures",
				column: "UserId",
				principalTable: "AspNetUsers",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		}
	}
}
