using Microsoft.EntityFrameworkCore;
using USca_Server.Measures;
using USca_Server.Tags;
using USca_Server.Users;

namespace USca_Server.Shared
{
    public class ServerDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Measure> Measures { get; set; }
		public DbSet<Tag> Tags { get; set; }

		private static bool _created = false;
        public ServerDbContext()
        {
            if (!_created)
            {
                _created = true;
                Database.EnsureDeleted();
                Database.EnsureCreated();
            }

            if (Users != null)
            {
                Users.Add(new() { Name = "Bob", Surname = "Jones", Username = "user1", Password = "1234" });
                Users.Add(new() { Name = "Bab", Surname = "Janes", Username = "user2", Password = "1234" });
                Users.Add(new() { Name = "Bib", Surname = "Jines", Username = "user3", Password = "1234" });
            }
            SaveChanges();
        }

		protected override void OnConfiguring(DbContextOptionsBuilder options)
		{
			options.UseSqlite("Data Source=Data/app.db");
		}
	}
}
