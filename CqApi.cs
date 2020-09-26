using Sdk.Cqp.Enum;
using System;

namespace MyServerDanmaku
{
    public class CqApi
    {
        public int AddLoger(LogerLevel level, string type, string content)
        {
            switch (level)
            {
                case LogerLevel.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
                case LogerLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogerLevel.Fatal:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
                case LogerLevel.Info:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case LogerLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
            Console.WriteLine($"{type}：{content}");
            return 0;
        }
    }
}
