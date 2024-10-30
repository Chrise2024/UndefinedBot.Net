using System;
using UndefinedBot.Net.Extra;
using UndefinedBot.Net.NetWork;
using UndefinedBot.Net.Utils;

namespace UndefinedBot.Net.Command.Content
{
    public class HistodayCommand
    {
        public string CommandName { get; } = "histoday";
        public string CommandDescription { get; } = string.Format("---------------help---------------\n{0}histoday - 历史上的今天\n使用方法：{0}histoday\ne.g. {0}histoday", Program.GetConfigManager().GetCommandPrefix());

        private readonly Logger CommandLogger = new("Command", "Histoday");
        public async Task Handle(ArgSchematics Args)
        {
            //ParamFormat: Any
            if (Args.Command.Equals(CommandName))
            {
                CommandLogger.Info("Command Triggered");
                string ImageCachePath = Histoday.GenHistodayImage();
                await HttpApi.SendGroupMsg(
                                Args.GroupId,
                                new MsgBuilder()
                                    .Image(ImageCachePath, ImageSendType.LocalFile, ImageSubType.Normal).Build()
                            );
                FileIO.SafeDeleteFile(ImageCachePath);
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
