using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UndefinedBot.Net.Utils
{
    public struct ConfigSchematics(
        string httpServerUrl,
        string httpPostUrl,
        List<long> groupId,
        string commandPrefix
        )
    {
        [JsonProperty("http_server_url")] public string HttpServerUrl = httpServerUrl;
        [JsonProperty("http_post_url")] public string HttpPostUrl = httpPostUrl;
        [JsonProperty("group_id")] public List<long> GroupId = groupId;
        [JsonProperty("command_prefix")] public string CommandPrefix = commandPrefix;
    }

    public struct ArgSchematics(
        string command,
        List<string> param,
        long callerUin,
        long groupId,
        int msgId,
        bool status
        )
    {
        public string Command = command;
        public List<string> Param = param;
        public long CallerUin = callerUin;
        public long GroupId = groupId;
        public int MsgId = msgId;
        public bool Status = status;
    }

    public struct CQEntitySchematics(string CQType)
    {
        public string CQType = CQType;
        public Dictionary<string, string> Properties;
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
    public struct CommandPropertieSchematics(string name, string description, string shortDescription,object instance)
    {
        public string Name = name;
        public string Description = description;
        public string ShortDescription = shortDescription;
        public object Instance = instance;
    }
    public class Schematics
    {
    }
}
