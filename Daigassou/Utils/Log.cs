using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daigassou
{
    public static class Log
    {
        private static LogForm logform { get; set; }
        private static DateTime lastTime;

        public delegate void LogEvent(string s);
        public static event LogEvent OverLayLogReceived;
        public static void overlayLog(string text)
        {
            Debug($"overlay: {text}");
            OverLayLogReceived?.Invoke(text);
        }

        //public static void overlayProcess(string process)
        //{
        //    if (log != null)
        //    {
        //        log.Process = process;
        //    }
        //}
        public static void Debug(string text)
        {
            Console.WriteLine($"[{DateTime.Now.ToString("T")}]: {text}");
            //output(Color.Blue, text);
        }


        public static void Info(string text)
        {
            Debug(text);
        }
        public static void Warning(string text)
        {
            Debug(text);
        }
        public static void Ex(Exception e, string text)
        {
            Debug(text);
        }
        

        public static void ByteText(byte[] text, bool isoffset)
        {
            var sb = new StringBuilder();
            var delaytime = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == 0xFF)
                {
                    delaytime += Convert.ToInt32(text[i + 1]);
                }
            }

            if (isoffset)
            {
                if ((DateTime.Now - lastTime).Milliseconds - delaytime > 150)
                {
                    Console.WriteLine("???");
                }
                sb.Append($"{text.Length} Bytes {delaytime} ms Interval {(DateTime.Now - lastTime).Milliseconds} ms");
                sb.AppendLine();
                lastTime = DateTime.Now;
            }
            else
            {

                sb.Append($"{text.Length} Bytes");

            }

            for (var i = 0; i < text.Length; i++)
            {
                //if (i != 0)
                //{
                //    if (i % 16 == 0)
                //    {
                //        sb.AppendLine();
                //    }
                //    else if (i % 8 == 0)
                //    {
                //        sb.Append(' ', 2);
                //    }
                //    else
                //    {
                //        sb.Append(' ');
                //    }
                //}
                sb.Append(' ');
                sb.Append(text[i].ToString("X2"));
            }

            //Log.OverlayLog(sb.ToString());
            Debug(sb.ToString());
        }
    }
}
