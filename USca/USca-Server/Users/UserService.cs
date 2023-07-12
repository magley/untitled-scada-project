using USca_Server.Shared;
using USca_Server.Util;

namespace USca_Server.Users
{
    public class UserService : IUserService
    {
        public User? Login(LoginDTO loginCredentials)
        {
            LogHelper.ServiceLog($"{GetType().Name}.{System.Reflection.MethodBase.GetCurrentMethod()?.Name}");
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
