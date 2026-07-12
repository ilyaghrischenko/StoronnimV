using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoronnimV.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addedPropertyVideoTypeToVideoEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Videos",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Videos");
        }
    }
}
