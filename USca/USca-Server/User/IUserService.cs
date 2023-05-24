namespace USCa_Server.User
{
    public interface IUserService
    {
        public User? Login(LoginDTO loginCredentials);
    }
}
