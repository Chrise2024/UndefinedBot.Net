using System;
using System.Reflection.Metadata;
using UndefinedBot.Net.Utils;

namespace UndefinedBot.Net.Command
{
    public interface IBaseCommand
    {
        public string CommandName { get; }
        public string CommandDescription { get; }
        public string CommandShortDescription { get; }
        public Logger CommandLogger { get; }
        public Task Handle(ArgSchematics args);
        public Task Execute(ArgSchematics args);
        public void Init();
    }
}
