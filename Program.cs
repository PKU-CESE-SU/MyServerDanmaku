using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace MyServerDanmaku
{
    class Danmaku
    {
        public int id { get; set; }
        public string sender { get; set; }
        public string content { get; set; }
        public int time { get; set; }
        public int type { get; set; }
    }

    class Program
    {
        static readonly string url = "https://wechat.xzonn.top/read.php";
        static int lastId = 0;
        static bool rendering = false;
        static string emojiRange = @"[\ud83c-\ud83e][\ud000-\udfff]";

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            Common.CqApi = new CqApi();
            Common.ConfigLoader = new ConfigLoader();
            Common.ConfigLoader.Load();
            Common.ConfigLoader.Save();
            Timer timer = new Timer
            {
                Enabled = true,
                Interval = 1000
            };
            timer.Elapsed += new ElapsedEventHandler(GetDanmaku);
            timer.Start();
            Common.DanmakuWall = new DanmakuWall();
            Application.Run(Common.DanmakuWall);
        }

        static void GetDanmaku(object source, ElapsedEventArgs args)
        {
            if (rendering)
            {
                return;
            }
            rendering = true;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + "?id=" + lastId);
                request.Method = "GET";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
                Danmaku[] danmakus = JsonConvert.DeserializeObject<Danmaku[]>(retString);
                foreach (Danmaku danmaku in danmakus)
                {
                    SendDanmaku(danmaku);
                    if (danmaku.id > lastId)
                    {
                        lastId = danmaku.id;
                    }
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                rendering = false;
            }
        }

        static void SendDanmaku(Danmaku danmaku)
        {
            string text = Regex.Replace(danmaku.content, emojiRange, new MatchEvaluator(ReplaceEmoji));
            switch (danmaku.type)
            {
                case 2:
                    string[] splited = danmaku.content.Split('/');
                    if (!Directory.Exists("images/"))
                    {
                        Directory.CreateDirectory("images/");
                    }
                    if (!File.Exists($"images/{splited[4]}.jpg"))
                    {
                        try
                        {
                            WebClient mywebclient = new WebClient();
                            mywebclient.DownloadFile(danmaku.content, $"images/{splited[4]}.jpg");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                    text = $"[CQ:image,path=images/{splited[4]}.jpg]";
                    break;
            }
            Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Debug, "调试", $"收到弹幕：{text}");
            Common.DanmakuWall.SendDanmaku(text);
        }
        static string ReplaceEmoji(Match match)
        {
            return $"[CQ:emoji,code={char.ConvertToUtf32(match.Value, 0)}]";
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string resourceName = new AssemblyName(args.Name).Name;
            resourceName = Assembly.GetExecutingAssembly().GetManifestResourceNames().First(x => x.Contains(resourceName));
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                byte[] assemblyData = new byte[stream.Length];
                stream.Read(assemblyData, 0, assemblyData.Length);
                return Assembly.Load(assemblyData);
            }
        }
    }
}
