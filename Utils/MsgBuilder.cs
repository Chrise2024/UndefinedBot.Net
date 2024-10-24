using System;
using System.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UndefinedBot.Net.NetWork;

namespace UndefinedBot.Net.Utils
{
    public enum ImageSendType
    {
        LocalFile = 0,
        Url = 1,
        Base64 = 2
    }
    public enum ImageSubType
    {
        Normal = 0,  //正常图片
        Emoji = 1,   //表情包
        Hot = 2,     //热图
        Battel = 3,  //斗图
    }
    internal class MsgBuilder
    {

        private readonly List<JObject> MsgChain = [];

        public MsgBuilder Text(string Text)
        {
            MsgChain.Add(new JObject()
            {
                { "type" , "text" },
                { "data" ,  new JObject(){
                    { "text" , Text }
                } },
            });
            return this;
        }
        public MsgBuilder QFace(string Face)
        {
            MsgChain.Add(new JObject()
            {
                { "type" , "text" },
                { "data" ,  new JObject(){
                    { "id" , Face }
                } },
            });
            return this;
        }
        public MsgBuilder Image(string ImageContent, ImageSendType SendType = ImageSendType.LocalFile,ImageSubType Subtype = ImageSubType.Normal)
        {
            string FPrefix = "";
            if (SendType == ImageSendType.LocalFile)
            {
                FPrefix = "file:///";
            }
            else if (SendType == ImageSendType.Base64)
            {
                FPrefix = "base64://";
            }
            MsgChain.Add(new JObject()
            {
                { "type" , "image" },
                { "data" ,  new JObject(){
                    { "file" , $"{FPrefix}{ImageContent}" },
                    { "subType" , (int)Subtype }
                } },
            });
            return this;
        }
        public MsgBuilder At(long AtUin)
        {
            MsgChain.Add(new JObject()
            {
                { "type" , "at" },
                { "data" ,  new JObject(){
                    { "qq" , AtUin }
                } },
            });
            return this;
        }
        public MsgBuilder Reply(int MsgId)
        {
            MsgChain.Add(new JObject()
            {
                { "type" , "reply" },
                { "data" ,  new JObject(){
                    { "id" , $"{MsgId}" }
                } },
            });
            return this;
        }
        public List<JObject> Build()
        {
            return MsgChain;
        }
    }
}
