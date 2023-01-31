﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Data.Migrations
{
    public partial class chatmodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChatModelId",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Chats",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MessageModel",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChatModelId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageModel_Chats_ChatModelId",
                        column: x => x.ChatModelId,
                        principalTable: "Chats",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ChatModelId",
                table: "AspNetUsers",
                column: "ChatModelId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageModel_ChatModelId",
                table: "MessageModel",
                column: "ChatModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Chats_ChatModelId",
                table: "AspNetUsers",
                column: "ChatModelId",
                principalTable: "Chats",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Chats_ChatModelId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "MessageModel");

            migrationBuilder.DropTable(
                name: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ChatModelId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ChatModelId",
                table: "AspNetUsers");
        }
    }
}
