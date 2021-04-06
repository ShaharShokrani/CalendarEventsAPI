using CalendarEvents.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CalendarEvents.DataAccess
{
    public class SeedUsers
    {
        public static void DoSeedUsers(ApplicationDbContext context)
        {
            if (!context.Users.Any())
            {
                var userData = System.IO.File.ReadAllText("Data/UsersSeedData.json");
                var users = JsonConvert.DeserializeObject<List<UserModel>>(userData);
                foreach (var user in users)
                {
                    CreatePasswordHash("password", out byte[] passwordHash, out byte[] passwordSalt);

                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;
                    user.Email = user.Email.ToLower();
                    context.Users.Add(user);
                }

                context.SaveChanges();
            }
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }
}
