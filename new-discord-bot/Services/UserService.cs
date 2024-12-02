using new_discord_bot.Data;
using Microsoft.EntityFrameworkCore;
using Discord.WebSocket;
using Discord;
using Discord.Interactions;

namespace new_discord_bot.Services
{
	public class UserService
	{
		private readonly UserContext _context;

		public UserService(UserContext context)
		{
			_context = context;
		}


		public Task<User> AddUserAsync(ulong Id, bool fromGetMethod)
		{
			User user = new User()
			{
				Id = Id,
				Balance = 0,
				Spins = 25,
			};

			_context.Users.Add(user);
			_context.SaveChangesAsync();

			return _context.SaveChangesAsync().ContinueWith(_ => user);
		}

		//Adding users when you dont know if they exist
		public async Task<User> AddUserAsync(SocketUser caller)
		{
			User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == caller.Id);
			if (user != null)
			{
				return user;
			}

			user = await AddUserAsync(caller.Id, true);
			await _context.SaveChangesAsync();
			return user;
		}


		public Task<List<User>> GetUsersAsync()
		{
			return _context.Users.ToListAsync();
		}


		public async Task<User> GetUserAsync(SocketUser user)
		{
			return await GetUserAsync(user.Id);
		}

		public async Task<User> GetUserAsync(ulong Id)
		{
			User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == Id);
			if (user == null)
			{
				user = await AddUserAsync(Id, true);
			}

			return user;
		}

	}
}
