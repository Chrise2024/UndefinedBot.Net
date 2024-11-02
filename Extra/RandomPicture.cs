using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UndefinedBot.Net.NetWork;

namespace UndefinedBot.Net.Extra
{
    public class RandomPicture
    {
        private static readonly Random s_randomRoot = new();

        public static string GetRandomContent(string RandType)
        {
            if (RandType.Equals("bg"))
            {
                return RandomBingWallPaper();
            }
            else if (RandType.Equals("fox"))
            {
                return RandomFox();
            }
            else if (RandType.Equals("cat"))
            {
                return RandomCat();
            }
            else if (RandType.Equals("dog"))
            {
                return RandomDog();
            }
            else if (RandType.Equals("acg"))
            {
                return RandomACG();
            }
            else if (RandType.Equals("star"))
            {
                return RandomStarrySky();
            }
            return "";
        }
        private static string RandomBingWallPaper()
        {
            try
            {
                JObject Resp = JObject.Parse(Program.GetHttpRequest().Get($"https://www.bing.com/HPImageArchive.aspx?format=js&idx={s_randomRoot.Next(0, 32767)}&n=1").Result);
                List<JObject> IA = Resp["images"]?.ToObject<List<JObject>>() ?? [];
                if (IA.Count > 0)
                {
                    string USuffix = IA[0].Value<string>("url") ?? "";
                    return USuffix.Length > 0 ? $"https://www.bing.com{USuffix}" : "";
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                return "";
            }
        }
        private static string RandomFox()
        {
            return $"https://randomfox.ca/images/{s_randomRoot.Next(1,124)}.jpg";
        }
        private static string RandomCat()
        {
            try
            {
                if (s_randomRoot.Next(1,100) > 64)
                {
                    List<JObject> IA = JsonConvert.DeserializeObject<List<JObject>>(Program.GetHttpRequest().Get("https://api.thecatapi.com/v1/images/search").Result) ?? [];
                    if (IA.Count > 0)
                    {
                        return IA[0].Value<string>("url") ?? "";
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    JObject Resp = JObject.Parse(Program.GetHttpRequest().Get("https://nekobot.xyz/api/image?type=neko").Result);
                    return Resp.Value<string>("message") ?? "";
                }
            }
            catch
            {
                return "";
            }
        }
        private static string RandomDog()
        {
            try
            {
                List<JObject> IA = JsonConvert.DeserializeObject<List<JObject>>(Program.GetHttpRequest().Get("https://api.thedogapi.com/v1/images/search").Result) ?? [];
                if (IA.Count > 0)
                {
                    return IA[0].Value<string>("url") ?? "";
                }
                else
                {
                    JObject Resp = JObject.Parse(Program.GetHttpRequest().Get("https://dog.ceo/api/breeds/image/random").Result);
                    return Resp.Value<string>("message") ?? "";
                }
            }
            catch
            {
                return "";
            }
        }
        private static string RandomACG()
        {
            try
            {
                if (s_randomRoot.Next(1, 100) > 75)
                {
                    return Program.GetHttpRequest().Get("https://www.loliapi.com/bg/?type=url").Result.Replace(".cn", ".com");
                }
                else
                {
                    JObject Resp = JObject.Parse(Program.GetHttpRequest().Get("https://iw233.cn/api.php?sort=cdniw&type=json").Result);
                    List<string> IA = Resp["pic"]?.ToObject<List<string>>() ?? [];
                    return IA.Count > 0 ? IA[0] : "";
                }
            }
            catch
            {
                return "";
            }
        }
        private static string RandomStarrySky()
        {
            try
            {
                JObject Resp = JObject.Parse(Program.GetHttpRequest().Get("https://moe.jitsu.top/api/?sort=starry&type=json").Result);
                List<string> IA = Resp["pics"]?.ToObject<List<string>>() ?? [];
                return IA.Count > 0 ? IA[0] : "";
            }
            catch
            {
                return "";
            }
        }
    }
}
