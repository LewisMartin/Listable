using Microsoft.EntityFrameworkCore.Migrations;

namespace Listable.UserMicroservice.Migrations
{
    public partial class MakeSubjectIdAlternateKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SubjectId",
                table: "Users",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Users_SubjectId",
                table: "Users",
                column: "SubjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Users_SubjectId",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "SubjectId",
                table: "Users",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
