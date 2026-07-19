using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoronnimV.Infrastructure.Migrations;

[DbContext(typeof(StoronnimVContext))]
[Migration("20260717233000_EnforceAdminLoginUniqueness")]
public partial class EnforceAdminLoginUniqueness : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            DO $$
            BEGIN
                IF EXISTS (
                    SELECT 1
                    FROM "Admins"
                    GROUP BY "Login"
                    HAVING COUNT(*) > 1
                ) THEN
                    RAISE EXCEPTION 'Cannot enforce Admin login uniqueness: duplicate logins exist.';
                END IF;
            END
            $$;
            """);

        migrationBuilder.CreateIndex(
            name: "IX_Admins_Login",
            table: "Admins",
            column: "Login",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Admins_Login",
            table: "Admins");
    }
}
