using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UndefinedBot.Net.Utils;
using UndefinedBot.Net.Command;

namespace UndefinedBot.Net.NetWork
{
    public class HttpServer
    {

        private readonly HttpListener s_httpListener = new();

        private readonly Logger s_httpServerLogger = new("HttpServer");

        public HttpServer(string Prefixe)
        {
            s_httpListener.Prefixes.Add(Prefixe);
        }
        public async Task Start()
        {
            s_httpListener.Start();
            s_httpServerLogger.Info("Http Server Started");
            while (s_httpListener.IsListening)
            {
                try
                {
                    var context = await s_httpListener.GetContextAsync().WaitAsync(new CancellationToken());
                    _ = HandleRequestAsync(context);
                    //catch { }
                }
                catch(Exception ex)
                {
                    s_httpServerLogger.Error("Error Occured, Error Information:");
                    s_httpServerLogger.Error(ex.Message);
                    s_httpServerLogger.Error(ex.StackTrace ?? "");
                }
            }
        }
        public void Stop()
        {
            s_httpServerLogger.Info("Http Server Stopped");
            s_httpListener.Stop();
            s_httpListener.Close();
        }
        private async Task HandleRequestAsync(HttpListenerContext context)
        {
            try
            {
                StreamReader sr = new(context.Request.InputStream);
                string TempString = sr.ReadToEnd().Replace("\\u",";/.-u").Replace("\\", "-:/&]").Replace(";/.-u","\\u");
                string ReqString = Regex.Unescape(TempString);
                sr.Close();
                context.Response.StatusCode = 200;
                context.Response.Close();
                await MsgHandler.HandleMsg(JsonConvert.DeserializeObject<MsgBodySchematics>(ReqString.Replace("-:/&]", "\\")));
            }
            catch (Exception ex)
            {
                s_httpServerLogger.Error("Error Occured, Error Information:");
                s_httpServerLogger.Error(ex.Message);
                s_httpServerLogger.Error(ex.StackTrace ?? "");
            }
        }
    }
}
