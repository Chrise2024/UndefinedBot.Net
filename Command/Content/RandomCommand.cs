using System;
using UndefinedBot.Net.Extra;
using UndefinedBot.Net.NetWork;
using UndefinedBot.Net.Utils;

namespace UndefinedBot.Net.Command.Content
{
    public class RandomCommand : IBaseCommand
    {
        public string CommandName { get; private set; } = "random";
        public string CommandDescription { get; private set; } = "{0}random - 随机图片\n使用方法：{0}random PicType\ne.g. {0}random acg\nacg - ACG\ndog - 哈基汪\ncat - 哈基米\nfox - 狐狸\nstar - 星空\nbg - 壁纸";
        public string CommandShortDescription { get; private set; } = "{0}random - 随机图片";
        public Logger CommandLogger { get; private set; } = new("Command", "Undefined");
        public async Task Execute(ArgSchematics args)
        {
            if (args.Param.Count > 0)
            {
                string OutUrl = RandomPicture.GetRandomContent(args.Param[0]);
                if (OutUrl.Length > 0)
                {
                    await HttpApi.SendGroupMsg(
                                    args.GroupId,
                                    new MsgBuilder()
                                        .Image(OutUrl, ImageSendType.Url).Build()
                                );
                }
                else
                {
                    await HttpApi.SendGroupMsg(
                            args.GroupId,
                            new MsgBuilder()
                                .Text("呃啊，图片迷路了").Build()
                        );
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
