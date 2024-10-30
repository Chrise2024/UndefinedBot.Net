using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using UndefinedBot.Net.Utils;

namespace UndefinedBot.Net.Utils
{
    public class ConfigManager
    {
        private readonly List<string> DefaultCommands = [
            "help",
            "symmet",
            "hito",
            "queto",
            //"invert",
            "raw",
            "mix",
            "random",
            "homo",
            "histoday",
        ];
        private readonly JSchema ConfigJsonSchema = JSchema.Parse(
            @"
            {
            ""type"": ""object"",
            ""properties"": {
                    ""http_server_url"": { ""type"": ""string"" , ""format"": ""uri"" },
                    ""http_post_url"": { ""type"": ""string"" , ""format"": ""uri"" },
                    ""group_id"": { ""type"": ""array"", ""items"": { ""type"": ""integer"" } },
                    ""command_prefix"": { ""type"": ""string"" }
                },
                ""required"": [""http_server_url"", ""http_post_url"", ""group_id"", ""command_prefix""]
            }"
            );

        private readonly string ConfigPath = Path.Join(Program.GetProgramRoot(), "config.json");

        private ConfigSchematics Config;

        private readonly ConfigSchematics DefaultConfig = new("http://127.0.0.1:8087/", "http://127.0.0.1:8085", [], "#");

        /*
         * 8087为Bot上报消息的Url，即当前程序开启的Http Server地址
         * 8085为Bot接收Http请求的Url，即当前程序发送Http请求的地址
         */

        public ConfigManager()
        {
            if (!File.Exists(ConfigPath))
            {
                FileIO.WriteAsJSON<ConfigSchematics>(ConfigPath, DefaultConfig);
                Config = DefaultConfig;
            }
            else
            {
                JObject RConfig = FileIO.ReadAsJSON(ConfigPath);
                if (RConfig.IsValid(ConfigJsonSchema))
                {
                    Config = RConfig.ToObject<ConfigSchematics>();
                }
                else
                {
                    Config = DefaultConfig;
                    FileIO.WriteAsJSON<ConfigSchematics>(ConfigPath, DefaultConfig);
                }
            }
        }
        public string GetHttpServerUrl()
        {
            return Config.HttpServerUrl;
        }
        public string GetHttpPostUrl()
        {
            return Config.HttpPostUrl;
        }
        public List<long> GetGroupList()
        {
            return Config.GroupId;
        }
        public List<string> GetCommandList()
        {
            return DefaultCommands;
        }
        public string GetCommandPrefix()
        {
            return Config.CommandPrefix;
        }
        private void SaveConfig()
        {
            FileIO.WriteAsJSON<ConfigSchematics>(ConfigPath, Config);
        }
    }
}