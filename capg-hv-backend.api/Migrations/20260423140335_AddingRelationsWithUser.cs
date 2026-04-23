using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace capg_hv_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddingRelationsWithUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormalEducation_Users_UserId",
                table: "FormalEducation");

            migrationBuilder.DropForeignKey(
                name: "FK_Publications_Users_UserId",
                table: "Publications");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Publications",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "FormalEducation",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FormalEducation_Users_UserId",
                table: "FormalEducation",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Publications_Users_UserId",
                table: "Publications",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormalEducation_Users_UserId",
                table: "FormalEducation");

            migrationBuilder.DropForeignKey(
                name: "FK_Publications_Users_UserId",
                table: "Publications");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Publications",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "FormalEducation",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_FormalEducation_Users_UserId",
                table: "FormalEducation",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Publications_Users_UserId",
                table: "Publications",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
