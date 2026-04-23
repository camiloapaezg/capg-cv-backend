using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace capg_hv_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkExperienceLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "WorkExperience",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "WorkExperience");
        }
    }
}
