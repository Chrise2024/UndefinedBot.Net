using System;

namespace UndefinedBot.Net.Utils
{
    public class Logger(string nameSpace,string subType = "Common")
    {
        private readonly string _nameSpace = nameSpace;

        private readonly string _subType = subType;

        private readonly ConsoleColor DefaultConsoleColor = Console.ForegroundColor;
        public void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine("[{0}][{1}][{2}] {3}", GetFormatTime(),_nameSpace, _subType, message);
            Console.ForegroundColor = DefaultConsoleColor;
        }
        public void Warn(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[{0}][{1}][{2}] {3}", GetFormatTime(), _nameSpace, _subType, message);
            Console.ForegroundColor = DefaultConsoleColor;
        }
        public void Info(string message)
        {
            Console.WriteLine("[{0}][{1}][{2}] {3}", GetFormatTime(), _nameSpace, _subType, message);
        }
        private string GetFormatTime()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}