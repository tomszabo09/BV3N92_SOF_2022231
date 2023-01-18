using Backend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data
{
	public class ApplicationDbContext : IdentityDbContext
	{
		public DbSet<Picture> Pictures { get; set; }
		public DbSet<SiteUser> Users { get; set; }
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.Entity<Picture>(p =>
			p.HasOne(u => u.User)
			.WithMany(p => p.Pictures)
			.HasForeignKey(p => p.UserId)
			.OnDelete(DeleteBehavior.Cascade));

			base.OnModelCreating(builder);
		}
	}
}