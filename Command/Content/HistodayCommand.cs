using System;
using UndefinedBot.Net.Extra;
using UndefinedBot.Net.NetWork;
using UndefinedBot.Net.Utils;

namespace UndefinedBot.Net.Command.Content
{
    public class HistodayCommand : IBaseCommand
    {
        public string CommandName { get; private set; } = "histoday";
        public string CommandDescription { get; private set; } = "{0}histoday - 历史上的今天\n使用方法：{0}histoday\ne.g. {0}histoday";
        public string CommandShortDescription { get; private set; } = "{0}histoday - 历史上的今天";
        public Logger CommandLogger { get; private set; } = new("Command", "Undefined");
        public async Task Execute(ArgSchematics args)
        {
            string ImageCachePath = Histoday.GenHistodayImage();
            await Program.GetHttpApi().SendGroupMsg(
                            args.GroupId,
                            new MsgBuilder()
                                .Image(ImageCachePath, ImageSendType.LocalFile, ImageSubType.Normal).Build()
                        );
            FileIO.SafeDeleteFile(ImageCachePath);
            CommandLogger.Info("Command Completed");
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