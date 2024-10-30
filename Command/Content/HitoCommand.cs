using System;
using UndefinedBot.Net.NetWork;
using UndefinedBot.Net.Utils;

namespace UndefinedBot.Net.Command.Content
{
    public class HitoCommand
    {
        public string CommandName { get; } = "hito";
        public string CommandDescription { get; } = string.Format("---------------help---------------\n{0}hito - 随机一言\n使用方法：{0}hito [一言类型]，不填类型则随机\ne.g. {0}hito b\n类型对照：\na - 动画\nb - 漫画\nc - 游戏\nd - 文学\ne - 原创\nf - 来自网络\ng - 其他\nh - 影视\ni - 诗词\nj - 网易云\nk - 哲学\nl - 抖机灵", Program.GetConfigManager().GetCommandPrefix());

        private readonly Logger CommandLogger = new("Command", "Hito");
        public async Task Handle(ArgSchematics Args)
        {
            //ParamFormat: [Type]
            if (Args.Command.Equals(CommandName))
            {
                CommandLogger.Info("Command Triggered");
                HitokotoSchematics Hitokoto = await HttpApi.GetHitokoto(Args.Param.Count > 0 ? Args.Param[0] : "");
                if ((Hitokoto.Id ?? 0) != 0)
                {
                    await HttpApi.SendGroupMsg(
                            Args.GroupId,
                            new MsgBuilder()
                                .Text($"{Hitokoto.Hitokoto}\n---- {Hitokoto.Creator}").Build()
                        );
                }
                else
                {
                    CommandLogger.Error($"Get Hitokoto Failed");
                    await HttpApi.SendGroupMsg(
                            Args.GroupId,
                            new MsgBuilder()
                                .Text("一言似乎迷路了").Build()
                        );
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
