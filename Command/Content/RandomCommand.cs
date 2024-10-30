using System;
using UndefinedBot.Net.Extra;
using UndefinedBot.Net.NetWork;
using UndefinedBot.Net.Utils;

namespace UndefinedBot.Net.Command.Content
{
    public class RandomCommand
    {
        public string CommandName { get; } = "random";
        public string CommandDescription { get; } = string.Format("---------------help---------------\n{0}random - 随机图片\n使用方法：{0}random PicType\ne.g. {0}random acg\nacg - ACG\ndog - 哈基汪\ncat - 哈基米\nfox - 狐狸\nstar - 星空\nbg - 壁纸", Program.GetConfigManager().GetCommandPrefix());

        private readonly Logger CommandLogger = new("Command", "Random");
        public async Task Handle(ArgSchematics Args)
        {
            //ParamFormat: [RandomType]
            if (Args.Command.Equals(CommandName))
            {
                CommandLogger.Info("Command Triggered");
                if (Args.Param.Count > 0)
                {
                    string OutUrl = RandomPicture.GetRandomContent(Args.Param[0]);
                    if (OutUrl.Length > 0)
                    {
                        await HttpApi.SendGroupMsg(
                                        Args.GroupId,
                                        new MsgBuilder()
                                            .Image(OutUrl, ImageSendType.Url).Build()
                                    );
                    }
                    else
                    {
                        await HttpApi.SendGroupMsg(
                                Args.GroupId,
                                new MsgBuilder()
                                    .Text("呃啊，图片迷路了").Build()
                            );
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
