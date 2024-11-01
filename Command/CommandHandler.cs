using System;
using Newtonsoft.Json;
using UndefinedBot.Net.NetWork;
using UndefinedBot.Net.Utils;

namespace UndefinedBot.Net.Command
{
    public class MsgHandler
    {
        private static readonly List<long> s_workGRoup = Program.GetConfigManager().GetGroupList();

        private static readonly CommandHandler s_commandHandler = new();

        private static readonly List<string> s_commandList = [];

        private static readonly Logger s_commandHandlerLogger = new("MsgHandler");
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
                        s_commandHandler.Trigger(args);
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
        public static CommandHandler GetCommandHandler()
        {
            return s_commandHandler;
        }
        public static void UpdateCommandList(List<string> CL)
        {
            foreach (var item in CL)
            {
                s_commandList.Add(item);
            }
        }
    }
    public delegate Task CommandEventHandler(ArgSchematics CommandArg);
    public class CommandHandler
    {
        public event CommandEventHandler? CommandEvent;

        public void Trigger(ArgSchematics CommandArg)
        {
            CommandEvent?.Invoke(CommandArg);
        }
    }
}
