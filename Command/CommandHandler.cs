using System;
using UndefinedBot.Net.NetWork;
using UndefinedBot.Net.Utils;

namespace UndefinedBot.Net.Command
{
    public class MsgHandler
    {
        private static readonly List<long> WorkGRoup = Program.GetConfigManager().GetGroupList();

        private static readonly CommandHandler CmdHandler = new();

        private static readonly List<string> CommandList = [];
        public static async Task HandleMsg(MsgBodySchematics MsgBody)
        {
            if ((MsgBody.PostType?.Equals("message") ?? false) &&
                (MsgBody.MessageType?.Equals("group") ?? false) &&
                WorkGRoup.Contains(MsgBody.GroupId ?? 0)
                )
            {
                ArgSchematics Args = CommandResolver.Parse(MsgBody);
                if (Args.Status)
                {
                    //CommandExecutor.Execute(Args);
                    if (CommandList.Contains(Args.Command))
                    {
                        CmdHandler.Trigger(Args);
                    }
                    else
                    {
                        await HttpApi.SendGroupMsg(
                                Args.GroupId,
                                new MsgBuilder()
                                    .Text($"这发的什么东西: <{Args.Command}>").Build()
                            );
                    }
                }
            }
        }
        public static CommandHandler GetCommandHandler()
        {
            return CmdHandler;
        }
        public static void UpdateCL(List<string> CL)
        {
            foreach (var item in CL)
            {
                CommandList.Add(item);
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
