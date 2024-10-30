using System;
using UndefinedBot.Net.Extra;
using UndefinedBot.Net.NetWork;
using UndefinedBot.Net.Utils;

namespace UndefinedBot.Net.Command.Content
{
    public class HomoCommand
    {
        public string CommandName { get; } = "homo";
        public string CommandDescription { get; } = string.Format("---------------help---------------\n{0}homo - 恶臭数字论证\n使用方法：{0}homo number\ne.g. {0}homo 10086", Program.GetConfigManager().GetCommandPrefix());

        private readonly Logger CommandLogger = new("Command", "Homo");
        public async Task Handle(ArgSchematics Args)
        {
            //ParamFormat: [Text]
            if (Args.Command.Equals(CommandName))
            {
                CommandLogger.Info("Command Triggered");
                if (Args.Param.Count > 0)
                {
                    string Res = Homo.Homoize(Args.Param[0], out bool Status);
                    if (Status)
                    {

                        await HttpApi.SendGroupMsg(
                                            Args.GroupId,
                                            new MsgBuilder()
                                                //.Reply(Args.MsgId)
                                                .Text($"{Args.Param[0]} = {Res}").Build()
                                        );
                    }
                    else
                    {
                        await HttpApi.SendGroupMsg(
                                            Args.GroupId,
                                            new MsgBuilder()
                                                //.Reply(Args.MsgId)
                                                .Text($"{Res}").Build()
                                        );
                    }
                }
                else
                {
                    CommandLogger.Error($"Unproper Arg: Too Less Args, At Command <{Args.Command}>");
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
