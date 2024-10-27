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

        private readonly HttpListener httpListener = new();

        private readonly Logger HServerLogger = new("HttpServer");

        public HttpServer(string Prefixe)
        {
            httpListener.Prefixes.Add(Prefixe);
        }
        public async Task Start()
        {
            httpListener.Start();
            HServerLogger.Info("Http Server Started");
            while (httpListener.IsListening)
            {
                try
                {
                    var context = await httpListener.GetContextAsync().WaitAsync(new CancellationToken());
                    _ = HandleRequestAsync(context);
                    //catch { }
                }
                catch(Exception ex)
                {
                    HServerLogger.Error("Error Occured, Error Information:");
                    HServerLogger.Error(ex.Message);
                    HServerLogger.Error(ex.StackTrace ?? "");
                }
            }
        }
        public void Stop()
        {
            HServerLogger.Info("Http Server Stopped");
            httpListener.Stop();
            httpListener.Close();
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
                await CommandResolver.HandleMsg(JsonConvert.DeserializeObject<MsgBodySchematics>(ReqString.Replace("-:/&]","\\")));
            }
            catch (Exception ex)
            {
                HServerLogger.Error("Error Occured, Error Information:");
                HServerLogger.Error(ex.Message);
                HServerLogger.Error(ex.StackTrace ?? "");
            }
        }
    }
}
