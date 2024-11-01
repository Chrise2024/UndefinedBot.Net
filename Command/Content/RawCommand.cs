using System;
using Newtonsoft.Json;
using UndefinedBot.Net.NetWork;
using UndefinedBot.Net.Utils;

namespace UndefinedBot.Net.Command.Content
{
    public class RawCommand : IBaseCommand
    {
        public string CommandName { get; private set; } = "raw";
        public string CommandDescription { get; private set; } = "{0}raw - 群u到底发的什么东西\n使用方法：用{0}raw 回复想生成的消息\ne.g. {0}raw";
        public string CommandShortDescription { get; private set; } = "{0}raw - 群u到底发的什么东西";
        public Logger CommandLogger { get; private set; } = new("Command", "Undefined");
        public async Task Execute(ArgSchematics args)
        {
            if (args.Param.Count > 0)
            {
                MsgBodySchematics TargetMsg = await HttpApi.GetMsg(args.Param[0]);
                await HttpApi.SendGroupMsg(
                                args.GroupId,
                                new MsgBuilder()
                                    .Text(JsonConvert.SerializeObject(TargetMsg.Message, Formatting.Indented)).Build()
                            );
            }
            else
            {
                CommandLogger.Error($"Unproper Arg: Too Less args, At Command <{args.Command}>");
            }
        }
        public async Task Handle(ArgSchematics args)
        {
            if (args.Command.Equals(CommandName))
            {
                CommandLogger.Info("Command Triggered");
                await Execute(args);
                CommandLogger.Info("Command Completed");
            }
        }
        public void Init()
        {
            CommandLogger = new("Command", CommandName);
            MsgHandler.GetCommandHandler().CommandEvent += Handle;
            CommandLogger.Info("Command Loaded");
        }
    }
}
