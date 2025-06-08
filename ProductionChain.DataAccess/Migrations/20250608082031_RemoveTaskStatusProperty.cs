using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductionChain.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTaskStatusProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProgressStatus",
                table: "AssemblyProductionTasks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProgressStatus",
                table: "AssemblyProductionTasks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
