using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Data.Migrations
{
    public partial class changed_SiteUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Picture_AspNetUsers_SiteUserId",
                table: "Picture");

            migrationBuilder.DropIndex(
                name: "IX_Picture_SiteUserId",
                table: "Picture");

            migrationBuilder.DropColumn(
                name: "PhotoData",
                table: "Picture");

            migrationBuilder.DropColumn(
                name: "SiteUserId",
                table: "Picture");

            migrationBuilder.RenameColumn(
                name: "PhotoContentType",
                table: "Picture",
                newName: "PhotoUrl");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Picture",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Picture");

            migrationBuilder.RenameColumn(
                name: "PhotoUrl",
                table: "Picture",
                newName: "PhotoContentType");

            migrationBuilder.AddColumn<byte[]>(
                name: "PhotoData",
                table: "Picture",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<string>(
                name: "SiteUserId",
                table: "Picture",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Picture_SiteUserId",
                table: "Picture",
                column: "SiteUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Picture_AspNetUsers_SiteUserId",
                table: "Picture",
                column: "SiteUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
