using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;
using UndefinedBot.Net.NetWork;

namespace UndefinedBot.Net.Extra
{
    public class EmojiMix
    {
        public static int GetEmojiUnicodePoint(string emojiString)
        {
            
            try{
                if (emojiString.Length > 0)
                {
                    StringRuneEnumerator SRE = emojiString.EnumerateRunes();
                    foreach (Rune R in SRE)
                    {
                        return R.Value;
                    }
                    return 0;
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
        public static string MixEmoji(List<string> emojiStringArray)
        {
            if (emojiStringArray.Count == 1)
            {
                List<string> LineElement = [];
                TextElementEnumerator ElementEnumerator = StringInfo.GetTextElementEnumerator(emojiStringArray[0]);
                ElementEnumerator.Reset();
                while (ElementEnumerator.MoveNext())
                {
                    string CurrentElement = ElementEnumerator.GetTextElement();
                    if (IsEmoji(CurrentElement))
                    {
                        LineElement.Add(CurrentElement);
                    }
                }
                if (LineElement.Count > 1)
                {

                    int E1CP = GetEmojiUnicodePoint(LineElement[0]);
                    int E2CP = GetEmojiUnicodePoint(LineElement[1]);
                    string TUrlN = $"https://www.gstatic.com/android/keyboard/emojikitchen/20201001/u{E1CP:x}/u{E1CP:x}_u{E2CP:x}.png";
                    string TUrlR = $"https://www.gstatic.com/android/keyboard/emojikitchen/20201001/u{E2CP:x}/u{E2CP:x}_u{E1CP:x}.png";
                    byte[] Res = Program.GetHttpRequest().GetBinary(TUrlN).Result;
                    if (Res.Length == 0 || Res[0] != 0x89)
                    {
                        Res = Program.GetHttpRequest().GetBinary(TUrlR).Result;
                        if (Res.Length == 0 || Res[0] != 0x89)
                        {
                            return "";
                        }
                        else
                        {
                            return TUrlR;
                        }
                    }
                    else
                    {
                        return TUrlN;
                    }
                }
                else
                {
                    return "";
                }
            }
            else if (emojiStringArray.Count > 1)
            {
                int E1CP = GetEmojiUnicodePoint(emojiStringArray[0]);
                int E2CP = GetEmojiUnicodePoint(emojiStringArray[1]);
                string TUrlN = $"https://www.gstatic.com/android/keyboard/emojikitchen/20201001/u{E1CP:X}/u{E1CP:X}_u{E2CP:X}.png".ToLower();
                string TUrlR = $"https://www.gstatic.com/android/keyboard/emojikitchen/20201001/u{E2CP:X}/u{E2CP:X}_u{E1CP:X}.png".ToLower();
                byte[] Res = Program.GetHttpRequest().GetBinary(TUrlN).Result;
                if (Res.Length == 0 || Res[0] != 0x89)
                {
                    Res = Program.GetHttpRequest().GetBinary(TUrlR).Result;
                    if (Res.Length == 0 || Res[0] != 0x89)
                    {
                        return "";
                    }
                    else
                    {
                        return TUrlR;
                    }
                }
                else
                {
                    return TUrlN;
                }
            }
            else
            {
                return "";
            }
        }
        private static bool IsEmoji(string textElement)
        {
            UnicodeCategory UC = CharUnicodeInfo.GetUnicodeCategory(textElement.Length > 0 ? textElement[0] : ' ');
            return UC == UnicodeCategory.OtherSymbol || UC == UnicodeCategory.ModifierSymbol ||
                   UC == UnicodeCategory.PrivateUse || UC == UnicodeCategory.Surrogate;
        }
    }
}
