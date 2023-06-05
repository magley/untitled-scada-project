using USca_Server.Shared;

namespace USca_Server.Users
{
    public class UserService : IUserService
    {
        private readonly ServerDbContext _context;
        public UserService(ServerDbContext context)
        {
            _context = context;
        }

        public User? Login(LoginDTO loginCredentials)
        {
            var user = _context.Users.Where(u => u.Username == loginCredentials.Username).FirstOrDefault();
            if (user == null)
            {
                return null;
            }

            if (user.Password == loginCredentials.Password)
            {
                return user;
            }
            return null;
        }
    }
}
