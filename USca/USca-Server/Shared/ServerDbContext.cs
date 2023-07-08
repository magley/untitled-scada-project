using Microsoft.EntityFrameworkCore;
using USca_Server.Alarms;
using USca_Server.Measures;
using USca_Server.TagLogs;
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
        public DbSet<TagLog> TagLogs { get; set; }

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

            Tag milkTank01 = new()
            {
                Address = 1, Name = "Milk Tank 01",
                Type = TagType.Analog, Unit = "litre", Min = 0, Max = 10000
            };
            Tag milkTank02 = new()
            {
                Address = 2, Name = "Milk Tank 02",
                Type = TagType.Analog, Unit = "litre", Min = 0, Max = 10000
            };
            Tag milkTank03 = new()
            {
                Address = 3, Name = "Milk Tank 03",
                Type = TagType.Analog, Unit = "litre", Min = 0, Max = 10000
            };
            Tag milkTank01Valve = new()
            {
                Address = 4, Name = "Milk Tank 01 Valve",
                Mode = TagMode.Output, Value = 1,
            };
            Tag milkTank02Valve = new()
            {
                Address = 5, Name = "Milk Tank 02 Valve",
                Mode = TagMode.Output, Value = 1,
            };
            Tag milkTank03Valve = new()
            {
                Address = 6, Name = "Milk Tank 03 Valve",
                Mode = TagMode.Output, Value = 1,
            };

            Tags.Add(milkTank01);
            Tags.Add(milkTank02);
            Tags.Add(milkTank03);
            Tags.Add(milkTank01Valve);
            Tags.Add(milkTank02Valve);
            Tags.Add(milkTank03Valve);

            Tag milkFilter = new()
            {
                Address = 7,
                Name = "Milk Filter Tank",
                Type = TagType.Analog,
                Unit = "litre",
                Min = 0,
                Max = 20000
            };
            Tag milkFilterValve = new()
            {
                Address = 8, Name = "Milk Filter Tank Valve",
                Mode = TagMode.Output, Value = 1,
            };
            Tag processedDairyTank = new()
            {
                Address = 9,
                Name = "Processed Dairy Tank",
                Type = TagType.Analog,
                Unit = "litre",
                Min = 0,
                Max = 10000
            };
            Tag processedDairyTankOutValve = new()
            {
                Address = 10,
                Name = "Processed Dairy Tank Valve",
                Mode = TagMode.Output,
                Value = 1,
            };
            Tag rawCreamStorageTank = new()
            {
                Address = 11,
                Name = "Raw Cream Storage",
                Type = TagType.Analog,
                Unit = "litre",
                Min = 0,
                Max = 10000
            };
            Tags.Add(milkFilter);
            Tags.Add(milkFilterValve);
            Tags.Add(processedDairyTank);
            Tags.Add(processedDairyTankOutValve);
            Tags.Add(rawCreamStorageTank);


            Tag compressorInValve = new()
            {
                Address = 12,
                Name = "Compressor Ingoing Valve",
                Mode = TagMode.Output,
                Value = 1,
            };
            Tag manometer = new()
            {
                Address = 13,
                Name = "Manometer",
                Type = TagType.Analog,
                Unit = "mB",
                Min = 5000,
                Max = 50000
            };
            Tag compressor = new()
            {
                Address = 14,
                Name = "Compressor",
                Type = TagType.Analog,
                Mode = TagMode.Output,
                Unit = "mB",
                Min = 0,
                Max = 1000,
                Value = 100,
            };
            Tag postCompressorTankValve = new()
            {
                Address = 15,
                Name = "Post Compressor Tank Inner Valve",
                Mode = TagMode.Output,
                Value = 1,
            };
            Tag postCompressorTank = new()
            {
                Address = 16,
                Name = "Post Compressor Tank",
                Type = TagType.Analog,
                Unit = "litre",
                Min = 0,
                Max = 10000
            };
            Tags.Add(compressorInValve);
            Tags.Add(manometer);
            Tags.Add(compressor);
            Tags.Add(postCompressorTankValve);
            Tags.Add(postCompressorTank);

            Tag waterTank = new()
            {
                Address = 17,
                Name = "Water Tank",
                Type = TagType.Analog,
                Unit = "litre",
                Min = 0,
                Max = 100000
            };
            Tag coolingTank = new()
            {
                Address = 18,
                Name = "Cooling Tank",
                Type = TagType.Analog,
                Unit = "litre",
                Min = 0,
                Max = 100000
            };
            Tag coolingTankThermometer = new()
            {
                Address = 19,
                Name = "Cooling Tank Thermometer",
                Type = TagType.Analog,
                Unit = "°C",
                Min = -50,
                Max = 50
            };
            Tag coolingTankCondenser01 = new()
            {
                Address = 20,
                Name = "Cooling Tank Condenser 01",
                Mode = TagMode.Output,
                Value = 1,
            };
            Tag coolingTankCondenser02 = new()
            {
                Address = 21,
                Name = "Cooling Tank Condenser 02",
                Mode = TagMode.Output,
                Value = 0,
            };
            Tag coolingTankCondenser03 = new()
            {
                Address = 22,
                Name = "Cooling Tank Condenser 03",
                Mode = TagMode.Output,
                Value = 0,
            };
            Tag coolingTankCondenser04 = new()
            {
                Address = 23,
                Name = "Cooling Tank Condenser 04",
                Mode = TagMode.Output,
                Value = 0,
            };
            Tags.Add(waterTank);
            Tags.Add(coolingTank);
            Tags.Add(coolingTankThermometer);
            Tags.Add(coolingTankCondenser01);
            Tags.Add(coolingTankCondenser02);
            Tags.Add(coolingTankCondenser03);
            Tags.Add(coolingTankCondenser04);

            Alarms.Add(new()
            {
                ThresholdType = AlarmThresholdType.ABOVE,
                Priority = AlarmPriority.HIGH,
                Threshold = 5,
                Tag = waterTank,
                IsActive = false,
            });
            Alarms.Add(new()
            {
                ThresholdType = AlarmThresholdType.ABOVE,
                Priority = AlarmPriority.MEDIUM,
                Threshold = 1.7,
                Tag = coolingTank,
                IsActive = false,
            });

            SaveChanges();
        }

		protected override void OnConfiguring(DbContextOptionsBuilder options)
		{
			options.UseSqlite("Data Source=Data/app.db");
		}
	}
}
