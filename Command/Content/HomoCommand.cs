using System;
using UndefinedBot.Net.Extra;
using UndefinedBot.Net.NetWork;
using UndefinedBot.Net.Utils;

namespace UndefinedBot.Net.Command.Content
{
    public class HomoCommand : IBaseCommand
    {
        public string CommandName { get; private set; } = "homo";
        public string CommandDescription { get; private set; } = "{0}homo - 恶臭数字论证\n使用方法：{0}homo number\ne.g. {0}homo 10086";
        public string CommandShortDescription { get; private set; } = "{0}homo - 恶臭数字论证";
        public Logger CommandLogger { get; private set; } = new("Command", "Undefined");
        public async Task Execute(ArgSchematics args)
        {
            if (args.Param.Count > 0)
            {
                string Res = Homo.Homoize(args.Param[0], out bool Status);
                if (Status)
                {

                    await HttpApi.SendGroupMsg(
                                        args.GroupId,
                                        new MsgBuilder()
                                            //.Reply(args.MsgId)
                                            .Text($"{args.Param[0]} = {Res}").Build()
                                    );
                }
                else
                {
                    await HttpApi.SendGroupMsg(
                                        args.GroupId,
                                        new MsgBuilder()
                                            //.Reply(args.MsgId)
                                            .Text($"{Res}").Build()
                                    );
                }
            }
            else
            {
                CommandLogger.Error($"Unproper Arg: Too Less args, At Command <{args.Command}>");
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
