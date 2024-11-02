using System;
using System.Reflection;
using Newtonsoft.Json;
using UndefinedBot.Net.Utils;

namespace UndefinedBot.Net.Command
{
    public class CommandInitializer
    {
        private static readonly Assembly s_initializerAssembly = Assembly.GetExecutingAssembly();
        public static Dictionary<string,CommandPropertieSchematics> InitCommand()
        {
            Dictionary<string, CommandPropertieSchematics> CommandReference = [];
            IEnumerable<Type> Types = s_initializerAssembly.GetTypes().Where(t => t.Namespace == "UndefinedBot.Net.Command.Content" && t.IsClass);
            foreach (Type type in Types)
            {
                var CommandInstance = Activator.CreateInstance(type);
                if (CommandInstance != null)
                {
                    MethodInfo? InitMethod = type.GetMethod("Init");
                    MethodInfo? GetNameInfo = type.GetMethod("get_CommandName");
                    MethodInfo? GetDescriptionInfo = type.GetMethod("get_CommandDescription");
                    MethodInfo? GetShortDescriptionInfo = type.GetMethod("get_CommandShortDescription");
                    if (GetNameInfo?.Invoke(CommandInstance, null) is string CName && GetDescriptionInfo?.Invoke(CommandInstance, null) is string CDescription && GetShortDescriptionInfo?.Invoke(CommandInstance, null) is string CShortDescription)
                    {
                        InitMethod?.Invoke(CommandInstance, null);
                        CommandReference.Add(CName, new CommandPropertieSchematics(CName, CDescription, CShortDescription, CommandInstance));
                    }
                }
            }
            return CommandReference;
        }
    }
    public struct CommandPropertieSchematics(string name, string description, string shortDescription, object instance)
    {
        public string Name = name;
        public string Description = description;
        public string ShortDescription = shortDescription;
        public object Instance = instance;
    }
}
