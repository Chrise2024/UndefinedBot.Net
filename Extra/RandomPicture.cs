using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UndefinedBot.Net.NetWork;

namespace UndefinedBot.Net.Extra
{
    internal class RandomPicture
    {
        private static readonly Random RandomRoot = new();
        public static string RandomBingWallPaper()
        {
            try
            {
                JObject Resp = JObject.Parse(HttpRequest.Get($"https://www.bing.com/HPImageArchive.aspx?format=js&idx={RandomRoot.Next(0, 32767)}&n=1").Result);
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
        public static string RandomFox()
        {
            return $"https://randomfox.ca/images/{RandomRoot.Next(1,124)}.jpg";
        }
        public static string RandomCat()
        {
            try
            {
                List<JObject> IA = JsonConvert.DeserializeObject<List<JObject>>(HttpRequest.Get("https://api.thecatapi.com/v1/images/search").Result) ?? [];
                if (IA.Count > 0)
                {
                    return IA[0].Value<string>("url") ?? "";
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
        public static string RandomDog()
        {
            try
            {
                List<JObject> IA = JsonConvert.DeserializeObject<List<JObject>>(HttpRequest.Get("https://api.thedogapi.com/v1/images/search").Result) ?? [];
                if (IA.Count > 0)
                {
                    return IA[0].Value<string>("url") ?? "";
                }
                else
                {
                    JObject Resp = JObject.Parse(HttpRequest.Get("https://dog.ceo/api/breeds/image/random").Result);
                    return Resp.Value<string>("message") ?? "";
                }
            }
            catch
            {
                return "";
            }
        }
    }
}
