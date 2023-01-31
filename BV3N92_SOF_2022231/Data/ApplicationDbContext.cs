using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data
{
	public class ApplicationDbContext : IdentityDbContext
	{
		public DbSet<Picture> Pictures { get; set; }
		public DbSet<SiteUser> Users { get; set; }
		public DbSet<LikedUser> LikedUsers { get; set; }
		public DbSet<DislikedUser> DislikedUsers { get; set; }
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

			builder.Entity<Hobby>(h =>
			h.HasOne(u => u.User)
			.WithMany(h => h.Hobbies)
			.HasForeignKey(h => h.UserId)
			.OnDelete(DeleteBehavior.Cascade));

			builder.Entity<SiteUser>(u =>
			u.HasMany(p => p.Pictures)
			.WithOne(u => u.User)
			.HasForeignKey(p => p.UserId)
			.OnDelete(DeleteBehavior.Cascade));

			builder.Entity<SiteUser>(u =>
			u.HasMany(l => l.LikedUsers)
			.WithOne(u => u.LikedBy)
			.HasForeignKey(l => l.LikedById)
			.OnDelete(DeleteBehavior.Cascade));

			builder.Entity<LikedUser>(l =>
			l.HasOne(u => u.LikedBy)
			.WithMany(l => l.LikedUsers)
			.HasForeignKey(u => u.LikedById)
			.OnDelete(DeleteBehavior.Cascade));

			builder.Entity<SiteUser>(u => 
			u.HasMany(m => m.MatchedUsers)
			.WithOne(u => u.LikedBy)
			.HasForeignKey(m => m.LikedById)
			.OnDelete(DeleteBehavior.Cascade));

			builder.Entity<MatchedUser>(m =>
			m.HasOne(u => u.LikedBy)
			.WithMany(m => m.MatchedUsers)
			.HasForeignKey(u => u.LikedById)
			.OnDelete(DeleteBehavior.Cascade));

			builder.Entity<SiteUser>(u =>
			u.HasMany(l => l.DislikedUsers)
			.WithOne(u => u.DislikedBy)
			.HasForeignKey(l => l.DislikedById)
			.OnDelete(DeleteBehavior.Cascade));

			builder.Entity<DislikedUser>(l =>
			l.HasOne(u => u.DislikedBy)
			.WithMany(l => l.DislikedUsers)
			.HasForeignKey(u => u.DislikedById)
			.OnDelete(DeleteBehavior.Cascade));

			base.OnModelCreating(builder);
		}
	}
}