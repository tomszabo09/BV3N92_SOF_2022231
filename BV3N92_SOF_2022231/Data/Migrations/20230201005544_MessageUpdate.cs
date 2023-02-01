using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Data.Migrations
{
    public partial class MessageUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Chats_ChatModelId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ChatModelId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "ChatModelId",
                table: "Messages");

            migrationBuilder.AddColumn<string>(
                name: "ChatId",
                table: "Messages",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ChatName",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatId",
                table: "Messages",
                column: "ChatId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Chats_ChatId",
                table: "Messages",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Chats_ChatId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ChatId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "ChatId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "ChatName",
                table: "Messages");

            migrationBuilder.AddColumn<string>(
                name: "ChatModelId",
                table: "Messages",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatModelId",
                table: "Messages",
                column: "ChatModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Chats_ChatModelId",
                table: "Messages",
                column: "ChatModelId",
                principalTable: "Chats",
                principalColumn: "Id");
        }
    }
}
