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
                        string ImageCachePath;
                        //ParamFormat: [Pattern] [ImageUrl]
                        if (Args.Param[1].StartsWith("http"))
                        {
                            ImageCachePath = ImageConvert.GetConvertedImage(Args.Param[1],ImageContentType.Url, Args.Param[0]);
                        }
                        //ParamFormat: [MsgId] [Pattern]
                        else
                        {
                            ImageCachePath = ImageConvert.GetConvertedImage(Args.Param[0], ImageContentType.MsgId, Args.Param[1]);
                        }
                        if (ImageCachePath.Length == 0)
                        {
                            ExecuteLogger.Error($"Pic Convert Failed, At Command <{Args.Command}>");
                            await HttpApi.SendGroupMsg(
                                Args.GroupId,
                                new MsgBuilder()
                                    .Text("似乎转换不了").Build()
                            );
                        }
                        else
                        {
                            await HttpApi.SendGroupMsg(
                                        Args.GroupId,
                                        new MsgBuilder()
                                            .Reply(Args.MsgId)
                                            .Image(ImageCachePath, ImageSendType.LocalFile, ImageSubType.Normal).Build()
                                    );
                            FileIO.SafeDeleteFile(ImageCachePath);
                        }
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
                                    .Text("一言似乎迷路了").Build()
                            );
                    }
                }
                else if (Args.Command.Equals("queto"))
                {
                    //ParamFormat: [TargetMsg]
                    if (Args.Param.Count > 0)
                    {
                        string ImageCachePath = Queto.GenQuetoImage(Args.Param[0]);
                        if (ImageCachePath.Length == 0)
                        {
                            ExecuteLogger.Error($"Generate Failed, At Command <{Args.Command}>");
                            await HttpApi.SendGroupMsg(
                                Args.GroupId,
                                new MsgBuilder()
                                    .Text("生成出错了").Build()
                            );
                        }
                        else
                        {
                            await HttpApi.SendGroupMsg(
                                    Args.GroupId,
                                    new MsgBuilder()
                                        .Image(ImageCachePath, ImageSendType.LocalFile, ImageSubType.Normal).Build()
                                );
                            FileIO.SafeDeleteFile(ImageCachePath);
                        }
                    }
                    else
                    {
                        ExecuteLogger.Error($"Unproper Arg: Too Less Args, At Command <{Args.Command}>");
                    }
                }
                /*
                else if (Args.Command.Equals("invert"))
                {
                    //ParamFormat: [MsgId] or [ImageUrl]
                    if (Args.Param.Count > 0)
                    {
                        MagickImage? MIm = null;
                        if (Args.Param[0].StartsWith("http"))
                        {
                            //[ImageUrl]
                            byte[] ImageBytes = await HttpRequest.GetBinary(Args.Param[0]);
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
                                            .Text("定位不到图片").Build()
                                    );
                            }
                            else
                            {
                                string PicUrl = CommandResolver.ExtractUrlFromMsg(TargetMsg);
                                byte[] ImageBytes = await HttpRequest.GetBinary(PicUrl);
                                if (ImageBytes.Length > 0)
                                {
                                    MIm = new(ImageBytes);
                                }
                            }
                        }
                        if (MIm != null)
                        {
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
                */
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
                    //ParamFormat: [Emoji1] [Emoji2] or [Emoji1Emoji2]
                    if (Args.Param.Count > 0)
                    {
                        string MixRes = EmojiMix.MixEmoji(Args.Param);
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
                                        .Text("似乎不能混合").Build()
                                );
                        }
                    }
                    else
                    {
                        ExecuteLogger.Error($"Unproper Arg: Too Less Args, At Command <{Args.Command}>");
                    }
                }
                else if (Args.Command.Equals("random"))
                {
                    //ParamFormat: [RandomType]
                    if (Args.Param.Count > 0)
                    {
                        string OutUrl = RandomPicture.GetRandomContent(Args.Param[0]);
                        if (OutUrl.Length > 0)
                        {
                            await HttpApi.SendGroupMsg(
                                            Args.GroupId,
                                            new MsgBuilder()
                                                .Image(OutUrl, ImageSendType.Url).Build()
                                        );
                        }
                        else
                        {
                            await HttpApi.SendGroupMsg(
                                    Args.GroupId,
                                    new MsgBuilder()
                                        .Text("呃啊，图片迷路了").Build()
                                );
                        }
                    }
                    else
                    {
                        ExecuteLogger.Error($"Unproper Arg: Too Less Args, At Command <{Args.Command}>");
                    }
                }
                else if (Args.Command.Equals("homo"))
                {
                    //ParamFormat: [Text]
                    if (Args.Param.Count > 0)
                    {
                        string Res = Homo.Homoize(Args.Param[0],out bool Status);
                        if (Status)
                        {

                            await HttpApi.SendGroupMsg(
                                                Args.GroupId,
                                                new MsgBuilder()
                                                    //.Reply(Args.MsgId)
                                                    .Text($"{Args.Param[0]} = {Res}").Build()
                                            );
                        }
                        else
                        {
                            await HttpApi.SendGroupMsg(
                                                Args.GroupId,
                                                new MsgBuilder()
                                                    //.Reply(Args.MsgId)
                                                    .Text($"{Res}").Build()
                                            );
                        }
                    }
                    else
                    {
                        ExecuteLogger.Error($"Unproper Arg: Too Less Args, At Command <{Args.Command}>");
                    }
                }
                else if (Args.Command.Equals("histoday"))
                {
                    //ParamFormat: Any
                    string ImageCachePath = Histoday.GenHistodayImage();
                    await HttpApi.SendGroupMsg(
                                    Args.GroupId,
                                    new MsgBuilder()
                                        .Image(ImageCachePath, ImageSendType.LocalFile, ImageSubType.Normal).Build()
                                );
                    FileIO.SafeDeleteFile(ImageCachePath);

                }
                ExecuteLogger.Info("Execute Finished");
            }
            else
            {
                ExecuteLogger.Error($"Unknown Command: <{Args.Command}>");
                await HttpApi.SendGroupMsg(
                                Args.GroupId,
                                new MsgBuilder()
                                    .Text($"这发的什么东西: <{Args.Command}>").Build()
                            );
            }
        }
    }
}
