using System;
using UndefinedBot.Net.Extra;
using UndefinedBot.Net.NetWork;
using UndefinedBot.Net.Utils;

namespace UndefinedBot.Net.Command.Content
{
    public class SymmetCommand:IBaseCommand
    {
        public string CommandName { get; } = "symmet";
        public string CommandDescription { get; } = string.Format("---------------help---------------\n{0}symmet - 图片、表情对称\n使用方法：{0}symmet <对称方法> [表情/图片] 或用 {0}symmet <对称方法> 回复[表情/图片]，支持上下、下上、左右、右左\ne.g. {0}symmet 上下 [图片]", Program.GetConfigManager().GetCommandPrefix());

        private readonly Logger CommandLogger = new("Command", "Symmet");
        public async Task Handle(ArgSchematics Args)
        {
            //ParamFormat: [MsgId] [Pattern] or [Pattern] [ImageUrl]
            if (Args.Command.Equals(CommandName))
            {
                CommandLogger.Info("Command Triggered");
                if (Args.Param.Count > 1)
                {
                    string ImageCachePath;
                    //ParamFormat: [Pattern] [ImageUrl]
                    if (Args.Param[1].StartsWith("http"))
                    {
                        ImageCachePath = ImageConvert.GetConvertedImage(Args.Param[1], ImageContentType.Url, Args.Param[0]);
                    }
                    //ParamFormat: [MsgId] [Pattern]
                    else
                    {
                        ImageCachePath = ImageConvert.GetConvertedImage(Args.Param[0], ImageContentType.MsgId, Args.Param[1]);
                    }
                    if (ImageCachePath.Length == 0)
                    {
                        CommandLogger.Error("Pic Convert Failed");
                        await HttpApi.SendGroupMsg(
                            Args.GroupId,
                            new MsgBuilder()
                                .Text("似乎转换不了").Build()
                        );
                    }
                    else
                    {
                        await HttpApi.SendGroupMsg(
                                    Args.GroupId,
                                    new MsgBuilder()
                                        .Reply(Args.MsgId)
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
