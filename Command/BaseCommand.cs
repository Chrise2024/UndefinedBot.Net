using System;
using UndefinedBot.Net.Utils;

namespace UndefinedBot.Net.Command
{
    public interface IBaseCommand
    {
        string CommandName { get; }
        string CommandDescription { get; }
        public Task Handle(ArgSchematics Args);
        public void Init();
    }
}
