using Microsoft.EntityFrameworkCore;
using StoronnimV.Domain.DbModels;

namespace StoronnimV.Data;

/// <summary>
/// Класс, который нужен для описания БД, взаимодействий между таблицами и т.д.
/// </summary>
public class StoronnimVContext : DbContext
{
    public StoronnimVContext()
    {
    }

    public StoronnimVContext(DbContextOptions<StoronnimVContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=StoronnimV;Username=postgres;Password=12241;Trust Server Certificate=true;");
        // "Host=localhost;Port=5432;Database=StoronnimV;Username=postgres;Password=ilya711626;Trust Server Certificate=true;");

    public virtual DbSet<News> NewsItems { get; set; }
    public virtual DbSet<GroupPage> GroupPages { get; set; }
    public virtual DbSet<Member> Members { get; set; }
    public virtual DbSet<Social> Socials { get; set; }
}