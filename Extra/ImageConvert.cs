using System;
using System.Drawing;
using System.Drawing.Imaging;
using SkiaSharp;
using ImageMagick;
using ImageMagick.Drawing;
using UndefinedBot.Net.NetWork;
using UndefinedBot.Net.Utils;
using UndefinedBot.Net.Command;

namespace UndefinedBot.Net.Extra
{
    public enum ImageContentType
    {
        Url = 0,
        MsgId = 1,
    }
    public class ImageConvert
    {
        public static string GetConvertedImage(string imageContent, ImageContentType contentType, string convertMethod = "L")
        {
            Image? Im;
            MemoryStream? Ms;
            if (contentType == ImageContentType.Url)
            {
                byte[] ImageBytes = Program.GetHttpRequest().GetBinary(imageContent).Result;
                Ms = new MemoryStream(ImageBytes);
                Im = Image.FromStream(Ms);
            }
            else
            {
                MsgBodySchematics TargetMsg = Program.GetHttpApi().GetMsg(imageContent).Result;
                if ((TargetMsg.MessageId ?? 0) == 0)
                {
                    return "";
                }
                else
                {
                    byte[] ImageBytes = Program.GetHttpRequest().GetBinary(CommandResolver.ExtractUrlFromMsg(TargetMsg)).Result;
                    Ms = new MemoryStream(ImageBytes);
                    Im = Image.FromStream(Ms);
                }
            }
            if (Im != null)
            {
                if (Im.RawFormat.Equals(ImageFormat.Gif))
                {
                    string ImCachePath = Path.Join(Program.GetProgramCahce(), $"{DateTime.Now:HH-mm-ss}.gif");
                    MagickImageCollection ResultImage = GifConvert.GifTransform(Im, convertMethod);
                    if (ResultImage.Count > 0)
                    {
                        ResultImage.Write(ImCachePath);
                        ResultImage.Dispose();
                        Im.Dispose();
                        Ms.Close();
                        return ImCachePath;
                    }
                    else
                    {
                        Im.Dispose();
                        Ms.Close();
                        return "";
                    }
                }
                else
                {
                    string ImCachePath = Path.Join(Program.GetProgramCahce(), $"{DateTime.Now:HH-mm-ss}.png");
                    Bitmap ResultImage = PicConvert.PicTransform(new Bitmap(Im), convertMethod);
                    if (ResultImage != null)
                    {
                        ResultImage.Save(ImCachePath, ImageFormat.Gif);
                        ResultImage.Dispose();
                        Im.Dispose();
                        Ms.Close();
                        return ImCachePath;
                    }
                    else
                    {
                        Im.Dispose();
                        Ms.Close();
                        return "";
                    }
                }
            }
            else
            {
                return "";
            }
        }
    }
    public class ImageSymmetry
    {
        public static Bitmap SymmetryL(Bitmap bmp)
        {
            int RW = bmp.Width / 2;
            Rectangle CropRect = new(0, 0, RW, bmp.Height);
            Bitmap CroppedImage = bmp.Clone(CropRect, bmp.PixelFormat);
            Bitmap Bg = new(bmp.Width, bmp.Height);
            Graphics g = Graphics.FromImage(Bg);
            g.DrawImage(CroppedImage, new System.Drawing.Point(0, 0));
            CroppedImage.RotateFlip(RotateFlipType.Rotate180FlipY);
            g.DrawImage(CroppedImage, new System.Drawing.Point(RW, 0));
            bmp.Dispose();
            CroppedImage.Dispose();
            return Bg;
        }
        public static Bitmap SymmetryR(Bitmap bmp)
        {
            int RW = bmp.Width / 2;
            Rectangle CropRect = new(RW, 0, RW, bmp.Height);
            Bitmap CroppedImage = bmp.Clone(CropRect, bmp.PixelFormat);
            Bitmap Bg = new(bmp.Width, bmp.Height);
            Graphics g = Graphics.FromImage(Bg);
            g.DrawImage(CroppedImage, new System.Drawing.Point(RW, 0));
            CroppedImage.RotateFlip(RotateFlipType.Rotate180FlipY);
            g.DrawImage(CroppedImage, new System.Drawing.Point(0, 0));
            bmp.Dispose();
            CroppedImage.Dispose();
            return Bg;
        }
        public static Bitmap SymmetryU(Bitmap bmp)
        {
            int RH = bmp.Height / 2;
            Rectangle CropRect = new(0, 0, bmp.Width, RH);
            Bitmap CroppedImage = bmp.Clone(CropRect, bmp.PixelFormat);
            Bitmap Bg = new(bmp.Width, bmp.Height);
            Graphics g = Graphics.FromImage(Bg);
            g.DrawImage(CroppedImage, new System.Drawing.Point(0, 0));
            CroppedImage.RotateFlip(RotateFlipType.Rotate180FlipX);
            g.DrawImage(CroppedImage, new System.Drawing.Point(0, RH));
            bmp.Dispose();
            CroppedImage.Dispose();
            return Bg;
        }
        public static Bitmap SymmetryD(Bitmap bmp)
        {
            int RH = bmp.Height / 2;
            Rectangle CropRect = new(0, RH, bmp.Width, RH);
            Bitmap CroppedImage = bmp.Clone(CropRect, bmp.PixelFormat);
            Bitmap Bg = new(bmp.Width, bmp.Height);
            Graphics g = Graphics.FromImage(Bg);
            g.DrawImage(CroppedImage, new System.Drawing.Point(0, RH));
            CroppedImage.RotateFlip(RotateFlipType.Rotate180FlipX);
            g.DrawImage(CroppedImage, new System.Drawing.Point(0, 0));
            bmp.Dispose();
            CroppedImage.Dispose();
            return Bg;
        }
        public static Bitmap SymmetryLU(Bitmap bmp)
        {
            int RW = bmp.Width / 2;
            int RH = bmp.Height / 2;
            Rectangle CropRect = new(0, 0, RW, RH);
            Bitmap CroppedImage = bmp.Clone(CropRect, bmp.PixelFormat);
            Bitmap Bg = new(bmp.Width, bmp.Height);
            Graphics g = Graphics.FromImage(Bg);
            g.DrawImage(CroppedImage, new System.Drawing.Point(0, 0));
            CroppedImage.RotateFlip(RotateFlipType.Rotate180FlipX);
            g.DrawImage(CroppedImage, new System.Drawing.Point(0, RH));
            CroppedImage.RotateFlip(RotateFlipType.Rotate180FlipY);
            g.DrawImage(CroppedImage, new System.Drawing.Point(RW, RH));
            CroppedImage.RotateFlip(RotateFlipType.Rotate180FlipX);
            g.DrawImage(CroppedImage, new System.Drawing.Point(RW, 0));
            bmp.Dispose();
            CroppedImage.Dispose();
            return Bg;
        }
        public static Bitmap SymmetryRU(Bitmap bmp)
        {
            int RW = bmp.Width / 2;
            int RH = bmp.Height / 2;
            Rectangle CropRect = new(RW, 0, RW, RH);
            Bitmap CroppedImage = bmp.Clone(CropRect, bmp.PixelFormat);
            Bitmap Bg = new(bmp.Width, bmp.Height);
            Graphics g = Graphics.FromImage(Bg);
            g.DrawImage(CroppedImage, new System.Drawing.Point(RW, 0));
            CroppedImage.RotateFlip(RotateFlipType.Rotate180FlipX);
            g.DrawImage(CroppedImage, new System.Drawing.Point(RW, RH));
            CroppedImage.RotateFlip(RotateFlipType.Rotate180FlipY);
            g.DrawImage(CroppedImage, new System.Drawing.Point(0, RH));
            CroppedImage.RotateFlip(RotateFlipType.Rotate180FlipX);
            g.DrawImage(CroppedImage, new System.Drawing.Point(0, 0));
            bmp.Dispose();
            CroppedImage.Dispose();
            return Bg;
        }
        public static Bitmap SymmetryLD(Bitmap bmp)
        {
            int RW = bmp.Width / 2;
            int RH = bmp.Height / 2;
            Rectangle CropRect = new(0, RH, RW, RH);
            Bitmap CroppedImage = bmp.Clone(CropRect, bmp.PixelFormat);
            Bitmap Bg = new(bmp.Width, bmp.Height);
            Graphics g = Graphics.FromImage(Bg);
            g.DrawImage(CroppedImage, new System.Drawing.Point(0, RH));
            CroppedImage.RotateFlip(RotateFlipType.Rotate180FlipX);
            g.DrawImage(CroppedImage, new System.Drawing.Point(0, 0));
            CroppedImage.RotateFlip(RotateFlipType.Rotate180FlipY);
            g.DrawImage(CroppedImage, new System.Drawing.Point(RW, 0));
            CroppedImage.RotateFlip(RotateFlipType.Rotate180FlipX);
            g.DrawImage(CroppedImage, new System.Drawing.Point(RW, RH));
            bmp.Dispose();
            CroppedImage.Dispose();
            return Bg;
        }
        public static Bitmap SymmetryRD(Bitmap bmp)
        {
            int RW = bmp.Width / 2;
            int RH = bmp.Height / 2;
            Rectangle CropRect = new(RW, RH, RW, RH);
            Bitmap CroppedImage = bmp.Clone(CropRect, bmp.PixelFormat);
            Bitmap Bg = new(bmp.Width, bmp.Height);
            Graphics g = Graphics.FromImage(Bg);
            g.DrawImage(CroppedImage, new System.Drawing.Point(RW, RH));
            CroppedImage.RotateFlip(RotateFlipType.Rotate180FlipX);
            g.DrawImage(CroppedImage, new System.Drawing.Point(RW, 0));
            CroppedImage.RotateFlip(RotateFlipType.Rotate180FlipY);
            g.DrawImage(CroppedImage, new System.Drawing.Point(0, 0));
            CroppedImage.RotateFlip(RotateFlipType.Rotate180FlipX);
            g.DrawImage(CroppedImage, new System.Drawing.Point(0, RH));
            bmp.Dispose();
            CroppedImage.Dispose();
            return Bg;
        }
    }

