using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace MyServerDanmaku
{
    public class ConfigLoader
    {
        public class ConfigClass
        {
            public long[] Admin { get; set; } = new long[0];
            public long[] Groups { get; set; } = new long[0];
            public string FontFamily { get; set; } = "黑体";
            public float FontSize { get; set; } = 44;
            public string EmojiFontFamily { get; set; } = "Segoe UI Emoji";
            public float EmojiFontSize { get; set; } = 33;
            public string Color { get; set; } = "#FFFFFF";
            public string BorderColor { get; set; } = "#000000";
            public int BorderWidth { get; set; } = 5;
            public string FacePath { get; set; } = "wxFace/48/";
            public string WelcomeString { get; set; } = "添加成功。您可以发送文字、表情或图片。";
            public double TimeSpan { get; set; } = 0.5;
            public int MaxImageWidth { get; set; } = 960;
            public int MaxImageHeight { get; set; } = 540;
            public bool AllowImage { get; set; } = true;
            public bool ShowName { get; set; } = false;
        }
        public ConfigClass Config = new ConfigClass();
        public bool Load()
        {
            try
            {
                Config = JsonConvert.DeserializeObject<ConfigClass>(File.ReadAllText(Path.Combine(Common.AppDirectory, "config.json"), Encoding.UTF8));
                Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Info, "提示", "已载入配置文件");
                return true;
            }
            catch
            {
                Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Info, "提示", "找不到配置文件，使用默认配置文件");
                return false;
            }
        }

        public bool Save()
        {
            try
            {
                File.WriteAllText(Path.Combine(Common.AppDirectory, "config.json"), JsonConvert.SerializeObject(Config), Encoding.UTF8);
                Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Info, "提示", "已保存配置文件");
                return true;
            }
            catch
            {
                Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Error, "错误", "无法保存配置文件，请确认是否有权限");
                return false;
            }
        }
    }
}
