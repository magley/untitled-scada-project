namespace USca_DbManager.User
{
    public interface IUserService
    {
        public User? Login(LoginDTO loginCredentials);
    }
}
