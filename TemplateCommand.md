```CSharp
using System;

namespace UndefinedBot.Net
{
    public class HelpCommand : IBaseCommand
    {
        public string CommandName { get;private set; } = "Base";
        public string CommandDescription { get; private set; } = "Base";
        public string CommandShortDescription { get; private set; } = "Base";
        public Logger CommandLogger { get; private set; } = new("Command","Undefined");
        public async Task Execute(ArgSchematics args)
        {
		    
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
```
