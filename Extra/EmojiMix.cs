using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;
using UndefinedBot.Net.NetWork;

namespace UndefinedBot.Net.Extra
{
    public class EmojiMix
    {
        public static int GetEmojiUnicodePoint(string EmojiString)
        {
            
            try{
                if (EmojiString.Length > 0)
                {
                    StringRuneEnumerator SRE = EmojiString.EnumerateRunes();
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
        public static string MixEmoji(List<string> EmojiStringArray)
        {
            if (EmojiStringArray.Count == 1)
            {
                List<string> LineElement = [];
                TextElementEnumerator ElementEnumerator = StringInfo.GetTextElementEnumerator(EmojiStringArray[0]);
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
                    string TUrlN = $"https://www.gstatic.com/android/keyboard/emojikitchen/20201001/u{E1CP:X}/u{E1CP:X}_u{E2CP:X}.png".ToLower();
                    string TUrlR = $"https://www.gstatic.com/android/keyboard/emojikitchen/20201001/u{E2CP:X}/u{E2CP:X}_u{E1CP:X}.png".ToLower();
                    byte[] Res = HttpRequest.GetBinary(TUrlN).Result;
                    if (Res.Length == 0 || Res[0] != 0x89)
                    {
                        Res = HttpRequest.GetBinary(TUrlR).Result;
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
            else if (EmojiStringArray.Count > 1)
            {
                int E1CP = GetEmojiUnicodePoint(EmojiStringArray[0]);
                int E2CP = GetEmojiUnicodePoint(EmojiStringArray[1]);
                string TUrlN = $"https://www.gstatic.com/android/keyboard/emojikitchen/20201001/u{E1CP:X}/u{E1CP:X}_u{E2CP:X}.png".ToLower();
                string TUrlR = $"https://www.gstatic.com/android/keyboard/emojikitchen/20201001/u{E2CP:X}/u{E2CP:X}_u{E1CP:X}.png".ToLower();
                byte[] Res = HttpRequest.GetBinary(TUrlN).Result;
                if (Res.Length == 0 || Res[0] != 0x89)
                {
                    Res = HttpRequest.GetBinary(TUrlR).Result;
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
        private static bool IsEmoji(string TextElement)
        {
            UnicodeCategory UC = CharUnicodeInfo.GetUnicodeCategory(TextElement.Length > 0 ? TextElement[0] : ' ');
            return UC == UnicodeCategory.OtherSymbol || UC == UnicodeCategory.ModifierSymbol ||
                   UC == UnicodeCategory.PrivateUse || UC == UnicodeCategory.Surrogate;
        }
    }
}
