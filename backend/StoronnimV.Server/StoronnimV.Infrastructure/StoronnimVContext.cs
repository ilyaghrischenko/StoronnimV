using Microsoft.EntityFrameworkCore;
using StoronnimV.Domain.Entities;

namespace StoronnimV.Infrastructure;

/// <summary>
/// Класс, который нужен для описания БД, взаимодействий между таблицами и т.д.
/// </summary>
public class StoronnimVContext : DbContext
{
    public StoronnimVContext() { }

    public StoronnimVContext(DbContextOptions<StoronnimVContext> options)
        : base(options) { }

    public DbSet<News> NewsItems { get; set; }
    public DbSet<GroupPage> GroupPages { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<Social> Socials { get; set; }
    public DbSet<GroupSocial> GroupSocials { get; set; }
    public DbSet<Schedule> Schedules { get; set; }
    public DbSet<MusicPlatform> MusicPlatforms { get; set; }
    public DbSet<Video> Videos { get; set; }
    public DbSet<Admin> Admins { get; set; }
}