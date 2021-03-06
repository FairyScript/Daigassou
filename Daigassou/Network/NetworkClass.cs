﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Daigassou.Utils;
using Machina.FFXIV;
using Machina;
using NetFwTypeLib;

namespace Daigassou
{
    public class PlayEvent : EventArgs
    {
        private readonly string text;
        private readonly int time;
        private readonly int mode;
        private readonly int timeStamp;
        [Obsolete("2.4.0.0版本弃用")]
        public PlayEvent(int Mode, int Time, String Text)
        {
            mode = Mode;
            time = Time;
            text = Text;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Mode">0:播放,1:停止</param>
        /// <param name="Time">秒数</param>
        /// <param name="TimeStamp">开始的时间戳</param>
        /// <param name="Text">发起人</param>
        public PlayEvent(int Mode, int Time, int TimeStamp, String Text)
        {
            mode = Mode;
            time = Time;
            text = Text;
            timeStamp = TimeStamp;
        }
        public String Text { get { return text; } }
        public int Time { get { return time; } }
        public int Mode
        {
            get { return mode; }
        }
        public int TimeStamp { get { return timeStamp; } }
    }
    public class PacketEntry
    {
        public string Direction { get; set; }
        public string Message { get; set; }
        public string Timestamp { get; set; }
        public string RouteID { get; set; }
        [XmlElement(ElementName = "Data")]
        public string DataString { get; set; }
        [XmlIgnore]
        public byte[] Data { get; set; }
        public string Note { get; set; }
        public int Set { get; set; }
        public uint PacketUnixTime { get; set; }
        public long SystemMsTime { get; set; }
        public bool IsDecrypted { get; set; }
        public FFXIVNetworkMonitor.ConnectionType Connection { get; set; }

        // ignored values
        [XmlIgnore]
        public string Name { get; set; }
        [XmlIgnore]
        public string Comment { get; set; }
        [XmlIgnore]
        public string Size { get; set; }
        [XmlIgnore]
        public string Category { get; set; }
        [XmlIgnore]
        public int ActorControl { get; set; }
        [XmlIgnore]
        public bool IsVisible { get; set; }
        [XmlIgnore]
        public bool IsForSelf { get; set; }

        public PacketEntry()
        {
            IsVisible = true;
            RouteID = string.Empty;
        }
    }
    public class NetworkClass
    {
        public event EventHandler<PlayEvent> Play;
        private void MessageReceived(long epoch, byte[] message, int set, FFXIVNetworkMonitor.ConnectionType connectionType)
        {
            var res = Parse(message);

            
            if (res.header.MessageType == 0x01ac)//CountDown
            {
                var countDownTime = res.data[36];
                var nameBytes = new byte[18];
                var timeStampBytes = new byte[4];
                Array.Copy(res.data, 41, nameBytes, 0, 18);
                Array.Copy(res.data, 24, timeStampBytes, 0, 4);
                var name = Encoding.UTF8.GetString(nameBytes) ?? "";
                Console.WriteLine("Countdown");
                Play?.Invoke(this, new PlayEvent(1, 0, BitConverter.ToInt32(timeStampBytes, 0), name));
            }


            if (res.header.MessageType == 0x02d2) //ensemble
            {
                Console.WriteLine("ensemble ready");
                var timeStampBytes = new byte[4];
                Array.Copy(res.data, 24, timeStampBytes, 0, 4);
                Console.WriteLine(BitConverter.ToInt32(timeStampBytes, 0));
                Play?.Invoke(this, new PlayEvent(0, 5, BitConverter.ToInt32(timeStampBytes, 0), "合奏助手"));

            }
            //if (res.header.MessageType == 0x02e8) //scene mark
            //{
            //    Console.WriteLine("ensemble end");

            //    Play?.Invoke(this, new PlayEvent(1, 0, "紧急"));

            //}
            

        }
        internal class ParseResult
        {
            public FFXIVMessageHeader header;
            public byte[] data;
        }
        internal class TimedNote
        {
            public DateTime StartTime;
            public int Note;
            public override string ToString()
            {
                return string.Format($"StartTime: {StartTime.ToString("HH:mm:ss.fff")}, Note: {Note.ToString("X2")}");
            }
        }

