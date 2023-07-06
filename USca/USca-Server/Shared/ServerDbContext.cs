using Microsoft.EntityFrameworkCore;
using USca_Server.Alarms;
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
        public DbSet<Alarm> Alarms { get; set; }
        public DbSet<AlarmLog> AlarmLogs { get; set; }

		private static bool _created = false;
        private static readonly object _lock = new();
        public ServerDbContext()
        {
            lock (_lock)
            {
                if (!_created)
                {
                    _created = true;
                    Database.EnsureDeleted();
                    Database.EnsureCreated();
                    LoadInitialData();
                }
            }
        }

        private void LoadInitialData()
        {
            Users.Add(new() { Name = "Bob", Surname = "Jones", Username = "user1", Password = "1234" });
            Users.Add(new() { Name = "Bab", Surname = "Janes", Username = "user2", Password = "1234" });
            Users.Add(new() { Name = "Bib", Surname = "Jines", Username = "user3", Password = "1234" });

            Tag tag1 = new()
            {
                Address = 1,
                Name = "Tank01",
                Type = TagType.Analog,
                Unit = "litre",
                Min = 0,
                Max = 10,
            };
            Tags.Add(tag1);

            Tag tag2 = new() 
            {
                Address = 2,
                Name = "Tank01_ValveIn",
                Type = TagType.Analog,
                Unit = "litre",
                Min = 0,
                Max = 1,
            };
            Tags.Add(tag2);

            Tag tag3 = new()
            { 
                Address = 3,
                Name = "Tank01_ValveIn_Reserve",
                Type = TagType.Analog,
                Unit = "litre",
                Min = 0,
                Max = 1,
            };
            Tags.Add(tag3);

            Tag tag4 = new() 
            { 
                Address = 4,
                Name = "Tank01_ValveOut",
                Type = TagType.Analog,
                Unit = "litre",
                Min = 0,
                Max = 1,
            };
            Tags.Add(tag4);

            Alarm a1 = new()
            {
                ThresholdType = AlarmThresholdType.ABOVE,
                Priority = AlarmPriority.HIGH,
                Threshold = 9,
                Tag = tag1,
            };
            Alarms.Add(a1);

            Alarm a2 = new()
            {
                ThresholdType = AlarmThresholdType.BELOW,
                Priority = AlarmPriority.MEDIUM,
                Threshold = 4.6,
                Tag = tag1,
            };
            Alarms.Add(a2);

            Alarm a3 = new()
            {
                ThresholdType = AlarmThresholdType.ABOVE,
                Priority = AlarmPriority.LOW,
                Threshold = 0.0005,
                Tag = tag2,
            };
            Alarms.Add(a3);

            Tags.Add(new()
            {
                Address = 2,
                Name = "Tank01_ValveIn_Open",
                Type = TagType.Digital,
                Mode = TagMode.Output,
                Value = 1,
            });
            Tags.Add(new()
            {
                Address = 3,
                Name = "Tank01_ValveIn_Reserve_Open",
                Type = TagType.Digital,
                Mode = TagMode.Output,
                Value = 0,
            });
            Tags.Add(new()
            {
                Address = 4,
                Name = "Tank01_ValveOut_Open",
                Type = TagType.Digital,
                Mode = TagMode.Output,
                Value = 1,
            });

            SaveChanges();
        }

		protected override void OnConfiguring(DbContextOptionsBuilder options)
		{
			options.UseSqlite("Data Source=Data/app.db");
		}
	}
}
