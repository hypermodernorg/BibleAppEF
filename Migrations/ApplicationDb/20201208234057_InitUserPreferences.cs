using Microsoft.EntityFrameworkCore.Migrations;

namespace BibleAppEF.Migrations.ApplicationDb
{
    public partial class InitUserPreferences : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VersionsString",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VersionsString",
                table: "AspNetUsers");
        }
    }
}
