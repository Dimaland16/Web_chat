using Microsoft.AspNetCore.Identity;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class UserService
    {
        private readonly WebApplication1Context _context;
        private readonly PasswordHasher<User> _passwordHasher;

        public UserService(WebApplication1Context context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
        }

        public void Register(string name, string username, string password)
        {
            var user = new User { Name = name, Username = username };
            user.PasswordHash = _passwordHasher.HashPassword(user, password);
            _context.User.Add(user);
            _context.SaveChanges();
        }

        public bool Login(string username, string password)
        {
            var user = _context.User.FirstOrDefault(u => u.Username == username);
            if (user != null && user.PasswordHash != null)
            {
                var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
                return result == PasswordVerificationResult.Success;
            }
            return false;
        }

        public bool UserExists(string username)
        {
            return _context.User.Any(u => u.Username == username);
        }
    }
}
