using USca_Server.Shared;

namespace USca_Server.Users
{
    public class UserService : IUserService
    {
        public User? Login(LoginDTO loginCredentials)
        {
            using (var db = new ServerDbContext())
            {
                var user = db.Users.Where(u => u.Username == loginCredentials.Username).FirstOrDefault();
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
}
