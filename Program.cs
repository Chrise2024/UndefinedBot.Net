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
    public class Program
    {
        private static readonly string ProgramRoot = Environment.CurrentDirectory;

        private static readonly string ProgramCahce = Path.Join(ProgramRoot, "Cache");

        private static readonly string ProgramLocal = Path.Join(ProgramRoot, "Local");

        private static readonly ConfigManager MainConfigManager = new();

        private static readonly HttpServer HServer = new(MainConfigManager.GetHttpServerUrl());

        private static readonly Logger MainLogger = new("Program");

        private static readonly Assembly MainAssembly = Assembly.GetExecutingAssembly();
        //Name -> Description
        private static readonly Dictionary<string, string> CommandReference = [];
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            FileIO.EnsurePath(ProgramCahce);
            FileIO.EnsurePath(ProgramLocal);
            //Activate Commands
            IEnumerable<Type> types = MainAssembly.GetTypes().Where(t => t.Namespace == "UndefinedBot.Net.Command.Content" && t.IsClass);
            foreach (Type type in types)
            {
                var instance = Activator.CreateInstance(type);
                MethodInfo? method = type.GetMethod("Init");
                PropertyInfo? NameInfo = type.GetProperty("CommandName");
                PropertyInfo? DescriptionInfo = type.GetProperty("CommandDescription");
                if (NameInfo?.GetValue(instance) is string CName && DescriptionInfo?.GetValue(instance) is string CDescription)
                {
                    method?.Invoke(instance, null);
                    CommandReference.Add(CName, CDescription);
                }
            }
            MsgHandler.UpdateCL([.. CommandReference.Keys]);
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
            MainLogger.Info("Bot Launched");
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
        public static Dictionary<string,string> GetCommandReference()
        {
            return CommandReference;
        }
    }
}
