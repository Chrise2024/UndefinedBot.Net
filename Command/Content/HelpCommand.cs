using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UndefinedBot.Net.NetWork;
using UndefinedBot.Net.Utils;

namespace UndefinedBot.Net.Command.Content
{
    public class HelpCommand : IBaseCommand
    {
        public string CommandName { get;private set; } = "help";
        public string CommandDescription { get; private set; } = "{0}help - 帮助信息\n使用方法：{0}help\ne.g. {0}help";
        public string CommandShortDescription { get; private set; } = "{0}help - 帮助信息";
        public Logger CommandLogger { get; private set; } = new("Command","Undefined");

        private readonly string _commandPrefix = Program.GetConfigManager().GetCommandPrefix();

        private string _commonHelpText = "";
        public async Task Execute(ArgSchematics args)
        {
            if (args.Param.Count > 0)
            {
                if (Program.GetCommandReference().TryGetValue(args.Param[0], out var Prop))
                {
                    await HttpApi.SendGroupMsg(
                        args.GroupId,
                        new MsgBuilder()
                            .Text(string.Format(Prop.Description, _commandPrefix)).Build()
                    );
                }
                else
                {
                    await HttpApi.SendGroupMsg(
                        args.GroupId,
                        new MsgBuilder()
                            .Text("咦，没有这个指令").Build()
                    );
                    CommandLogger.Warn($"Command Not Found: <{args.Param[0]}>");
                }
            }
            else
            {
                if (_commonHelpText.Length == 0)
                {
                    string CommandText = "";
                    List<CommandPropertieSchematics> CMDProp = [.. Program.GetCommandReference().Values];
                    CMDProp.ForEach(cmd =>
                    {
                        CommandText += cmd.ShortDescription + "\n";
                    });
                    _commonHelpText = "---------------help---------------\n指令列表：\n" +
                        CommandText +
                        "使用#help+具体指令查看使用方法\ne.g. #help symmet";
                    _commonHelpText = string.Format(_commonHelpText, _commandPrefix);
                }
                await HttpApi.SendGroupMsg(
                            args.GroupId,
                            new MsgBuilder()
                                .Text(_commonHelpText).Build()
                        );
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
            MsgHandler.GetCommandHandler().CommandEvent += Handle;
            CommandLogger.Info("Command Loaded");
        }
    }
}
