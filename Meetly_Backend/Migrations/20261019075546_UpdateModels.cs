using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meetly_Backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Meetings_MeetingId",
                table: "Categories");

            migrationBuilder.DropTable(
                name: "MeetingUser");

            migrationBuilder.DropIndex(
                name: "IX_Categories_MeetingId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Host",
                table: "Meetings");

            migrationBuilder.DropColumn(
                name: "MeetingId",
                table: "Categories");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Meetings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "HostId",
                table: "Meetings",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "MeetingParticipants",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Review",
                table: "MeetingParticipants",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Categories",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "CategoryMeeting",
                columns: table => new
                {
                    CategoriesId = table.Column<int>(type: "int", nullable: false),
                    MeetingsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryMeeting", x => new { x.CategoriesId, x.MeetingsId });
                    table.ForeignKey(
                        name: "FK_CategoryMeeting_Categories_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryMeeting_Meetings_MeetingsId",
                        column: x => x.MeetingsId,
                        principalTable: "Meetings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_HostId",
                table: "Meetings",
                column: "HostId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Meeting_MaxAttendants",
                table: "Meetings",
                sql: "[MaxAttendants] >= 1");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingParticipants_MeetingId",
                table: "MeetingParticipants",
                column: "MeetingId");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingParticipants_UserId_MeetingId",
                table: "MeetingParticipants",
                columns: new[] { "UserId", "MeetingId" },
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_MeetingParticipant_Grade",
                table: "MeetingParticipants",
                sql: "[Grade] BETWEEN 1 AND 5");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryMeeting_MeetingsId",
                table: "CategoryMeeting",
                column: "MeetingsId");

            migrationBuilder.AddForeignKey(
                name: "FK_MeetingParticipants_AspNetUsers_UserId",
                table: "MeetingParticipants",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MeetingParticipants_Meetings_MeetingId",
                table: "MeetingParticipants",
                column: "MeetingId",
                principalTable: "Meetings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Meetings_AspNetUsers_HostId",
                table: "Meetings",
                column: "HostId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MeetingParticipants_AspNetUsers_UserId",
                table: "MeetingParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_MeetingParticipants_Meetings_MeetingId",
                table: "MeetingParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_Meetings_AspNetUsers_HostId",
                table: "Meetings");

            migrationBuilder.DropTable(
                name: "CategoryMeeting");

            migrationBuilder.DropIndex(
                name: "IX_Meetings_HostId",
                table: "Meetings");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Meeting_MaxAttendants",
                table: "Meetings");

            migrationBuilder.DropIndex(
                name: "IX_MeetingParticipants_MeetingId",
                table: "MeetingParticipants");

            migrationBuilder.DropIndex(
                name: "IX_MeetingParticipants_UserId_MeetingId",
                table: "MeetingParticipants");

            migrationBuilder.DropCheckConstraint(
                name: "CK_MeetingParticipant_Grade",
                table: "MeetingParticipants");

            migrationBuilder.DropColumn(
                name: "HostId",
                table: "Meetings");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Meetings",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "Host",
                table: "Meetings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "MeetingParticipants",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Review",
                table: "MeetingParticipants",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<int>(
                name: "MeetingId",
                table: "Categories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MeetingUser",
                columns: table => new
                {
                    MeetingsId = table.Column<int>(type: "int", nullable: false),
                    UsersId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeetingUser", x => new { x.MeetingsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_MeetingUser_AspNetUsers_UsersId",
                        column: x => x.UsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MeetingUser_Meetings_MeetingsId",
                        column: x => x.MeetingsId,
                        principalTable: "Meetings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_MeetingId",
                table: "Categories",
                column: "MeetingId");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingUser_UsersId",
                table: "MeetingUser",
                column: "UsersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Meetings_MeetingId",
                table: "Categories",
                column: "MeetingId",
                principalTable: "Meetings",
                principalColumn: "Id");
        }
    }
}