        private DateTime lastSentTime;
        private void MessageSent(long epoch, byte[] message, int set, FFXIVNetworkMonitor.ConnectionType connectionType)
        {
            var res = Parse(message);

            var item = new PacketEntry
            {
                IsVisible = true,
                ActorControl = -1,
                Data = message,
                Message = res.header.MessageType.ToString("X4"),
                Direction = "C",
                Category = set.ToString(),
               // Timestamp = Util.UnixTimeStampToDateTime(res.header.Seconds).ToString(@"MM\/dd\/yyyy HH:mm:ss"),
                Size = res.header.MessageLength.ToString(),
                Set = set,
                RouteID = res.header.RouteID.ToString(),
                PacketUnixTime = res.header.Seconds,
               // SystemMsTime = Millis(),
                Connection = connectionType
            };
            if (res.header.MessageType == 0x018B) //Bard Performance
            {
                var length = res.data[32];
                var notes = new byte[length];
                Array.Copy(res.data, 33, notes, 0, length);
                Log.ByteText(notes, true);//TODO: Time analyze 
                //ParameterController.GetInstance().AnalyzeNotes(notes);
            }
        }

        private static ParseResult Parse(byte[] data)
        {
            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            FFXIVMessageHeader head = (FFXIVMessageHeader)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(FFXIVMessageHeader));
            handle.Free();

            ParseResult result = new ParseResult();
            result.header = head;
            result.data = data;

            return result;
        }

        public bool _shouldStop = false;
        public void Run(uint processID)
        {
            FFXIVNetworkMonitor monitor = new FFXIVNetworkMonitor();
            RegisterToFirewall();
            monitor.MonitorType = TCPNetworkMonitor.NetworkMonitorType.RawSocket;
            monitor.MessageReceived = MessageReceived;
            monitor.MessageSent = MessageSent;
            monitor.ProcessID = processID;
            monitor.Start();

            while (!_shouldStop)
            {
                // So don't burn the cpu while doing nothing
                Thread.Sleep(1);
            }

            Console.WriteLine("MachinaCaptureWorker: Terminating");
            monitor.Stop();
        }
        private void RegisterToFirewall()
        {
            try
            {
                Process p = new Process();
                var exePath = Process.GetCurrentProcess().MainModule.FileName;
                //p.StartInfo.FileName = "cmd.exe"; //命令
                //p.StartInfo.UseShellExecute = false; //不启用shell启动进程
                //p.StartInfo.RedirectStandardInput = true; // 重定向输入
                //p.StartInfo.RedirectStandardOutput = true; // 重定向标准输出
                //p.StartInfo.RedirectStandardError = true; // 重定向错误输出 
                //p.StartInfo.CreateNoWindow = true; // 不创建新窗口
                //p.Start();
                //p.StandardInput.WriteLine("netsh advfirewall firewall add rule name=\"WinClient\" dir=in program=\"" + exePath + "\" action=allow localip=any remoteip=any security=notrequired description=DFAssist"); //cmd执行的语句
                //                                                                                                                                                                                                        //p.StandardOutput.ReadToEnd(); //读取命令执行信息
                //p.StandardInput.WriteLine("exit"); //退出

                INetFwPolicy2 firewallPolicy = GetInstance<INetFwPolicy2>("HNetCfg.FwPolicy2");
                INetFwRule firewallRule = GetInstance<INetFwRule>("HNetCfg.FWRule");
                bool isExists = false;

                foreach (INetFwRule item in firewallPolicy.Rules)
                {
                    if (item.ApplicationName == exePath) isExists = true;
                }
                if (!isExists)
                {
                    firewallRule.Name = "Daigassou";
                    firewallRule.ApplicationName = exePath;
                    firewallRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
                    firewallRule.Description = " My Windows Firewall Rule";
                    firewallRule.Enabled = true;
                    firewallRule.InterfaceTypes = "All";

                    firewallPolicy.Rules.Add(firewallRule);
                }
            }
            catch (Exception ex)
            {
                Log.Ex(ex, "l-firewall-error");
            }
        }

        private T GetInstance<T>(string typeName)
        {
            return (T)Activator.CreateInstance(Type.GetTypeFromProgID(typeName));
        }
    }
}
