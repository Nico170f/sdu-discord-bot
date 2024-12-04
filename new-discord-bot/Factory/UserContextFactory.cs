using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using new_discord_bot.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//namespace new_discord_bot.Factory
//{
//	public class UserContextFactory : IDesignTimeDbContextFactory<UserContext>
//	{
//		public UserContext CreateDbContext(string[] args)
//		{
//			var configuration = new ConfigurationBuilder()
//				.SetBasePath(Directory.GetCurrentDirectory())
//				.AddJsonFile("appsettings.json")
//				.Build();

//			var optionsBuilder = new DbContextOptionsBuilder<UserContext>();
//			optionsBuilder.UseSqlite(configuration.GetConnectionString("DefaultConnection"));

//			return new UserContext(optionsBuilder.Options);
//		}
//	}
//}