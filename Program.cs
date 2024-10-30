using System.Net;
using System.Text;
using System.Reflection;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UndefinedBot.Net.Command;
using UndefinedBot.Net.Extra;
using UndefinedBot.Net.NetWork;
using UndefinedBot.Net.Utils;
using ImageMagick;
using System.Globalization;

namespace UndefinedBot.Net
{
    internal class Program
    {
        private static readonly string ProgramRoot = Environment.CurrentDirectory;

        private static readonly string ProgramCahce = Path.Join(ProgramRoot, "Cache");

        private static readonly string ProgramLocal = Path.Join(ProgramRoot, "Local");

        private static readonly ConfigManager MainConfigManager = new();

        private static readonly HttpServer HServer = new(MainConfigManager.GetHttpServerUrl());

        private static readonly Logger MainLogger = new("Program");

        private static readonly Assembly MainAssembly = Assembly.GetExecutingAssembly();

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            MainLogger.Info("Bot Launched");
            FileIO.EnsurePath(ProgramCahce);
            FileIO.EnsurePath(ProgramLocal);
            if (!File.Exists(Path.Join(ProgramLocal, "QSplash.png")))
            {
                Stream? stream = MainAssembly.GetManifestResourceStream("UndefinedBot.Net.Local.QSplash.png");
                if (stream != null)
                {
                    Image CoverImage = Image.FromStream(stream);
                    CoverImage.Save(Path.Join(ProgramLocal, "QSplash.png"), ImageFormat.Png);
                    CoverImage.Dispose();
                }
                stream?.Close();
            }
            Task.Run(HServer.Start);
            string TempString;
            while (true)
            {
                TempString = Console.ReadLine() ?? "";
                if (TempString.Equals("stop"))
                {
                    HServer.Stop();
                    break;
                }
            }
            MainLogger.Info("Bot Colsed");
            Console.ReadKey();
        }
        public static string GetProgramRoot()
        {
            return ProgramRoot;
        }
        public static string GetProgramCahce()
        {
            return ProgramCahce;
        }
        public static string GetProgramLocal()
        {
            return ProgramLocal;
        }
        public static ConfigManager GetConfigManager()
        {
            return MainConfigManager;
        }
    }
}
