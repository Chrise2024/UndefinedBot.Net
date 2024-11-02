using System;
using System.Text;
using System.Net;
using System.Net.Http.Json;
using System.Drawing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UndefinedBot.Net;
using UndefinedBot.Net.Utils;
using UndefinedBot.Net.Command;

namespace UndefinedBot.Net.NetWork
{
    public class HttpApi
    {
        private static readonly string s_httpPostUrl = Program.GetConfigManager().GetHttpPostUrl();

        private static readonly HttpClient s_httpClient = new()
        {
            Timeout = TimeSpan.FromSeconds(20)
        };

        private static readonly Logger s_httpApiLogger = new("HttpRequest");

        public static async Task SendGroupMsg<T>(T targetGroupId,List<JObject> msgChain)
        {
            try
            {
                object ReqJSON = new
                {
                    group_id = targetGroupId,
                    message = msgChain
                };
                //Console.WriteLine(JsonConvert.SerializeObject(ReqJSON));
                await s_httpClient.PostAsync(s_httpPostUrl + "/send_group_msg",
                   new StringContent(
                       JsonConvert.SerializeObject(ReqJSON),
                       Encoding.UTF8,
                       "application/json"
                   )
                );
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine("Task Cacled: ");
                s_httpApiLogger.Error(ex.Message);
                s_httpApiLogger.Error(ex.StackTrace ?? "");
            }
            catch (Exception ex)
            {
                s_httpApiLogger.Error("Error Occured, Error Information:");
                s_httpApiLogger.Error(ex.Message);
                s_httpApiLogger.Error(ex.StackTrace ?? "");
            }
        }
        public static async void RecallGroupMsg<T>(T msgId)
        {
            try
            {
                object ReqJSON = new
                {
                    message_id = msgId
                };
                await s_httpClient.PostAsync(s_httpPostUrl + "/delete_msg",
                   new StringContent(
                       JsonConvert.SerializeObject(ReqJSON),
                       Encoding.UTF8,
                       "application/json"
                   )
                );
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine("Task Cacled: ");
                s_httpApiLogger.Error(ex.Message);
                s_httpApiLogger.Error(ex.StackTrace ?? "");
            }
            catch (Exception ex)
            {
                s_httpApiLogger.Error("Error Occured, Error Information:");
                s_httpApiLogger.Error(ex.Message);
                s_httpApiLogger.Error(ex.StackTrace ?? "");
            }
        }
        public static async Task<MsgBodySchematics> GetMsg<T>(T msgId)
        {
            try
            {
                object ReqJSON = new
                {
                    message_id = msgId
                };
                HttpResponseMessage response = await s_httpClient.PostAsync(s_httpPostUrl + "/get_msg",
                   new StringContent(
                       JsonConvert.SerializeObject(ReqJSON),
                       Encoding.UTF8,
                       "application/json"
                   )
                );
                return JObject.Parse(response.Content.ReadAsStringAsync().Result)["data"]?.ToObject<MsgBodySchematics>() ?? new MsgBodySchematics();
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine("Task Cacled: ");
                s_httpApiLogger.Error(ex.Message);
                s_httpApiLogger.Error(ex.StackTrace ?? "");
                return new MsgBodySchematics();
            }
            catch
            {
                return new MsgBodySchematics();
            }
        }
        public static async Task<GroupMemberSchematics> GetGroupMember<T1, T2>(T1 targetGroupId, T2 targetUin)
        {
            try
            {
                object ReqJSON = new
                {
                    group_id = targetGroupId,
                    user_id = targetUin,
                    no_cache = false
                };
                HttpResponseMessage response = await s_httpClient.PostAsync(s_httpPostUrl + "/get_group_member_info",
                   new StringContent(
                       JsonConvert.SerializeObject(ReqJSON),
                       Encoding.UTF8,
                       "application/json"
                   )
                );
                return JObject.Parse(response.Content.ReadAsStringAsync().Result)["data"]?.ToObject<GroupMemberSchematics>() ?? new GroupMemberSchematics();
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine("Task Cacled: ");
                s_httpApiLogger.Error(ex.Message);
                s_httpApiLogger.Error(ex.StackTrace ?? "");
                return new GroupMemberSchematics();
            }
            catch
            {
                return new GroupMemberSchematics();
            }
        }
        public static async Task<HitokotoSchematics> GetHitokoto(string htioType)
        {
            string Para = "";
            foreach(char index in htioType)
            {
                if (index >= 'a' && index <= 'l')
                {
                    Para += $"c={index}&";
                }
            }
            try
            {
                HttpResponseMessage response = await s_httpClient.GetAsync("https://v1.hitokoto.cn/?" + Para);
                return JsonConvert.DeserializeObject<HitokotoSchematics>(response.Content.ReadAsStringAsync().Result);
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine("Task Cacled: ");
                s_httpApiLogger.Error(ex.Message);
                s_httpApiLogger.Error(ex.StackTrace ?? "");
                return new HitokotoSchematics();
            }
            catch
            {
                return new HitokotoSchematics();
            }
        }
        public static async Task<Image> GetQQAvatar<T>(T targetUin)
        {
            try
            {
                byte[] ImageBytes = await s_httpClient.GetByteArrayAsync($"http://q.qlogo.cn/headimg_dl?dst_uin={targetUin}&spec=640&img_type=jpg");
                if (ImageBytes.Length > 0)
                {
                    MemoryStream ms = new MemoryStream(ImageBytes);
                    Image Im = Image.FromStream(ms);
                    ms.Close();
                    return Im;
                }
                else
                {
                    return Image.FromFile(Path.Join(Program.GetProgramRoot(), "Local", "512x512.png"));
                }
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine("Task Cacled: ");
                s_httpApiLogger.Error(ex.Message);
                s_httpApiLogger.Error(ex.StackTrace ?? "");
                return new Bitmap(1, 1);
            }
            catch
            {
                return new Bitmap(1,1);
            }
        }
        public static async Task<bool> CheckUin(long targetGroupId, long targetUin)
        {
            return ((await GetGroupMember(targetGroupId, targetUin)).GroupId ?? 0) != 0;
        }
    }
    public class HttpRequest
    {
        private static readonly HttpClient s_httpClient = new()
        {
            Timeout = TimeSpan.FromSeconds(5)
        };

