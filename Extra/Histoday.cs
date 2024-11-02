using System;
using System.Drawing;
using System.Globalization;
using System.Runtime.Serialization;
using UndefinedBot.Net.NetWork;

namespace UndefinedBot.Net.Extra
{
    public class Histoday
    {
        private static readonly CultureInfo s_zhCNInfo = new("zh-CN");

        private static readonly Calendar ZhCNCalendar = s_zhCNInfo.DateTimeFormat.Calendar;

        private static readonly Font s_dateFont = new("Simsun", 60);

        private static readonly Font s_contentFont = new("Simsun", 40);

        private static readonly SolidBrush s_drawBrush = new(Color.Black);
        public static string GenHistodayImage()
        {
            string CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + $"\n{ZhCNCalendar.GetDayOfWeek(DateTime.Now)}";
            string Content = Program.GetHttpRequest().Get("https://xiaoapi.cn/API/lssdjt.php").Result;
            Bitmap bg = new(1080,1500);
            Graphics g = Graphics.FromImage(bg);
            g.Clear(Color.White);
            g.DrawString(CurrentTime, s_dateFont, s_drawBrush, new RectangleF(100, 100, 880, 200));
            g.DrawString(Content, s_contentFont, s_drawBrush, new RectangleF(50, 400, 980, 1100));
            string ImCachePath = Path.Join(Program.GetProgramCahce(), $"{DateTime.Now:HH-mm-ss}.png");
            bg.Save(ImCachePath);
            g.Dispose();
            bg.Dispose();
            return ImCachePath;
        }
    }
}
