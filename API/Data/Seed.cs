using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(UserManager<AppUser> userManager)
        {
            if (await userManager.Users.AnyAsync()) return;

            var userData= await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");

            var users= JsonSerializer.Deserialize<List<AppUser>>(userData);
            foreach (var user in users)
            {
                await userManager.CreateAsync(user, "Test123!");
            } 
        }
    }
}
