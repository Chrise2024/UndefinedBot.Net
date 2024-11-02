using System;
using UndefinedBot.Net.Extra;
using UndefinedBot.Net.NetWork;
using UndefinedBot.Net.Utils;

namespace UndefinedBot.Net.Command.Content
{
    public class SymmetCommand : IBaseCommand
    {
        public string CommandName { get; } = "symmet";
        public string CommandDescription { get; private set; } = "{0}symmet - 图片、表情对称\n使用方法：{0}symmet <对称方法> [表情/图片] 或用 {0}symmet <对称方法> 回复[表情/图片]，支持上下、下上、左右、右左\ne.g. {0}symmet 上下 [图片]";
        public string CommandShortDescription { get; private set; } = "{0}symmet - 图片、表情对称";
        public Logger CommandLogger { get; private set; } = new("Command", "Undefined");
        public async Task Execute(ArgSchematics args)
        {
            if (args.Param.Count > 1)
            {
                string ImageCachePath;
                //ParamFormat: [Pattern] [ImageUrl]
                if (args.Param[1].StartsWith("http"))
                {
                    ImageCachePath = ImageConvert.GetConvertedImage(args.Param[1], ImageContentType.Url, args.Param[0]);
                }
                //ParamFormat: [MsgId] [Pattern]
                else
                {
                    ImageCachePath = ImageConvert.GetConvertedImage(args.Param[0], ImageContentType.MsgId, args.Param[1]);
                }
                if (ImageCachePath.Length == 0)
                {
                    CommandLogger.Error("Pic Convert Failed");
                    await Program.GetHttpApi().SendGroupMsg(
                        args.GroupId,
                        new MsgBuilder()
                            .Text("似乎转换不了").Build()
                    );
                }
                else
                {
                    await Program.GetHttpApi().SendGroupMsg(
                                args.GroupId,
                                new MsgBuilder()
                                    .Reply(args.MsgId)
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
