using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoListApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignedToIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TodoTasks_AssignedTo",
                table: "TodoTasks",
                column: "AssignedTo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TodoTasks_AssignedTo",
                table: "TodoTasks");
        }
    }
}
