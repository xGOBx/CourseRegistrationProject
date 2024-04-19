using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GavenPearl_P1.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRegisteredCourses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RegisteredCourses",
                columns: table => new
                {
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    // Define other columns for the RegisteredCourses table
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegisteredCourses", x => x.CourseId);
                    table.ForeignKey(
                        name: "FK_RegisteredCourses_Course_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Course",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegisteredCourses");
        }
    }
}