namespace Magicube.LightApp.Wechat {
    public class WechatConfigOptions {
        public WechatConfigItem[] Items { get; set; }
    }

    public class WechatConfigItem {
        public string Identity  { get; set; }

        public string AppId     { get; set; }
        public string AppSecret { get; set; }
    }
}
