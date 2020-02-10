using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Daigassou.Input_Midi;
using Daigassou.Properties;
using Daigassou.Utils;

namespace Daigassou
{
    public class KeyController
    {
        private readonly BackgroundKey bkKeyController = new BackgroundKey();

        private static readonly object keyLock = new object();
        private static Keys _lastCtrlKey;
        public volatile bool isBackGroundKey = false;
        public bool internalRunningFlag = true;
        public int pauseOffset = 0;

        [DllImport("User32.dll")]
        public static extern void keybd_event(Keys bVk, byte bScan, int dwFlags, int dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern uint MapVirtualKey(uint uCode, uint uMapType);

        public static void KeyboardPress(int pitch)
        {
            if (pitch <= 84 && pitch >= 48)
            {
                if (Settings.Default.IsEightKeyLayout)
                    KeyboardPress(KeyBinding.GetNoteToCtrlKey(pitch), KeyBinding.GetNoteToKey(pitch));
                //else if (isBackGroundKey)
                //    bkKeyController.BackgroundKeyPress(KeyBinding.GetNoteToKey(pitch));
                else
                    KeyboardPress(KeyBinding.GetNoteToKey(pitch));
                Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")},{(pitch - 24).ToString("X2")} Note On");
                //ParameterController.GetInstance().CheckSyncStatus();
            }
        }

        public static void KeyboardPress(Keys ctrKeys, Keys viKeys)
        {
            keybd_event(_lastCtrlKey, (byte)MapVirtualKey((uint)_lastCtrlKey, 0), 2, 0);
            Thread.Sleep(1);
            if (ctrKeys != Keys.None)
            {
                keybd_event(ctrKeys, (byte)MapVirtualKey((uint)ctrKeys, 0), 0, 0);
                Thread.Sleep(15);
            }

            keybd_event(viKeys, (byte)MapVirtualKey((uint)viKeys, 0), 0, 0);
            _lastCtrlKey = ctrKeys;
        }

        public static void KeyboardPress(Keys viKeys)
        {
            lock (keyLock)
            {
                keybd_event(viKeys, (byte)MapVirtualKey((uint)viKeys, 0), 0, 0);
                Thread.Sleep(1);
            }
        }


        public static void KeyboardRelease(int pitch)
        {
            lock (keyLock)
            {
                if (pitch <= 84 && pitch >= 48)
                {
                    if (Settings.Default.IsEightKeyLayout)
                        KeyboardRelease(KeyBinding.GetNoteToCtrlKey(pitch), KeyBinding.GetNoteToKey(pitch));
                    //else if (isBackGroundKey)
                    //    bkKeyController.BackgroundKeyRelease(KeyBinding.GetNoteToKey(pitch));
                    else
                        KeyboardRelease(KeyBinding.GetNoteToKey(pitch));
                    Console.WriteLine(
                        $"{DateTime.Now.ToString("HH:mm:ss.fff")},{(pitch - 24).ToString("X2")} Note Off");
                }
            }
        }

        public static void KeyboardRelease(Keys ctrKeys, Keys viKeys)
        {
            if (ctrKeys != Keys.None)
                keybd_event(ctrKeys, (byte)MapVirtualKey((uint)ctrKeys, 0), 2, 0);
            Thread.Sleep(1);
            keybd_event(viKeys, (byte)MapVirtualKey((uint)viKeys, 0), 2, 0);
        }


        public static void KeyboardRelease(Keys viKeys)
        {
            keybd_event(viKeys, (byte)MapVirtualKey((uint)viKeys, 0), 2, 0);
        }

        public void InitBackGroundKey(IntPtr pid)
        {
            bkKeyController.Init(pid);
        }

        public static void ResetKey()
        {
            //internalRunningFlag = true;
            //pauseOffset = 0;
            KeyboardRelease(Keys.Control);
            Thread.Sleep(1);
            KeyboardRelease(Keys.Shift);
            Thread.Sleep(1);
            KeyboardRelease(Keys.Alt);
            Thread.Sleep(1);
            for (int i = 48; i < 84; i++)
            {
                KeyboardRelease(KeyBinding.GetNoteToKey(i));
            }
            ParameterController.GetInstance().Pitch = 0;
        }

        public class KeyPlayList
        {
            public enum NoteEvent
            {
                NoteOff,
                NoteOn
            }

            public NoteEvent Ev;
            public int Pitch;
            public long TimeMs;

            public KeyPlayList(NoteEvent ev, int pitch, long timeMs)
            {
                TimeMs = timeMs;
                Ev = ev;
                Pitch = pitch;
            }
        }
    }
}