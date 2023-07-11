namespace USca_Server.Util
{
    // Copied from hci-travel-agent (courtesy of magley) with minor changes
    public class ScadaConfig
    {
        public const int DefaultSyncThreadTimerInMs = 1000;
        public const string DefaultAlarmLogPath = "./Data/alarmLog.txt";

        public int SyncThreadTimerInMs { get; set; } = DefaultSyncThreadTimerInMs;
        public string AlarmLogPath { get; set; } = DefaultAlarmLogPath;

        private static readonly ScadaConfig _instance = new();
        private ScadaConfig()
        {
            LoadConfig();
        }
        public static ScadaConfig Instance { get { return _instance; } }

        private void LoadConfig()
        {
            List<string> lines;
            try
            {
                lines = File.ReadAllLines("./Data/scadaConfig.txt").ToList();
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Config not found, creating default config...");
                Save();
                return;
            }

            bool shouldUpdateDefault = false;
            foreach (var line in lines)
            {
                var tokens = line.Split("=", StringSplitOptions.TrimEntries);
                var key = tokens[0];
                var value = tokens[1];

                if (key == "SyncThreadTimerInMs")
                {
                    bool success = int.TryParse(value, out int miliseconds);
                    if (!success) shouldUpdateDefault = true;
                    SyncThreadTimerInMs = success ? miliseconds : 1000;
                }
                if (key == "AlarmLogPath")
                {
                    string path = value.ToString();
                    if (path == "") shouldUpdateDefault = true;
                    AlarmLogPath = (path != "") ? path : DefaultAlarmLogPath;
                }
            }
            if (shouldUpdateDefault)
            {
                Save();
            }
        }

        public void Save()
        {
            string s = "";

            s += $"SyncThreadTimerInMs={SyncThreadTimerInMs}\n";
            s += $"AlarmLogPath={AlarmLogPath}\n";

            File.WriteAllText("./Data/scadaConfig.txt", s);
        }
    }
}
