namespace USca_Server.Util
{
    public static class LogHelper
    {
        public static void ServiceLog(string? methodName, ConsoleColor color = ConsoleColor.Green)
        {
            Console.ForegroundColor = color;
            Console.WriteLine($"[{DateTime.Now}] {methodName}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void GeneralLog(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
