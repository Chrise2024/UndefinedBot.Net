using System;
using System.Text;
using UndefinedBot.Net.NetWork;

namespace UndefinedBot.Net.Extra
{
    internal class EmojiMix
    {
        public static int GetEmojiUnicodePoint(string EmojiString)
        {
            try{
                if (EmojiString.Length == 1)
                {
                    Rune ER = new(EmojiString[0]);
                    return ER.Value;
                }
                else if (EmojiString.Length > 1)
                {
                    Rune ER = new(EmojiString[0], EmojiString[1]);
                    return ER.Value;
                }
               else
                {
                    return 0;
                }
            }
            catch
            {
                return 0;
            }
        }
        public static string MixEmoji(string Emoji1,string Emoji2)
        {
            int E1CP = GetEmojiUnicodePoint(Emoji1);
            int E2CP = GetEmojiUnicodePoint(Emoji2);
            string TUrl = $"https://www.gstatic.com/android/keyboard/emojikitchen/20201001/u{E1CP:X}/u{E1CP:X}_u{E2CP:X}.png".ToLower();
            byte[] Res = HttpService.GetBinary(TUrl).Result;
            if (Res.Length == 0 || Res[0] != 0x89)
            {
                return "";
            }
            else
            {
                return TUrl;
            }
        }
    }
}
