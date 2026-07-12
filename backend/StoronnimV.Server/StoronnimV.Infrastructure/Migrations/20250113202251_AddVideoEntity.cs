using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace StoronnimV.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVideoEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Video",
                table: "NewsItems");

            migrationBuilder.AddColumn<long>(
                name: "VideoId",
                table: "NewsItems",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Videos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Url = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Videos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NewsItems_VideoId",
                table: "NewsItems",
                column: "VideoId");

            migrationBuilder.AddForeignKey(
                name: "FK_NewsItems_Videos_VideoId",
                table: "NewsItems",
                column: "VideoId",
                principalTable: "Videos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NewsItems_Videos_VideoId",
                table: "NewsItems");

            migrationBuilder.DropTable(
                name: "Videos");

            migrationBuilder.DropIndex(
                name: "IX_NewsItems_VideoId",
                table: "NewsItems");

            migrationBuilder.DropColumn(
                name: "VideoId",
                table: "NewsItems");

            migrationBuilder.AddColumn<string>(
                name: "Video",
                table: "NewsItems",
                type: "text",
                nullable: true);
        }
    }
}