        private static readonly Logger s_httpRequestLogger = new("HttpRequest");

        public static async Task<string> POST(string Url, object? content = null)
        {
            try
            {
                HttpResponseMessage response = await s_httpClient.PostAsync(Url, content == null ? null : new StringContent(
                       JsonConvert.SerializeObject(content),
                       Encoding.UTF8,
                       "application/json"
                   ));
                return await response.Content.ReadAsStringAsync();
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine("Task Cacled: ");
                s_httpRequestLogger.Error(ex.Message);
                s_httpRequestLogger.Error(ex.StackTrace ?? "");
                return "";
            }
            catch (Exception ex)
            {
                s_httpRequestLogger.Error("Error Occured, Error Information:");
                s_httpRequestLogger.Error(ex.Message);
                s_httpRequestLogger.Error(ex.StackTrace ?? "");
                return "";
            }
        }

        public static async Task<string> Get(string url)
        {
            try
            {
                HttpResponseMessage response = await s_httpClient.GetAsync(url);
                return await response.Content.ReadAsStringAsync();
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine("Task Cacled: ");
                s_httpRequestLogger.Error(ex.Message);
                s_httpRequestLogger.Error(ex.StackTrace ?? "");
                return "";
            }
            catch (Exception ex)
            {
                s_httpRequestLogger.Error("Error Occured, Error Information:");
                s_httpRequestLogger.Error(ex.Message);
                s_httpRequestLogger.Error(ex.StackTrace ?? "");
                return "";
            }
        }

        public static async Task<byte[]> GetBinary(string url)
        {
            try
            {
                return await s_httpClient.GetByteArrayAsync(url);
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine("Task Cacled: ");
                s_httpRequestLogger.Error(ex.Message);
                s_httpRequestLogger.Error(ex.StackTrace ?? "");
                return [];
            }
            catch (Exception ex)
            {
                s_httpRequestLogger.Error("Error Occured, Error Information:");
                s_httpRequestLogger.Error(ex.Message);
                s_httpRequestLogger.Error(ex.StackTrace ?? "");
                return [];
            }
        }
    }
    public struct HitokotoSchematics
    {
        [JsonProperty("id")] public int? Id;
        [JsonProperty("uuid")] public string? Uuid;
        [JsonProperty("hitokoto")] public string? Hitokoto;
        [JsonProperty("type")] public string? Type;
        [JsonProperty("from")] public string? From;
        [JsonProperty("from_who")] public string? FromWho;
        [JsonProperty("creator")] public string? Creator;
        [JsonProperty("creator_uid")] public int? CreatorUid;
        [JsonProperty("reviewer")] public int? Reviewer;
        [JsonProperty("commit_from")] public string? CommitFrom;
        [JsonProperty("created_at")] public string? CreatedAt;
        [JsonProperty("length")] public int? Length;
    }
    public struct GroupMemberSchematics
    {
        [JsonProperty("group_id")] public long? GroupId;
        [JsonProperty("user_id")] public long? UserId;
        [JsonProperty("nickname")] public string? Nickname;
        [JsonProperty("card")] public string? Card;
        [JsonProperty("sex")] public string? Sex;
        [JsonProperty("age")] public int? Age;
        [JsonProperty("area")] public string? Area;
        [JsonProperty("join_time")] public int? JoinTime;
        [JsonProperty("last_sent_time")] public int? LastSentTime;
        [JsonProperty("level")] public string? Level;
        [JsonProperty("role")] public string? Role;
        [JsonProperty("unfriendly")] public bool? Unfriendly;
        [JsonProperty("title")] public string? Title;
        [JsonProperty("title_expire_time")] public int? TitleExpireTime;
        [JsonProperty("card_changeable")] public bool? CardChangeable;
    }
}
