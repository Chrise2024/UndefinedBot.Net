using System;
using Newtonsoft.Json;
using UndefinedBot.Net.NetWork;
using UndefinedBot.Net.Utils;

namespace UndefinedBot.Net.Command.Content
{
    public class RawCommand
    {
        public string CommandName { get; } = "raw";
        public string CommandDescription { get; } = string.Format("---------------help---------------\n{0}raw - 群u到底发的什么东西\n使用方法：用{0}raw 回复想生成的消息\ne.g. {0}raw", Program.GetConfigManager().GetCommandPrefix());

        private readonly Logger CommandLogger = new("Command", "Raw");
        public async Task Handle(ArgSchematics Args)
        {
            //ParamFormat: Any
            if (Args.Command.Equals(CommandName))
            {
                CommandLogger.Info("Command Triggered");
                if (Args.Param.Count > 0)
                {
                    MsgBodySchematics TargetMsg = await HttpApi.GetMsg(Args.Param[0]);
                    await HttpApi.SendGroupMsg(
                                    Args.GroupId,
                                    new MsgBuilder()
                                        .Text(JsonConvert.SerializeObject(TargetMsg.Message, Formatting.Indented)).Build()
                                );
                }
                else
                {
                    CommandLogger.Error($"Unproper Arg: Too Less Args, At Command <{Args.Command}>");
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
