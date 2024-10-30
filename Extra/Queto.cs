using System;
using System.Globalization;
using System.Drawing;
using System.Drawing.Imaging;
using SkiaSharp;
using Newtonsoft.Json.Linq;
using UndefinedBot.Net.NetWork;
using UndefinedBot.Net.Utils;

namespace UndefinedBot.Net.Extra
{
    internal class Queto
    {
        public static string GenQuetoImage(string TargetMsgIdString)
        {
            MsgBodySchematics TargetMsg = HttpApi.GetMsg(Int32.TryParse(TargetMsgIdString, out int TargetMsgId) ? TargetMsgId : 0).Result;
            if ((TargetMsg.MessageId ?? 0) == 0)
            {
                return "";
            }
            else
            {
                string TargetMsgString = "";
                List<JObject> MsgSeq = TargetMsg.Message ?? [];
                foreach (JObject index in MsgSeq)
                {
                    if (index.Value<string>("type")?.Equals("text") ?? false)
                    {
                        string TText = index.Value<JObject>("data")?.Value<string>("text") ?? "";
                        if (TText.Length != 0 && !RegexProvider.GetEmptyStringRegex().IsMatch(TText))
                        {
                            TargetMsgString += TText;
                        }
                    }
                    else if (index.Value<string>("type")?.Equals("at") ?? false)
                    {
                        TargetMsgString += (index.Value<JObject>("data")?.Value<string>("name") ?? "@") + " ";
                    }
                    else if (index.Value<string>("type")?.Equals("face") ?? false)
                    {
                        string FId = index.Value<JObject>("data")?.Value<string>("id") ?? "";
                        TargetMsgString += (TextRender.QFaceReference.TryGetValue(FId, out var EmojiString) ? EmojiString : "");
                    }
                    else
                    {
                        continue;
                    }
                }
                long TargetUin = TargetMsg.Sender?.UserId ?? 0;
                GroupMemberSchematics CMember = HttpApi.GetGroupMember(TargetMsg.GroupId ?? 0, TargetUin).Result;
                string TargetName = $"@{CMember.Nickname ?? ""}";
                string ImCachePath = Path.Join(Program.GetProgramCahce(), $"{DateTime.Now:HH-mm-ss}.png");
                string QSplashPath = Path.Join(Program.GetProgramLocal(), "QSplash.png");
                string QTextCachePath = Path.Join(Program.GetProgramCahce(), $"Text_{DateTime.Now:HH-mm-ss}.png");
                string QNNCachePath = Path.Join(Program.GetProgramCahce(), $"NickName_{DateTime.Now:HH-mm-ss}.png");
                if (File.Exists(QSplashPath))
                {
                    Image CoverImage = Image.FromFile(QSplashPath);
                    Image TargetAvatar = HttpApi.GetQQAvatar(TargetUin).Result;
                    Bitmap bg = new(1200, 640);
                    Graphics g = Graphics.FromImage(bg);
                    g.DrawImage(TargetAvatar, 0, 0, 640, 640);
                    g.DrawImage(CoverImage, 0, 0, 1200, 640);
                    TextRender.GenTextImage(QTextCachePath, TargetMsgString, 96, 1800, 1350);
                    TextRender.GenTextImage(QNNCachePath, TargetName, 72, 1500, 120);
                    Bitmap TextBmp = new(QTextCachePath);
                    Bitmap NameBmp = new(QNNCachePath);
                    g.DrawImage(TextBmp, 550, 95, 600, 450);
                    g.DrawImage(NameBmp, 600, 600, 500, 40);
                    //g.DrawString(TargetMsgString, new Font("Noto Color Emoji", 40, FontStyle.Regular), new SolidBrush(Color.White), new RectangleF(440, 170, 800, 300), format);
                    //g.DrawString(TargetName, new Font("Noto Color Emoji", 24, FontStyle.Regular), new SolidBrush(Color.White), new RectangleF(690, 540, 300, 80), format);
                    bg.Save(ImCachePath, ImageFormat.Png);
                    TextBmp.Dispose();
                    NameBmp.Dispose();
                    g.Dispose();
                    bg.Dispose();
                    CoverImage.Dispose();
                    TargetAvatar.Dispose();
                    FileIO.SafeDeleteFile(QNNCachePath);
                    FileIO.SafeDeleteFile(QTextCachePath);
                    return ImCachePath;
                }
                else
                {
                    return "";
                }
            }
        }
    }
    internal class TextRender
    {
        public static readonly Dictionary<string, string> QFaceReference = new()
        {
            { "0" , "😯" },
            { "1" , "😖" },
            { "2" , "😍" },
            { "3" , "😦" },
            { "4" , "😎" },
            { "5" , "😭" },
            { "6" , "😊" },
            { "7" , "🤐" },
            { "8" , "😪" },
            { "9" , "😢" },
            { "10" , "😟" },
            { "11" , "😡" },
            { "12" , "😋" },
            { "13" , "😁" },
            { "14" , "🙂" },
            { "15" , "🙁" },
            { "16" , "😎" },
            { "18" , "😩" },
            { "19" , "🤮" },
            { "20" , "🤭" },
            { "21" , "☺" },
            { "22" , "😶" },
            { "23" , "😕" },
            { "24" , "😋" },
            { "25" , "🥱" },
            { "26" , "😨" },
            { "27" , "😥" },
            { "31" , "🤬" },
            { "32" , "🤔" },
            { "36" , "🌚" },
            { "37" , "💀" },
            { "46" , "🐷" },
            { "53" , "🍰" },
            { "56" , "🔪" },
            { "59" , "💩" },
            { "60" , "☕️" },
            { "63" , "🌹" },
            { "64" , "🥀" },
            { "66" , "❤️" },
            { "67" , "💔" },
            { "74" , "🌞" },
            { "75" , "🌛" },
            { "76" , "👍" },
            { "77" , "👎" },
            { "78" , "🤝" },
            { "79" , "✌" },
            { "97" , "😓" },
            { "98" , "🥱" },
            { "110" , "😧" },
            { "111" , "🥺" },
            { "112" , "🔪" },
            { "114" , "🏀" },
            { "116" , "👄" },
            { "120" , "✊" },
            { "123" , "👆" },
            { "124" , "👌" },
            { "146" , "💢" },
            { "147" , "🍭" },
            { "171" , "🍵" },
            { "177" , "🤧" },
            { "182" , "😂" },
            { "185" , "🦙" },
            { "186" , "👻" },
            { "273" , "🍋" },
            { "325" , "😱" }
        };
        public static void GenTextImage(string TempFilePath, string Text, int FontSize, int Width, int Height)
        {
            //Location:SKPoint->Bottom Center
            var Surface = SKSurface.Create(new SKImageInfo(Width, Height));
            SKCanvas Canvas = Surface.Canvas;
            Canvas.Clear(SKColors.Transparent);
            DrawTextWithWrapping(Canvas, Text, new SKRect(0, 0, Width, Height), FontSize);
            SKImage TempImage = Surface.Snapshot();
            SKData TempData = TempImage.Encode(SKEncodedImageFormat.Png, 100);
            FileStream TempFileStream = File.OpenWrite(TempFilePath);
            if (TempFileStream != null)
            {
                TempData.SaveTo(TempFileStream);
                TempFileStream.Close();
            }
            TempData.Dispose();
            TempImage.Dispose();
            Canvas.Dispose();
            Surface.Dispose();
        }
        private static void DrawTextWithWrapping(SKCanvas canvas, string text, SKRect TextArea, int FontSize)
        {
            SKPaint PaintEmoji = new SKPaint
            {
                Typeface = SKTypeface.FromFamilyName("Segoe UI Emoji"),
                TextSize = FontSize,
                Color = SKColors.White,
                IsAntialias = true,
                StrokeWidth = 1,
                TextAlign = SKTextAlign.Center,
                IsLinearText = true,
            };
            SKPaint PaintText = new SKPaint
            {
                Typeface = SKTypeface.FromFamilyName("Simsun"),
                TextSize = FontSize,
                Color = SKColors.White,
                IsAntialias = true,
                StrokeWidth = 1,
                TextAlign = SKTextAlign.Center,
                IsLinearText = true,
            };
            SKPoint DrawPosition = new(TextArea.Width / 2, FontSize);
            List<List<string>> Lines = SplitString(text, FontSize, TextArea.Width - FontSize * 1.5F, PaintText);
            float yOffset = (TextArea.Height / 2) - (Lines.Count * FontSize / 2);
            for (int i = 0; i < Lines.Count; i++)
            {
                DrawLine(canvas, Lines[i], new SKPoint(DrawPosition.X, DrawPosition.Y + yOffset), PaintEmoji, PaintText);
                yOffset += FontSize;
                if (yOffset + FontSize > TextArea.Height)
                {
                    break;
                }
            }
            PaintEmoji.Dispose();
            PaintText.Dispose();
        }
        private static void DrawLine(SKCanvas canvas, List<string> LineText, SKPoint LinePosition, SKPaint PaintEmoji, SKPaint PaintText)
        {
            float FontSize = PaintText.TextSize;
            float LineWidth = 0;
            foreach (string index in LineText)
            {
                LineWidth += IsEmoji(index) ? PaintEmoji.MeasureText(index) : PaintText.MeasureText(index);
            }
            float LPos = LinePosition.X - LineWidth / 2 + FontSize / 2;
            foreach (string TE in LineText)
            {
                if (IsEmoji(TE))
                {
                    canvas.DrawText(TE, LPos + PaintEmoji.MeasureText(TE) / 2, LinePosition.Y, PaintEmoji);
                    LPos += PaintEmoji.MeasureText(TE);
                }
                else
                {
                    canvas.DrawText(TE, LPos + PaintText.MeasureText(TE) / 2, LinePosition.Y, PaintText);
                    LPos += PaintText.MeasureText(TE);
                }
            }
        }
        private static List<List<string>> SplitString(string input, int FontSize, float Width, SKPaint CurrentPaint)
        {
            Width -= FontSize;
            List<List<string>> output = [];
            List<string> TempLine = [];
            List<string> Elements = [];
            float CLength = 0;
            TextElementEnumerator ElementEnumerator = StringInfo.GetTextElementEnumerator(input);
            ElementEnumerator.Reset();
            while (ElementEnumerator.MoveNext())
            {
                Elements.Add(ElementEnumerator.GetTextElement());
            }
            if (Elements.Count > 1)
            {
                string TempString = Elements[0];
                for (int index = 1; index < Elements.Count; index++)
                {
                    if (IsEmoji(Elements[index - 1]) == IsEmoji(Elements[index]))
                    {
                        TempString += Elements[index];
                    }
                    else
                    {
                        TempLine.Add(TempString);
                        TempString = Elements[index];
                    }
                    if (IsEmoji(Elements[index - 1]))
                    {
                        CLength += CurrentPaint.TextSize * 1.5F;
                    }
                    else
                    {
                        CLength += CurrentPaint.MeasureText(Elements[index - 1]);
                    }
                    if (CLength > Width)
                    {
                        if (TempString.Length > 0)
                        {
                            TempLine.Add(TempString);
                            TempString = "";
                        }
                        output.Add(TempLine);
                        TempLine = [];
                        CLength = 0;
                    }
                }
                if (TempString.Length > 0)
                {
                    TempLine.Add(TempString);
                    output.Add(TempLine);
                }
                return output;
            }
            else
            {
                return [[input]];
            }
        }
        private static bool IsEmoji(string TextElement)
        {
            UnicodeCategory UC = CharUnicodeInfo.GetUnicodeCategory(TextElement.Length > 0 ? TextElement[0] : ' ');
            return UC == UnicodeCategory.OtherSymbol || UC == UnicodeCategory.ModifierSymbol ||
                   UC == UnicodeCategory.PrivateUse || UC == UnicodeCategory.Surrogate;
        }
        private static bool IsASCII(char IC)
        {
            return IC >= 0x00 && IC < 0xFF;
        }
        private static bool IsASCII(string TextElement)
        {
            if (TextElement.Length != 1)
            {
                return false;
            }
            else
            {
                return TextElement[0] >= 0x00 && TextElement[0] < 0xFF;
            }
        }
    }
}
