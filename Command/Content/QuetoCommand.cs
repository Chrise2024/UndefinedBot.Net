using System;
using UndefinedBot.Net.Extra;
using UndefinedBot.Net.NetWork;
using UndefinedBot.Net.Utils;

namespace UndefinedBot.Net.Command.Content
{
    public class QuetoCommand : IBaseCommand
    {
        public string CommandName { get; private set; } = "queto";
        public string CommandDescription { get; private set; } = "{0}queto - 生成切片（入典）\n使用方法：用{0}queto 回复想生成的消息\ne.g. {0}queto";
        public string CommandShortDescription { get; private set; } = "{0}queto - 生成切片（入典）";
        public Logger CommandLogger { get; private set; } = new("Command", "Undefined");
        public async Task Execute(ArgSchematics args)
        {
            if (args.Param.Count > 0)
            {
                string ImageCachePath = Queto.GenQuetoImage(args.Param[0]);
                if (ImageCachePath.Length == 0)
                {
                    CommandLogger.Error("Generate Failed");
                    await Program.GetHttpApi().SendGroupMsg(
                        args.GroupId,
                        new MsgBuilder()
                            .Text("生成出错了").Build()
                    );
                }
                else
                {
                    await Program.GetHttpApi().SendGroupMsg(
                            args.GroupId,
                            new MsgBuilder()
                                .Image(ImageCachePath, ImageSendType.LocalFile, ImageSubType.Normal).Build()
                        );
                    FileIO.SafeDeleteFile(ImageCachePath);
                }
            }
            else
            {
                CommandLogger.Error("Unproper Arg: Too Less args");
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
            CommandHandler.CommandEvent += Handle;
            CommandLogger.Info("Command Loaded");
        }
    }
}
