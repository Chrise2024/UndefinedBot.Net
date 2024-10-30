using System;
using UndefinedBot.Net.NetWork;
using UndefinedBot.Net.Utils;
using static System.Net.Mime.MediaTypeNames;

namespace UndefinedBot.Net.Command.Content
{
    public class HelpCommand:IBaseCommand
    {
        public string CommandName { get;} = "help";
        public string CommandDescription { get; } = "help";

        private readonly Logger CommandLogger = new("Command","Help");
        public async Task Handle(ArgSchematics Args)
        {
            if (Args.Command.Equals(CommandName))
            {
                CommandLogger.Info("Command Triggered");
                if (Args.Param.Count > 0)
                {
                    if (Program.GetCommandReference().TryGetValue(Args.Param[0], out var txt))
                    {
                        await HttpApi.SendGroupMsg(
                            Args.GroupId,
                            new MsgBuilder()
                                .Text(txt).Build()
                        );
                    }
                    else
                    {
                        await HttpApi.SendGroupMsg(
                            Args.GroupId,
                            new MsgBuilder()
                                .Text("咦，没有这个指令").Build()
                        );
                        CommandLogger.Warn($"Command Not Found: <{Args.Param[0]}>");
                    }
                }
                else
                {
                    await HttpApi.SendGroupMsg(
                            Args.GroupId,
                    new MsgBuilder()
                                .Text("help").Build()
                        );
                }
                CommandLogger.Info("Command Completed");
            }
        }
        public void Init()
        {
            MsgHandler.GetCommandHandler().CommandEvent += Handle;
            CommandLogger.Info("Loaded");
        }
    }
}
