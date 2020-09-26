using CalendarEvents.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CalendarEvents.DataAccess
{
    public interface IAuthRepository
    {
        Task<UserModel> Register(UserModel userModel, string password);
        Task<UserModel> Login(string userName, string password);
        Task<bool> IsUserExists(string userName);
    }

    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _context;
        public AuthRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserModel> Login(string userName, string password)
        {
            //var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(x => x.Username == username);
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == userName);

            if (user == null)
                return null;

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i]) return false;
                }
            }
            return true;
        }

        public async Task<UserModel> Register(UserModel userModel, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            userModel.PasswordHash = passwordHash;
            userModel.PasswordSalt = passwordSalt;

            await _context.Users.AddAsync(userModel);
            await _context.SaveChangesAsync();

            return userModel;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> IsUserExists(string userName)
        {
            if (await _context.Users.AnyAsync(x => x.Email == userName))
                return true;

            return false;
        }
    }
}
