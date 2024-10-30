using System;
using UndefinedBot.Net.Extra;
using UndefinedBot.Net.NetWork;
using UndefinedBot.Net.Utils;

namespace UndefinedBot.Net.Command.Content
{
    public class MixCommand
    {
        public string CommandName { get; } = "mix";
        public string CommandDescription { get; } = string.Format("---------------help---------------\n{0}mix - 混合Emoji\n使用方法：{0}mix Emoji1 Emoji2\ne.g. {0}mix 😀 😁", Program.GetConfigManager().GetCommandPrefix());

        private readonly Logger CommandLogger = new("Command", "Mix");
        public async Task Handle(ArgSchematics Args)
        {
            //ParamFormat: [Emoji1] [Emoji2] or [Emoji1Emoji2]
            if (Args.Command.Equals(CommandName))
            {
                CommandLogger.Info("Command Triggered");
                if (Args.Param.Count > 0)
                {
                    string MixRes = EmojiMix.MixEmoji(Args.Param);
                    if (MixRes.Length > 0)
                    {
                        await HttpApi.SendGroupMsg(
                                        Args.GroupId,
                                        new MsgBuilder()
                                            .Reply(Args.MsgId)
                                            .Image(MixRes, ImageSendType.Url).Build()
                                    );
                    }
                    else
                    {
                        await HttpApi.SendGroupMsg(
                                Args.GroupId,
                                new MsgBuilder()
                                    .Text("似乎不能混合").Build()
                            );
                    }
                }
                else
                {
                    CommandLogger.Error($"Unproper Arg: Too Less Args");
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
