using System;
using System.IO;

namespace WpfApplication
{
    public static class Logger
    {
        public static object locker = new object();

        public static void Log(string text)
        {
            lock (locker)
            {
                using (var writer = new StreamWriter("Log.txt", append: true))
                {
                    writer.WriteLine($"{DateTime.Now}.{DateTime.Now.Millisecond} | {text}");
                }
            }
        }
    }
}