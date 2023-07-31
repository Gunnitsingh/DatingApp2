using API.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUser(DataContext context)
        {
            if (await context.Users.AnyAsync()) return;

            var userData = await File.ReadAllTextAsync("Data/UserSeedDara.json");

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

            foreach ( var user in users )
            {
                using var hMac = new HMACSHA512();
                user.UserName = user.UserName.ToLower();
                user.PaswordHash = hMac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
                user.PaswordSalt = hMac.Key;

                context.Users.Add(user);
            }

            await context.SaveChangesAsync();
        }
    }
}