    public class PicConvert
    {
        public static Bitmap PicTransform(Bitmap picImage, string method)
        {
            var TransformMethod = ImageSymmetry.SymmetryL;
            if (method.Equals("右左"))
            {
                TransformMethod = ImageSymmetry.SymmetryR;
            }
            else if (method.Equals("上下"))
            {
                TransformMethod = ImageSymmetry.SymmetryU;
            }
            else if (method.Equals("下上"))
            {
                TransformMethod = ImageSymmetry.SymmetryD;
            }
            else if (method.Equals("左上"))
            {
                TransformMethod = ImageSymmetry.SymmetryLU;
            }
            else if (method.Equals("左下"))
            {
                TransformMethod = ImageSymmetry.SymmetryLD;
            }
            else if (method.Equals("右上"))
            {
                TransformMethod = ImageSymmetry.SymmetryRU;
            }
            else if (method.Equals("右下"))
            {
                TransformMethod = ImageSymmetry.SymmetryRD;
            }
            return TransformMethod(picImage);
        }
    }

    public class GifConvert
    {

        private static readonly byte[] DefaultBytes = [0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0];

        public static uint GetGifFrameDelay(Image image)
        {
            try
            {
                return (uint)BitConverter.ToInt32(image.GetPropertyItem(0x5100)?.Value ?? DefaultBytes, 4);
            }
            catch
            {
                return 0;
            }
        }
        public static MagickImageCollection GifTransform(Image gifImage, string method)
        {
            var TransformMethod = ImageSymmetry.SymmetryL;
            if (method.Equals("右左"))
            {
                TransformMethod = ImageSymmetry.SymmetryR;
            }
            else if (method.Equals("上下"))
            {
                TransformMethod = ImageSymmetry.SymmetryU;
            }
            else if (method.Equals("下上"))
            {
                TransformMethod = ImageSymmetry.SymmetryD;
            }
            else if (method.Equals("左上"))
            {
                TransformMethod = ImageSymmetry.SymmetryLU;
            }
            else if (method.Equals("左下"))
            {
                TransformMethod = ImageSymmetry.SymmetryLD;
            }
            else if (method.Equals("右上"))
            {
                TransformMethod = ImageSymmetry.SymmetryRU;
            }
            else if (method.Equals("右下"))
            {
                TransformMethod = ImageSymmetry.SymmetryRD;
            }

            FrameDimension Dimension = new(gifImage.FrameDimensionsList[0]);
            int FrameCount = gifImage.GetFrameCount(Dimension);
            uint Delay = GetGifFrameDelay(gifImage);
            var Ncollection = new MagickImageCollection();
            for (int i = 0; i < FrameCount; i++)
            {
                gifImage.SelectActiveFrame(Dimension, i);
                Bitmap frame = new(gifImage);
                MemoryStream FMemoryStream = new();
                TransformMethod(frame).Save(FMemoryStream, ImageFormat.Bmp);
                FMemoryStream.Position = 0;
                MagickImage MagickFrame = new(FMemoryStream)
                {
                    AnimationDelay = Delay,
                    GifDisposeMethod = GifDisposeMethod.Background
                };
                Ncollection.Add(MagickFrame);
                FMemoryStream.Close();
                frame.Dispose();
            }
            Ncollection[0].AnimationIterations = 0;
            gifImage.Dispose();
            return Ncollection;
        }
    }
}
