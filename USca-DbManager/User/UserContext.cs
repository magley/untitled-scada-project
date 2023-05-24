using Microsoft.EntityFrameworkCore;

namespace USca_DbManager.User
{
    public class UserContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        private static bool _created = false;
        public UserContext(DbContextOptions<UserContext> options)
        : base(options)
        {
            if (!_created)
            {
                _created = true;
                Database.EnsureDeleted();
                Database.EnsureCreated();
            }

            Users.Add(new() { Name = "Bob", Surname = "Jones", Username = "user1", Password = "1234" });
            Users.Add(new() { Name = "Bab", Surname = "Janes", Username = "user2", Password = "1234" });
            Users.Add(new() { Name = "Bib", Surname = "Jines", Username = "user3", Password = "1234" });
            SaveChanges();
        }
    }
}
