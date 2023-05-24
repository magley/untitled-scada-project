namespace USca_DbManager.User
{
    public class UserService : IUserService
    {
        private readonly UserContext _context;
        public UserService(UserContext context)
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
