namespace USca_Server.Users
{
    public interface IUserService
    {
        public User? Login(LoginDTO loginCredentials);
    }
}
