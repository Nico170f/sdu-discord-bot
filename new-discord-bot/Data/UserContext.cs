using Microsoft.EntityFrameworkCore;


namespace new_discord_bot.Data
{
	public class UserContext : DbContext
	{
		public DbSet<User> Users { get; set; }

		public UserContext(DbContextOptions<UserContext> options) : base(options) { }

		protected override void OnConfiguring(DbContextOptionsBuilder options)
		{
			options.UseSqlite("Data Source=users.db");  // Use your preferred database path or connection string
		}

		//protected override void OnConfiguring(DbContextOptionsBuilder options)
		//{
		//	if (!options.IsConfigured)
		//	{
		//		options.UseSqlite("Data Source=users.db");
		//	}
		//}
	}
}
