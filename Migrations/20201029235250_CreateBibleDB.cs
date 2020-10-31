using Microsoft.EntityFrameworkCore.Migrations;
using MySql.Data.EntityFrameworkCore.Metadata;

namespace BibleAppEF.Migrations
{
    public partial class CreateBibleDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bibles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Version = table.Column<string>(type: "varchar(40)", nullable: false),
                    BookChapterVerse = table.Column<string>(type: "varchar(100)", nullable: true),
                    Book = table.Column<string>(type: "varchar(40)", nullable: true),
                    Chapter = table.Column<string>(type: "varchar(40)", nullable: false),
                    Verse = table.Column<string>(type: "varchar(40)", nullable: false),
                    BibleText = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bibles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Registers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Source = table.Column<string>(type: "varchar(200)", nullable: true),
                    Name = table.Column<string>(type: "varchar(100)", nullable: false),
                    FileType = table.Column<string>(type: "varchar(200)", nullable: true),
                    Copyright = table.Column<string>(type: "varchar(200)", nullable: true),
                    Abbreviation = table.Column<string>(type: "varchar(40)", nullable: false),
                    Language = table.Column<string>(type: "varchar(200)", nullable: true),
                    Note = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Registers", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bibles");

            migrationBuilder.DropTable(
                name: "Registers");
        }
    }
}
