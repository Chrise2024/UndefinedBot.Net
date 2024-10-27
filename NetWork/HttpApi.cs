using System;
using System.Text;
using System.Net;
using System.Net.Http.Json;
using System.Drawing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UndefinedBot.Net;
using UndefinedBot.Net.Utils;

namespace UndefinedBot.Net.NetWork
{
    internal class HttpApi
    {
        private static readonly string HttpPostUrl = Program.GetConfigManager().GetHttpPostUrl();

        private static readonly HttpClient HClient = new()
        {
            Timeout = TimeSpan.FromSeconds(5)
        };

        private static readonly Logger HApiLogger = new("HttpRequest");

        public static async Task SendGroupMsg<T>(T TargetGroupId,List<JObject> MsgChain)
        {
            try
            {
                object ReqJSON = new
                {
                    group_id = TargetGroupId,
                    message = MsgChain
                };
                //Console.WriteLine(JsonConvert.SerializeObject(ReqJSON));
                await HClient.PostAsync(HttpPostUrl + "/send_group_msg",
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
                HApiLogger.Error(ex.Message);
                HApiLogger.Error(ex.StackTrace ?? "");
            }
            catch (Exception ex)
            {
                HApiLogger.Error("Error Occured, Error Information:");
                HApiLogger.Error(ex.Message);
                HApiLogger.Error(ex.StackTrace ?? "");
            }
        }
        public static async void RecallGroupMsg<T>(T MsgId)
        {
            try
            {
                object ReqJSON = new
                {
                    message_id = MsgId
                };
                await HClient.PostAsync(HttpPostUrl + "/delete_msg",
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
                HApiLogger.Error(ex.Message);
                HApiLogger.Error(ex.StackTrace ?? "");
            }
            catch (Exception ex)
            {
                HApiLogger.Error("Error Occured, Error Information:");
                HApiLogger.Error(ex.Message);
                HApiLogger.Error(ex.StackTrace ?? "");
            }
        }
        public static async Task<MsgBodySchematics> GetMsg<T>(T MsgId)
        {
            try
            {
                object ReqJSON = new
                {
                    message_id = MsgId
                };
                HttpResponseMessage response = await HClient.PostAsync(HttpPostUrl + "/get_msg",
                   new StringContent(
                       JsonConvert.SerializeObject(ReqJSON),
                       Encoding.UTF8,
                       "application/json"
                   )
                );
                //Console.WriteLine("GetMsg<T>");
                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                return JObject.Parse(response.Content.ReadAsStringAsync().Result)["data"]?.ToObject<MsgBodySchematics>() ?? new MsgBodySchematics();
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine("Task Cacled: ");
                HApiLogger.Error(ex.Message);
                HApiLogger.Error(ex.StackTrace ?? "");
                return new MsgBodySchematics();
            }
            catch
            {
                return new MsgBodySchematics();
            }
        }
        public static async Task<GroupMemberSchematics> GetGroupMember<T1, T2>(T1 TargetGroupId, T2 TargetUin)
        {
            try
            {
                object ReqJSON = new
                {
                    group_id = TargetGroupId,
                    user_id = TargetUin,
                    no_cache = false
                };
                HttpResponseMessage response = await HClient.PostAsync(HttpPostUrl + "/get_group_member_info",
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
                HApiLogger.Error(ex.Message);
                HApiLogger.Error(ex.StackTrace ?? "");
                return new GroupMemberSchematics();
            }
            catch
            {
                return new GroupMemberSchematics();
            }
        }
        public static async Task<HitokotoSchematics> GetHitokoto(string HType)
        {
            string Para = "";
            foreach(char index in HType)
            {
                if (index >= 'a' && index <= 'l')
                {
                    Para += $"c={index}&";
                }
            }
            try
            {
                HttpResponseMessage response = await HClient.GetAsync("https://v1.hitokoto.cn/?" + Para);
                return JsonConvert.DeserializeObject<HitokotoSchematics>(response.Content.ReadAsStringAsync().Result);
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine("Task Cacled: ");
                HApiLogger.Error(ex.Message);
                HApiLogger.Error(ex.StackTrace ?? "");
                return new HitokotoSchematics();
            }
            catch
            {
                return new HitokotoSchematics();
            }
        }
        public static async Task<Image> GetQQAvatar<T>(T TargetUin)
        {
            try
            {
                byte[] ImageBytes = await HClient.GetByteArrayAsync($"http://q.qlogo.cn/headimg_dl?dst_uin={TargetUin}&spec=640&img_type=jpg");
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
                HApiLogger.Error(ex.Message);
                HApiLogger.Error(ex.StackTrace ?? "");
                return new Bitmap(1, 1);
            }
            catch
            {
                return new Bitmap(1,1);
            }
        }
        public static async Task<bool> CheckUin(long TargetGroupId, long TargetUin)
        {
            return ((await GetGroupMember(TargetGroupId, TargetUin)).GroupId ?? 0) != 0;
        }
    }
    internal class HttpRequest
    {
        private static readonly HttpClient HClient = new()
        {
            Timeout = TimeSpan.FromSeconds(5)
        };

        private static readonly Logger HRequestLogger = new("HttpRequest");

        public static async Task<string> POST(string Url, object? Content = null)
        {
            try
            {
                HttpResponseMessage response = await HClient.PostAsync(Url, Content == null ? null : new StringContent(
                       JsonConvert.SerializeObject(Content),
                       Encoding.UTF8,
                       "application/json"
                   ));
                return await response.Content.ReadAsStringAsync();
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine("Task Cacled: ");
                HRequestLogger.Error(ex.Message);
                HRequestLogger.Error(ex.StackTrace ?? "");
                return "";
            }
            catch (Exception ex)
            {
                HRequestLogger.Error("Error Occured, Error Information:");
                HRequestLogger.Error(ex.Message);
                HRequestLogger.Error(ex.StackTrace ?? "");
                return "";
            }
        }

        public static async Task<string> Get(string Url)
        {
            try
            {
                HttpResponseMessage response = await HClient.GetAsync(Url);
                return await response.Content.ReadAsStringAsync();
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine("Task Cacled: ");
                HRequestLogger.Error(ex.Message);
                HRequestLogger.Error(ex.StackTrace ?? "");
                return "";
            }
            catch (Exception ex)
            {
                HRequestLogger.Error("Error Occured, Error Information:");
                HRequestLogger.Error(ex.Message);
                HRequestLogger.Error(ex.StackTrace ?? "");
                return "";
            }
        }

        public static async Task<byte[]> GetBinary(string Url)
        {
            try
            {
                return await HClient.GetByteArrayAsync(Url);
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine("Task Cacled: ");
                HRequestLogger.Error(ex.Message);
                HRequestLogger.Error(ex.StackTrace ?? "");
                return [];
            }
            catch (Exception ex)
            {
                HRequestLogger.Error("Error Occured, Error Information:");
                HRequestLogger.Error(ex.Message);
                HRequestLogger.Error(ex.StackTrace ?? "");
                return [];
            }
        }
    }
}
