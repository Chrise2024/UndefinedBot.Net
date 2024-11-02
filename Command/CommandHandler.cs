using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UndefinedBot.Net.NetWork;
using UndefinedBot.Net.Utils;

namespace UndefinedBot.Net.Command
{
    public delegate Task CommandEventHandler(ArgSchematics CommandArg);
    public class CommandHandler
    {
        private static readonly List<long> s_workGRoup = Program.GetConfigManager().GetGroupList();

        private static readonly List<string> s_commandList = [];

        private static readonly Logger s_commandHandlerLogger = new("MsgHandler");

        public static event CommandEventHandler? CommandEvent;
        public static async Task HandleMsg(MsgBodySchematics MsgBody)
        {
            if ((MsgBody.PostType?.Equals("message") ?? false) &&
                (MsgBody.MessageType?.Equals("group") ?? false) &&
                s_workGRoup.Contains(MsgBody.GroupId ?? 0)
                )
            {
                ArgSchematics args = CommandResolver.Parse(MsgBody);
                if (args.Status)
                {
                    if (s_commandList.Contains(args.Command))
                    {
                        s_commandHandlerLogger.Info("Executing with arg:");
                        s_commandHandlerLogger.Info(JsonConvert.SerializeObject(args,Formatting.Indented));
                        //s_commandHandler.Trigger(args);
                        CommandEvent?.Invoke(args);
                    }
                    else
                    {
                        await HttpApi.SendGroupMsg(
                                args.GroupId,
                                new MsgBuilder()
                                    .Text($"这发的什么东西: <{args.Command}>").Build()
                            );
                    }
                }
            }
        }
        public static void UpdateCommandList(List<string> CL)
        {
            foreach (var item in CL)
            {
                s_commandList.Add(item);
            }
        }
    }
    public struct MsgSenderSchematics
    {
        [JsonProperty("user_id")] public long? UserId;
        [JsonProperty("nickname")] public string? Nickname;
        [JsonProperty("sex")] public string? Sex;
        [JsonProperty("age")] public int? Age;
    }
    public struct MsgBodySchematics
    {
        [JsonProperty("time")] public long? Time;
        [JsonProperty("self_id")] public long? SelfId;
        [JsonProperty("post_type")] public string? PostType;
        [JsonProperty("message_type")] public string? MessageType;
        [JsonProperty("sub_type")] public string? SubType;
        [JsonProperty("message_id")] public int? MessageId;
        [JsonProperty("group_id")] public long? GroupId;
        [JsonProperty("user_id")] public long? UserId;
        [JsonProperty("message")] public List<JObject>? Message;
        [JsonProperty("raw_message")] public string? RawMessage;
        [JsonProperty("font")] public int? Font;
        [JsonProperty("sender")] public MsgSenderSchematics? Sender;
    }
}
