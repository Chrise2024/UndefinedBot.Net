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
            List<List<string>> Lines = SplitString(text,FontSize,TextArea.Width - FontSize * 1.5F, PaintText);
            float yOffset = (TextArea.Height / 2) - (Lines.Count * FontSize / 2);//0;
            for (int i = 0;i < Lines.Count; i++)
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
        private static void DrawLine(SKCanvas canvas, List<string> LineText,SKPoint LinePosition, SKPaint PaintEmoji, SKPaint PaintText)
        {
            //Console.WriteLine(JsonConvert.SerializeObject(LineText));
            float FontSize = PaintText.TextSize;
            float LineWidth = 0;
            foreach (string index in LineText)
            {
                LineWidth += IsEmoji(index) ? PaintEmoji.MeasureText(index) : PaintText.MeasureText(index);
            }
            float LPos = LinePosition.X - LineWidth / 2 + FontSize / 2;
            //Console.WriteLine(LineWidth);
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
        private static List<List<string>> SplitString(string input, int FontSize,float Width, SKPaint CurrentPaint)
        {
            Width -= FontSize;
            List<List<string>> output = [];
            List<string> TempLine = [];
            List<string> Elements = [];
            float CLength = 0;
            //float TempLength = 0;
            TextElementEnumerator ElementEnumerator = StringInfo.GetTextElementEnumerator(input);
            ElementEnumerator.Reset();
            while (ElementEnumerator.MoveNext())
            {
                Elements.Add(ElementEnumerator.GetTextElement());
            }
            if (Elements.Count > 1)
            {
                string TempString = Elements[0];
                string CurrentElement = Elements[0];
                for (int index = 1; index < Elements.Count; index++)
                {
                    //Console.WriteLine($"{Elements[index]}-{IsEmoji(Elements[index])}");
                    if (IsEmoji(CurrentElement) == IsEmoji(Elements[index]))
                    {
                        TempString += Elements[index];
                        CurrentElement = Elements[index];
                    }
                    else
                    {
                        //TempString += CurrentElement;
                        TempLine.Add(TempString);
                        TempString = Elements[index];
                        CurrentElement = Elements[index];
                    }
                    if (IsEmoji(CurrentElement))
                    {
                        CLength += CurrentPaint.TextSize * 1.5F;
                    }
                    else
                    {
                        CLength += CurrentPaint.MeasureText(CurrentElement);
                    }
                    //Console.WriteLine(TempString);
                    if (CLength > Width)
                    {
                        if (TempString.Length > 0)
                        {
                            TempLine.Add(TempString);
                            TempString = "";
                            //CurrentElement = Elements[index];
                        }
                        output.Add(TempLine);
                        TempLine = [];
                        CLength = 0;
                    }
                }
                if (TempString.Length > 0)
                {
                    output.Add([TempString]);
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
    }
}
