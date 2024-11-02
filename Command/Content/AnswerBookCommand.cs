using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UndefinedBot.Net.Extra;
using UndefinedBot.Net.NetWork;
using UndefinedBot.Net.Utils;

namespace UndefinedBot.Net.Command.Content
{
    public class AnswerBookCommand : IBaseCommand
    {
        public string CommandName { get; private set; } = "answerbook";
        public string CommandDescription { get; private set; } = "{0}answerbook - 答案之书\n使用方法：{0}answerbook\ne.g. {0}answerbook";
        public string CommandShortDescription { get; private set; } = "{0}answerbook - 答案之书";
        public Logger CommandLogger { get; private set; } = new("Command", "AnswerBook");
        public async Task Execute(ArgSchematics args)
        {
            await Program.GetHttpApi().SendGroupMsg(
                            args.GroupId,
                            new MsgBuilder()
                                .Reply(args.MsgId)
                                .Text(AnswerBook.GetAnswer()).Build()
                        );
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
