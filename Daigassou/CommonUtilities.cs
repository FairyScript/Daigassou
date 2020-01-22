﻿using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Daigassou.Utils;
using Newtonsoft.Json;
//using UpdateHelper;

namespace Daigassou
{
    internal class CommonUtilities
    {

        public static async void GetLatestVersion()
        {
            //我就不检查,你能奈我何
            return;

            var wc = new WebClient();
            try
            {
                var nowVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

                var newVersionJson = "{}";//await UpdateHelper.UpdateHelper.CheckUpdate();
                try
                {
                    var versionObj = JsonConvert.DeserializeObject<versionObject>(newVersionJson);
                    if (versionObj.isRefuseToUse)
                    {
                        Environment.Exit(-1);
                    }
                    if (nowVersion != versionObj.Version)
                        if (MessageBox.Show($"检测到新版本{versionObj.Version}已经发布，点击确定下载最新版哦！\r\n " +
                                            $"当然就算你点了取消，这个提示每次打开还会出现的哦！" +
                                            $"下载错误可以去NGA发布帖哦！bbs.nga.cn/read.php?tid=18790669 \r\n" +
                                            $"新版本更新内容：{versionObj.Description}", "哇——更新啦！",
                                MessageBoxButtons.OKCancel,
                                MessageBoxIcon.Information) == DialogResult.OK)
                        {
                            Process.Start("http://blog.ffxiv.cat/index.php/download/");

                        }
                    if (versionObj.isForceUpdate)
                    {
                        Environment.Exit(-2);
                    }

                    if (versionObj.isBeta)
                    {
                        Log.isBeta = true;
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                
            }
            catch (Exception e)
            {
            }
        }
        public class versionObject
        {
            public bool isForceUpdate { get; set; }
            public bool isRefuseToUse { get; set; }
            public string Version { get; set; }
            public string Description { get; set; }
            public bool isBeta { get; set; }
        }
        public static void WriteLog(string msg)
        {
            Console.WriteLine($"{DateTime.Now.ToString("O")}\t\t\t" + $"{msg}");
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SystemTime
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMiliseconds;
        }

        public class SetSystemDateTime
        {
            [DllImport("Kernel32.dll")]
            public static extern bool SetLocalTime(ref SystemTime sysTime);

            public static bool SetLocalTimeByStr(DateTime dt)
            {
                bool flag = false;
                SystemTime sysTime = new SystemTime();
                sysTime.wYear = Convert.ToUInt16(dt.Year);
                sysTime.wMonth = Convert.ToUInt16(dt.Month);
                sysTime.wDay = Convert.ToUInt16(dt.Day);
                sysTime.wHour = Convert.ToUInt16(dt.Hour);
                sysTime.wMinute = Convert.ToUInt16(dt.Minute);
                sysTime.wSecond = Convert.ToUInt16(dt.Second);
                sysTime.wMiliseconds = Convert.ToUInt16(dt.Millisecond);
                try
                {
                    flag = SetSystemDateTime.SetLocalTime(ref sysTime);
                }
                catch (Exception e)
                {
                    WriteLog("Failed to set system date time with exception "+e.Message);
                }

                return flag;
            }
        }

        public static void syncSetting()
        {
            if (Settings.Default.UpgradeRequired)
            {
                Console.WriteLine("检测到版本变动！开始读取旧版本设置");
                Settings.Default.Upgrade();
                Settings.Default.UpgradeRequired = false;
                Settings.Default.Save();
            }
        }
    }
}