using System;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using SkiaSharp;

namespace UndefinedBot.Net.Extra
{
    internal class TextRender
    {
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
        private static void DrawTextWithWrapping(SKCanvas canvas, string text, SKRect TextArea,int FontSize)
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
            int MaxTextElementCount = (int)Math.Floor(TextArea.Width / FontSize);
            List<List<string>> Lines = SplitString(text,FontSize,TextArea.Width,out List<float> LineWidth);
            float yOffset = (TextArea.Height / 2) - (Lines.Count * FontSize / 2);//0;
            for (int i = 0;i < Lines.Count; i++)
            {
                DrawLine(canvas, Lines[i], MaxTextElementCount, LineWidth[i], new SKPoint(DrawPosition.X, DrawPosition.Y + yOffset), PaintEmoji, PaintText);
                yOffset += FontSize;
                if (yOffset + FontSize > TextArea.Height)
                {
                    break;
                }
            }
            PaintEmoji.Dispose();
            PaintText.Dispose();
        }
        private static void DrawLine(SKCanvas canvas, List<string> LineText,int MaxTextElementCount,float LineWidth, SKPoint LinePosition, SKPaint PaintEmoji, SKPaint PaintText)
        {
            float FontSize = PaintText.TextSize;
            float LPos = LinePosition.X - LineWidth / 2 + FontSize / 2;
            foreach (string TE in LineText)
            {
                if (IsEmoji(TE))
                {
                    canvas.DrawText(TE, LPos, LinePosition.Y, PaintEmoji);
                    LPos += PaintEmoji.TextSize * 1.5F;
                }
                else if (IsASCII(TE))
                {
                    canvas.DrawText(TE, LPos, LinePosition.Y, PaintEmoji);
                    if (IsFullCode(TE))
                    {
                        LPos += PaintText.TextSize * 0.8F;
                    }
                    else
                    {
                        LPos += PaintText.TextSize * 0.6F;
                    }
                }
                else
                {
                    canvas.DrawText(TE, LPos, LinePosition.Y, PaintText);
                    LPos += PaintText.TextSize;
                }
            }
        }
        private static List<List<string>> SplitString(string input, int FontSize,float Width,out List<float> LW)
        {
            List<List<string>> output = [];
            List<string> tmp = [];
            LW = [];
            float CLength = FontSize;
            TextElementEnumerator ElementEnumerator = StringInfo.GetTextElementEnumerator(input);
            ElementEnumerator.Reset();
            while (ElementEnumerator.MoveNext())
            {
                string CTE = ElementEnumerator.GetTextElement();
                tmp.Add(CTE);
                if (IsEmoji(CTE))
                {
                    CLength += FontSize * 1.5F;
                }
                else if (IsASCII(CTE))
                {
                    if (IsFullCode(CTE))
                    {
                        CLength += FontSize * 0.8F;
                    }
                    else
                    {
                        CLength += FontSize * 0.6F;
                    }
                }
                else
                {
                    CLength += FontSize;
                }
                if (CLength > Width)
                {
                    LW.Add(CLength - FontSize);
                    output.Add(tmp);
                    tmp = new List<string>();
                    CLength = FontSize;
                }
            }
            if (tmp.Count > 0)
            {
                LW.Add(CLength - FontSize);
                output.Add(tmp);
            }
            return output;
        }
        private static bool IsEmoji(string TextElement)
        {
            UnicodeCategory UC = CharUnicodeInfo.GetUnicodeCategory(TextElement[0]);
            return UC == UnicodeCategory.OtherSymbol || UC == UnicodeCategory.ModifierSymbol ||
                   UC == UnicodeCategory.PrivateUse  || UC == UnicodeCategory.Surrogate;
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
        private static List<char> FullList = new() {'@','#','$','&'};
        private static bool IsFullCode(char IC)
        {
            return FullList.Contains(IC);
        }
        private static bool IsFullCode(string TextElement)
        {
            if (TextElement.Length != 1)
            {
                return false;
            }
            else
            {
                return FullList.Contains(TextElement[0]);
            }
        }
    }
}
