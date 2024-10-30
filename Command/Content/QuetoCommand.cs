using System;
using UndefinedBot.Net.Extra;
using UndefinedBot.Net.NetWork;
using UndefinedBot.Net.Utils;

namespace UndefinedBot.Net.Command.Content
{
    public class QuetoCommand
    {
        public string CommandName { get; } = "queto";
        public string CommandDescription { get; } = string.Format("---------------help---------------\n{0}queto - 生成切片（入典）\n使用方法：用{0}queto 回复想生成的消息\ne.g. {0}queto", Program.GetConfigManager().GetCommandPrefix());

        private readonly Logger CommandLogger = new("Command", "Queto");
        public async Task Handle(ArgSchematics Args)
        {
            //ParamFormat: [TargetMsg]
            if (Args.Command.Equals(CommandName))
            {
                CommandLogger.Info("Command Triggered");
                if (Args.Param.Count > 0)
                {
                    string ImageCachePath = Queto.GenQuetoImage(Args.Param[0]);
                    if (ImageCachePath.Length == 0)
                    {
                        CommandLogger.Error("Generate Failed");
                        await HttpApi.SendGroupMsg(
                            Args.GroupId,
                            new MsgBuilder()
                                .Text("生成出错了").Build()
                        );
                    }
                    else
                    {
                        await HttpApi.SendGroupMsg(
                                Args.GroupId,
                                new MsgBuilder()
                                    .Image(ImageCachePath, ImageSendType.LocalFile, ImageSubType.Normal).Build()
                            );
                        FileIO.SafeDeleteFile(ImageCachePath);
                    }
                }
                else
                {
                    CommandLogger.Error("Unproper Arg: Too Less Args");
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
