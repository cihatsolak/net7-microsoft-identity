using Microsoft.EntityFrameworkCore.Migrations;

namespace MemberShip.Web.Migrations
{
    public partial class UdemyCourseSectionTwo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "TwoFactor",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TwoFactor",
                table: "AspNetUsers");
        }
    }
}
