using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoronnimV.Infrastructure.Migrations;

[DbContext(typeof(StoronnimVContext))]
[Migration("20260715012000_EnforceGroupPageSingleton")]
public partial class EnforceGroupPageSingleton : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            DO $$
            BEGIN
                IF (SELECT COUNT(*) FROM "GroupPages") > 1 THEN
                    RAISE EXCEPTION 'Cannot enforce GroupPage singleton: duplicate rows exist.';
                END IF;
            END
            $$;

            CREATE UNIQUE INDEX "IX_GroupPages_Singleton" ON "GroupPages" ((TRUE));
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP INDEX \"IX_GroupPages_Singleton\";");
    }
}
