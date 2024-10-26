using System;
using UndefinedBot.Net.NetWork;
using UndefinedBot.Net.Utils;

namespace UndefinedBot.Net.Command
{
    internal abstract class HelpCommand
    {
        private static readonly string CommandPrefix = Program.GetConfigManager().GetCommandPrefix();

        private static readonly Dictionary<string, string> HelpTextReference = new()
        {
            { "help" ,   string.Format("---------------help---------------\n指令列表：\n{0}help - 查看帮助\n{0}symmet - 图片、表情对称\n{0}hito - 随机一言\n{0}queto - 生成切片（入典）\n{0}invert - 图片色反\n{0}raw - 群u到底发的什么消息\n{0}mix - 混合Emoji\n使用{0}help+具体指令查看使用方法\ne.g. {0}help symmet",CommandPrefix) },
            { "symmet" , string.Format("---------------help---------------\n{0}symmet - 图片、表情对称\n使用方法：{0}symmet <对称方法> [表情/图片] 或用 {0}symmet <对称方法> 回复[表情/图片]，支持上下、下上、左右、右左\ne.g. {0}symmet 上下 [图片]",CommandPrefix) },
            { "hito" ,   string.Format("---------------help---------------\n{0}hito - 随机一言\n使用方法：{0}hito [一言类型]，不填类型则随机\ne.g. {0}hito b\n类型对照：\na - 动画\nb - 漫画\nc - 游戏\nd - 文学\ne - 原创\nf - 来自网络\ng - 其他\nh - 影视\ni - 诗词\nj - 网易云\nk - 哲学\nl - 抖机灵",CommandPrefix) },
            { "queto" ,  string.Format("---------------help---------------\n{0}queto - 生成切片（入典）\n使用方法：用{0}queto 回复想生成的消息\ne.g. {0}queto",CommandPrefix) },
            { "invert" , string.Format("---------------help---------------\n{0}invert - 图片色反\n使用方法：用{0}invert 回复想生成的消息\ne.g. {0}invert",CommandPrefix) },
            { "raw" ,    string.Format("---------------help---------------\n{0}raw - 群u到底发的什么东西\n使用方法：用{0}raw 回复想生成的消息\ne.g. {0}raw",CommandPrefix) },
            { "mix" ,    string.Format("---------------help---------------\n{0}mix - 混合Emoji\n使用方法：{0}mix Emoji1 Emoji2\ne.g. {0}mix 😀 😁",CommandPrefix) },
            //{ "" , string.Format("",CommandPrefix) },
        };
        public static async Task<bool> PrintHelpText(long GroupId, string Command)
        {
            if (HelpTextReference.TryGetValue(Command, out var txt))
            {
                await HttpApi.SendGroupMsg(
                    GroupId,
                    new MsgBuilder()
                        .Text(txt).Build()
                );
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
