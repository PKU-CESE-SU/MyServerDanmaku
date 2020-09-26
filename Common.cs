namespace MyServerDanmaku
{
    public static class Common
    {
        public static ConfigLoader ConfigLoader { get; set; }
        public static DanmakuWall DanmakuWall { get; set; }
        public static string AppDirectory
        {
            get
            {
                return "";
            }
        }
        public static CqApi CqApi { get; set; }
    }
}
