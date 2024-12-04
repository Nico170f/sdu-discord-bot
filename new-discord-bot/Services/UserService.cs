using new_discord_bot.Data;
using Microsoft.EntityFrameworkCore;
using Discord.WebSocket;

namespace new_discord_bot.Services
{
	public class UserService
	{
		private readonly UserContext _context;

		public UserService(UserContext context)
		{
			_context = context;
		}


		public async Task RemoveAllUsersAsync()
		{
			_context.Users.RemoveRange(_context.Users);
			//_context.
			await _context.SaveChangesAsync();
		}


		public async Task<User> AddUserAsync(ulong Id, bool fromGetMethod)
		{
			User user = new User()
			{
				Id = Id
			};

			try
			{
				_context.Users.Add(user);
				await _context.SaveChangesAsync();
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}

			return user;
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


		public async Task<List<User>> GetUsersAsync()
		{
			var users = await _context.Users.ToListAsync();
			return users ?? new List<User>();
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
