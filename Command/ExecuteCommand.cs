using System;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Drawing.Imaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ImageMagick;
using ImageMagick.Drawing;
using UndefinedBot.Net.NetWork;
using UndefinedBot.Net.Utils;
using UndefinedBot.Net.Extra;
using System.Reflection;

namespace UndefinedBot.Net.Command
{
    internal class CommandExecutor
    {

        private static readonly ConfigManager CEConfigManager = Program.GetConfigManager();

        private static readonly Logger ExecuteLogger = new("CommandExecutor", "Execute");

        private static readonly List<string> AvaliableCommands = CEConfigManager.GetCommandList();

        private static readonly Assembly CEAssembly = Assembly.GetExecutingAssembly();
        public static async void Execute(ArgSchematics Args)
        {
            if (AvaliableCommands.Contains(Args.Command))
            {
                ExecuteLogger.Info($"Executing: <{Args.Command}>,  With Arg\n{JsonConvert.SerializeObject(Args, Formatting.Indented)}");
                if (Args.Command.Equals("help"))
                {
                    //ParamFormat: [<Command>]
                    if (Args.Param.Count > 0)
                    {
                        if (!await HelpCommand.PrintHelpText(Args.GroupId, Args.Param[0]))
                        {
                            ExecuteLogger.Error($"Unknown Command: <{Args.Param[0]}>, At Command <{Args.Command}>");
                            return;
                        }
                    }
                    else
                    {
                        _ = await HelpCommand.PrintHelpText(Args.GroupId, "help");
                    }
                }
                else if (Args.Command.Equals("symmet"))
                {
                    //ParamFormat: [MsgId] [Pattern] or [Pattern] [ImageUrl]
                    if (Args.Param.Count > 1)
                    {
                        Image? Im = null;
                        MemoryStream ms = new();
                        string Pattern = "L";
                        if (Args.Param[1].StartsWith("http"))
                        {
                            //[Pattern] [ImageUrl]
                            byte[] ImageBytes = await HttpService.GetBinary(Args.Param[1]);
                            if (ImageBytes.Length > 0)
                            {
                                ms = new MemoryStream(ImageBytes);
                                Im = Image.FromStream(ms);
                            }
                            Pattern = Args.Param[0];
                        }
                        else
                        {
                            //[MsgId] [Pattern]
                            MsgBodySchematics TargetMsg = await HttpApi.GetMsg(Args.Param[0]);
                            if ((TargetMsg.MessageId ?? 0) == 0)
                            {
                                ExecuteLogger.Error($"Invalid MsgId: {Args.Param[0]}, At Command <{Args.Command}>");
                                await HttpApi.SendGroupMsg(
                                        Args.GroupId,
                                        new MsgBuilder()
                                            .Text("无效的Msg").Build()
                                    );
                            }
                            else
                            {
                                string PicUrl = CommandResolver.ExtractUrlFromMsg(TargetMsg);
                                byte[] ImageBytes = await HttpService.GetBinary(PicUrl);
                                if (ImageBytes.Length > 0)
                                {
                                    ms = new MemoryStream(ImageBytes);
                                    Im = Image.FromStream(ms);
                                }
                                Pattern = Args.Param[1];
                            }
                        }
                        if (Im != null)
                        {
                            if (Im.RawFormat.Equals(ImageFormat.Gif))
                            {
                                string ImCachePath = Path.Join(Program.GetProgramCahce(), $"{DateTime.Now:HH-mm-ss}.gif");
                                MagickImageCollection ResultImage = GifConvert.GifTransform(Im, Pattern);
                                if (ResultImage.Count > 0)
                                {
                                    ResultImage.Write(ImCachePath);
                                    await HttpApi.SendGroupMsg(
                                        Args.GroupId,
                                        new MsgBuilder()
                                            .Reply(Args.MsgId)
                                            .Image(ImCachePath, ImageSendType.LocalFile, ImageSubType.Normal).Build()
                                    );
                                    FileIO.SafeDeleteFile(ImCachePath);
                                }
                                else
                                {
                                    ExecuteLogger.Error($"Pic Convert Failed, At Command <{Args.Command}>");
                                    await HttpApi.SendGroupMsg(
                                        Args.GroupId,
                                        new MsgBuilder()
                                            .Text("转换失败").Build()
                                    );
                                }
                                ResultImage.Dispose();
                            }
                            else
                            {
                                string ImCachePath = Path.Join(Program.GetProgramCahce(), $"{DateTime.Now:HH-mm-ss}.png");
                                Bitmap ResultImage = PicConvert.PicTransform(new Bitmap(Im), Pattern);
                                if (ResultImage != null)
                                {
                                    ResultImage.Save(ImCachePath, ImageFormat.Gif);
                                    await HttpApi.SendGroupMsg(
                                        Args.GroupId,
                                        new MsgBuilder()
                                            .Reply(Args.MsgId)
                                            .Image(ImCachePath, ImageSendType.LocalFile, ImageSubType.Normal).Build()
                                    );
                                    FileIO.SafeDeleteFile(ImCachePath);
                                }
                                else
                                {
                                    ExecuteLogger.Error($"Pic Convert Failed, At Command <{Args.Command}>");
                                    await HttpApi.SendGroupMsg(
                                        Args.GroupId,
                                        new MsgBuilder()
                                            .Text("转换失败").Build()
                                    );
                                }
                                ResultImage?.Dispose();
                            }
                            Im.Dispose();
                        }
                        ms.Close();
                    }
                    else
                    {
                        ExecuteLogger.Error($"Unproper Arg: Too Less Args, At Command <{Args.Command}>");
                    }
                }
                else if (Args.Command.Equals("hito"))
                {
                    //ParamFormat: [Type]
                    HitokotoSchematics Hitokoto = await HttpApi.GetHitokoto(Args.Param.Count > 0 ? Args.Param[0] : "");
                    if ((Hitokoto.Id ?? 0) != 0)
                    {
                        await HttpApi.SendGroupMsg(
                                Args.GroupId,
                                new MsgBuilder()
                                    .Text($"{Hitokoto.Hitokoto}\n---- {Hitokoto.Creator}").Build()
                            );
                    }
                    else
                    {
                        ExecuteLogger.Error($"Get Hitokoto Failed, At Command <{Args.Command}>");
                        await HttpApi.SendGroupMsg(
                                Args.GroupId,
                                new MsgBuilder()
                                    .Text("获取失败").Build()
                            );
                    }
                }
                else if (Args.Command.Equals("queto"))
                {
                    //ParamFormat: [TargetMsg]
                    if (Args.Param.Count > 0)
                    {
                        MsgBodySchematics TargetMsg = await HttpApi.GetMsg(Int32.TryParse(Args.Param[0], out int TargetMsgId) ? TargetMsgId : 0);
                        if ((TargetMsg.MessageId ?? 0) == 0)
                        {
                            ExecuteLogger.Error($"Invalid MsgId: {Args.Param[0]}, At Command <{Args.Command}>");
                            await HttpApi.SendGroupMsg(
                                    Args.GroupId,
                                    new MsgBuilder()
                                        .Text("无效的Msg").Build()
                                );
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
                                    TargetMsgString += (TextRender.QFaceReference.TryGetValue(FId,out var EmojiString) ? EmojiString : "");
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            long TargetUin = TargetMsg.Sender?.UserId ?? 0;
                            GroupMemberSchematics CMember = await HttpApi.GetGroupMember(Args.GroupId, TargetUin);
                            string TargetName = $"@{CMember.Nickname ?? ""}";
                            string ImCachePath = Path.Join(Program.GetProgramCahce(), $"{DateTime.Now:HH-mm-ss}.png");
                            string QSplashPath = Path.Join(Program.GetProgramLocal(), "QSplash.png");
                            string QTextCachePath = Path.Join(Program.GetProgramCahce(), $"Text_{DateTime.Now:HH-mm-ss}.png");
                            string QNNCachePath = Path.Join(Program.GetProgramCahce(), $"NickName_{DateTime.Now:HH-mm-ss}.png");
                            if (File.Exists(QSplashPath))
                            {
                                Image CoverImage = Image.FromFile(QSplashPath);
                                Image TargetAvatar = await HttpApi.GetQQAvatar(TargetUin);
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
                                await HttpApi.SendGroupMsg(
                                    Args.GroupId,
                                    new MsgBuilder()
                                        .Image(ImCachePath, ImageSendType.LocalFile, ImageSubType.Normal).Build()
                                );
                                TextBmp.Dispose();
                                NameBmp.Dispose();
                                g.Dispose();
                                bg.Dispose();
                                CoverImage.Dispose();
                                TargetAvatar.Dispose();
                                FileIO.SafeDeleteFile(ImCachePath);
                                FileIO.SafeDeleteFile(QNNCachePath);
                                FileIO.SafeDeleteFile(QTextCachePath);
                            }
                            else
                            {
                                ExecuteLogger.Error($"Generate Failed, At Command <{Args.Command}>");
                                await HttpApi.SendGroupMsg(
                                    Args.GroupId,
                                    new MsgBuilder()
                                        .Text("生成失败").Build()
                                );
                            }
                        }
                    }
                    else
                    {
                        ExecuteLogger.Error($"Unproper Arg: Too Less Args, At Command <{Args.Command}>");
                    }
                }
                else if (Args.Command.Equals("invert"))
                {
                    //ParamFormat: [MsgId] or [ImageUrl]
                    if (Args.Param.Count > 0)
                    {
                        MagickImage? MIm = null;
                        if (Args.Param[0].StartsWith("http"))
                        {
                            //[ImageUrl]
                            byte[] ImageBytes = await HttpService.GetBinary(Args.Param[0]);
                            if (ImageBytes.Length > 0)
                            {
                                MIm = new(ImageBytes);
                            }
                        }
                        else
                        {
                            //[MsgId]
                            MsgBodySchematics TargetMsg = await HttpApi.GetMsg(Args.Param[0]);
                            if ((TargetMsg.MessageId ?? 0) == 0)
                            {
                                ExecuteLogger.Error($"Invalid MsgId: {Args.Param[0]}, At Command <{Args.Command}>");
                                await HttpApi.SendGroupMsg(
                                        Args.GroupId,
                                        new MsgBuilder()
                                            .Text("无效的Msg").Build()
                                    );
                            }
                            else
                            {
                                string PicUrl = CommandResolver.ExtractUrlFromMsg(TargetMsg);
                                byte[] ImageBytes = await HttpService.GetBinary(PicUrl);
                                if (ImageBytes.Length > 0)
                                {
                                    MIm = new(ImageBytes);
                                }
                            }
                        }
                        if (MIm != null)
                        {
                            Console.WriteLine(1);
                            string ImCachePath = Path.Join(Program.GetProgramCahce(), $"{DateTime.Now:HH-mm-ss}.png");
                            MIm.Negate();
                            MIm.Write(ImCachePath);
                            await HttpApi.SendGroupMsg(
                                    Args.GroupId,
                                    new MsgBuilder()
                                        .Reply(Args.MsgId)
                                        .Image(ImCachePath, ImageSendType.LocalFile, ImageSubType.Normal).Build()
                                );
                            FileIO.SafeDeleteFile(ImCachePath);
                        }
                        MIm?.Dispose();
                    }
                    else
                    {
                        ExecuteLogger.Error($"Unproper Arg: Too Less Args, At Command <{Args.Command}>");
                    }
                }
                else if (Args.Command.Equals("raw"))
                {
                    //ParamFormat: Any
                    if (Args.Param.Count > 0)
                    {
                        MsgBodySchematics TargetMsg = await HttpApi.GetMsg(Args.Param[0]);
                        await HttpApi.SendGroupMsg(
                                        Args.GroupId,
                                        new MsgBuilder()
                                            .Text(JsonConvert.SerializeObject(TargetMsg.Message, Formatting.Indented)).Build()
                                    );
                    }
                    else
                    {
                        ExecuteLogger.Error($"Unproper Arg: Too Less Args, At Command <{Args.Command}>");
                    }
                }
                else if (Args.Command.Equals("mix"))
                {
                    //ParamFormat: [Emoji1] [Emoji2]
                    if (Args.Param.Count > 1)
                    {
                        string MixRes = EmojiMix.MixEmoji(Args.Param[0], Args.Param[1]);
                        if (MixRes.Length > 0)
                        {
                            await HttpApi.SendGroupMsg(
                                            Args.GroupId,
                                            new MsgBuilder()
                                                .Reply(Args.MsgId)
                                                .Image(MixRes,ImageSendType.Url).Build()
                                        );
                        }
                        else
                        {
                            await HttpApi.SendGroupMsg(
                                    Args.GroupId,
                                    new MsgBuilder()
                                        .Text("生成失败").Build()
                                );
                        }
                    }
                    else
                    {
                        ExecuteLogger.Error($"Unproper Arg: Too Less Args, At Command <{Args.Command}>");
                    }
                }
                ExecuteLogger.Info("Execute Finished");
            }
            else
            {
                ExecuteLogger.Error($"Unknown Command: <{Args.Command}>");
                await HttpApi.SendGroupMsg(
                                Args.GroupId,
                                new MsgBuilder()
                                    .Text($"未知指令: <{Args.Command}>").Build()
                            );
            }
        }
    }
}
