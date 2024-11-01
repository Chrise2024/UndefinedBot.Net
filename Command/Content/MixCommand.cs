using System;
using UndefinedBot.Net.Extra;
using UndefinedBot.Net.NetWork;
using UndefinedBot.Net.Utils;

namespace UndefinedBot.Net.Command.Content
{
    public class MixCommand : IBaseCommand
    {
        public string CommandName { get; private set; } = "mix";
        public string CommandDescription { get; private set; } = "{0}mix - 混合Emoji\n使用方法：{0}mix Emoji1 Emoji2\ne.g. {0}mix 😀 😁";
        public string CommandShortDescription { get; private set; } = "{0}mix - 混合Emoji";
        public Logger CommandLogger { get; private set; } = new("Command", "Undefined");
        public async Task Execute(ArgSchematics args)
        {
            if (args.Param.Count > 0)
            {
                string MixRes = EmojiMix.MixEmoji(args.Param);
                if (MixRes.Length > 0)
                {
                    await HttpApi.SendGroupMsg(
                                    args.GroupId,
                                    new MsgBuilder()
                                        .Reply(args.MsgId)
                                        .Image(MixRes, ImageSendType.Url).Build()
                                );
                }
                else
                {
                    await HttpApi.SendGroupMsg(
                            args.GroupId,
                            new MsgBuilder()
                                .Text("似乎不能混合").Build()
                        );
                }
            }
            else
            {
                CommandLogger.Error($"Unproper Arg: Too Less args");
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
