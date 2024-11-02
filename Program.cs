using System.Net;
using System.Text;
using System.Reflection;
using System.Drawing;
using System.Drawing.Imaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UndefinedBot.Net.Command;
using UndefinedBot.Net.Extra;
using UndefinedBot.Net.NetWork;
using UndefinedBot.Net.Utils;

namespace UndefinedBot.Net
{
    public class Program
    {
        private static readonly string ProgramRoot = Environment.CurrentDirectory;

        private static readonly string ProgramCahce = Path.Join(ProgramRoot, "Cache");

        private static readonly string ProgramLocal = Path.Join(ProgramRoot, "Local");

        private static readonly ConfigManager s_mainConfigManager = new();

        private static readonly HttpServer s_httpServer = new(s_mainConfigManager.GetHttpServerUrl());

        private static readonly Logger s_mainLogger = new("Program");

        private static readonly Assembly s_mainAssembly = Assembly.GetExecutingAssembly();

        private static readonly Dictionary<string,CommandPropertieSchematics> s_commandReference = CommandInitializer.InitCommand();
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            FileIO.EnsurePath(ProgramCahce);
            FileIO.EnsurePath(ProgramLocal);
            //Console.WriteLine("{0:x}",111);
            CommandHandler.UpdateCommandList([.. s_commandReference.Keys]);
            if (!File.Exists(Path.Join(ProgramLocal, "QSplash.png")))
            {
                Stream? stream = s_mainAssembly.GetManifestResourceStream("UndefinedBot.Net.Local.QSplash.png");
                if (stream != null)
                {
                    Image CoverImage = Image.FromStream(stream);
                    CoverImage.Save(Path.Join(ProgramLocal, "QSplash.png"), ImageFormat.Png);
                    CoverImage.Dispose();
                }
                stream?.Close();
            }
            s_mainLogger.Info("Bot Launched");
            Task.Run(s_httpServer.Start);
            string TempString;
            while (true)
            {
                TempString = Console.ReadLine() ?? "";
                if (TempString.Equals("stop"))
                {
                    s_httpServer.Stop();
                    break;
                }
            }
            s_mainLogger.Info("Bot Colsed");
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
            return s_mainConfigManager;
        }
        public static Dictionary<string,CommandPropertieSchematics> GetCommandReference()
        {
            return s_commandReference;
        }
    }
}
