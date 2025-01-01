using API.Dto;
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
        public static async Task SeedUsers(UserManager<AppUser> userManager
            , RoleManager<AppRole> roleManager)
        {
            if (await userManager.Users.AnyAsync()) return;

            var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");

            var roles = new List<AppRole>
            {
                new AppRole { Name = "Member" },
                new AppRole { Name = "Admin" },
                new AppRole { Name = "Moderator" }
            };

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }


            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);
            foreach (var user in users)
            {
                user.UserName = user.UserName.ToLower();
                await userManager.CreateAsync(user, "Test123!");
                await userManager.AddToRoleAsync(user, "Member");
            }

            var admin = new AppUser
            {
                UserName = "admin"
            };

            await userManager.CreateAsync(admin, "Test123!");
            await userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" });
        }


        public static IList<City> SeedCities()
        {
            var cities = new List<City>
           {
              new City { Id= 1 , Name = "Bagerhat" },
              new City { Id= 2 , Name = "Bandarban" },
              new City { Id= 3 , Name = "Barguna" },
              new City { Id= 4 , Name = "Barisal" },
              new City { Id= 5 , Name = "Bhola" },
              new City { Id= 6 , Name = "Bogura" },
              new City { Id= 7 , Name = "Brahmanbaria" },
              new City { Id= 8 , Name = "Chandpur" },
              new City { Id= 9 , Name = "Chattogram" },
              new City { Id= 10 , Name = "Chuadanga" },
              new City { Id= 11 , Name = "Cox's Bazar" },
              new City { Id= 12 , Name = "Cumilla" },
              new City { Id= 13 , Name = "Dhaka" },
              new City { Id= 14 , Name = "Dinajpur" },
              new City { Id= 15 , Name = "Faridpur" },
              new City { Id= 16 , Name = "Feni" },
              new City { Id= 17 , Name = "Gaibandha" },
              new City { Id= 18 , Name = "Gazipur" },
              new City { Id= 19 , Name = "Gopalganj" },
              new City { Id= 20 , Name = "Habiganj" },
              new City { Id= 21 , Name = "Jamalpur" },
              new City { Id= 22 , Name = "Jashore" },
              new City { Id= 23 , Name = "Jhalokati" },
              new City { Id= 24 , Name = "Jhenaidah" },
              new City { Id= 25 , Name = "Joypurhat" },
              new City { Id= 26 , Name = "Khagrachhari" },
              new City { Id= 27 , Name = "Khulna" },
              new City { Id= 28 , Name = "Kishoreganj" },
              new City { Id= 29 , Name = "Kurigram" },
              new City { Id= 30 , Name = "Kushtia" },
              new City { Id= 31 , Name = "Lakshmipur" },
              new City { Id= 32 , Name = "Lalmonirhat" },
              new City { Id= 33 , Name = "Madaripur" },
              new City { Id= 34 , Name = "Magura" },
              new City { Id= 35 , Name = "Manikganj" },
              new City { Id= 36 , Name = "Meherpur" },
              new City { Id= 37 , Name = "Moulvibazar" },
              new City { Id= 38 , Name = "Munshiganj" },
              new City { Id= 39 , Name = "Mymensingh" },
              new City { Id= 40 , Name = "Naogaon" },
              new City { Id= 41 , Name = "Narail" },
              new City { Id= 42 , Name = "Narayanganj" },
              new City { Id= 43 , Name = "Narsingdi" },
              new City { Id= 44 , Name = "Natore" },
              new City { Id= 45 , Name = "Netrokona" },
              new City { Id= 46 , Name = "Nilphamari" },
              new City { Id= 47 , Name = "Noakhali" },
              new City { Id= 48 , Name = "Pabna" },
              new City { Id= 49 , Name = "Panchagarh" },
              new City { Id= 50 , Name = "Patuakhali" },
              new City { Id= 51 , Name = "Pirojpur" },
              new City { Id= 52 , Name = "Rajbari" },
              new City { Id= 53 , Name = "Rajshahi" },
              new City { Id= 54 , Name = "Rangamati" },
              new City { Id= 55 , Name = "Rangpur" },
              new City { Id= 56 , Name = "Satkhira" },
              new City { Id= 57 , Name = "Shariatpur" },
              new City { Id= 58 , Name = "Sherpur" },
              new City { Id= 59 , Name = "Sirajganj" },
              new City { Id= 60 , Name = "Sunamganj" },
              new City { Id= 61 , Name = "Sylhet" },
              new City { Id= 62 , Name = "Tangail" },
              new City { Id= 63 , Name = "Thakurgaon" },
              new City { Id= 64 , Name = "Chapai Nawabganj" }
           };

            return cities;
        }
    }
}
