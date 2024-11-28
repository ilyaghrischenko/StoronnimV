using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StoronnimV.Domain.Entities;

namespace StoronnimV.Data;

/// <summary>
/// Класс, который нужен для описания БД, взаимодействий между таблицами и т.д.
/// </summary>
public class StoronnimVContext : DbContext
{
    public StoronnimVContext() { }

    public StoronnimVContext(DbContextOptions<StoronnimVContext> options)
        : base(options) { }

    public virtual DbSet<News> NewsItems { get; set; }
    public virtual DbSet<GroupPage> GroupPages { get; set; }
    public virtual DbSet<Member> Members { get; set; }
    public virtual DbSet<Social> Socials { get; set; }
}