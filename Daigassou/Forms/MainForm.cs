﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BondTech.HotkeyManagement.Win;
using Daigassou.Input_Midi;
using Daigassou.Properties;
using Daigassou.Utils;
using Newtonsoft.Json;

using static Daigassou.State;
namespace Daigassou
{
    public partial class MainForm : Form
    {
        private readonly KeyController kc = new KeyController();
        private readonly KeyBindFormOld keyForm22 = new KeyBindFormOld();
        private readonly KeyBindForm8Key keyForm8 = new KeyBindForm8Key();
        //private readonly MidiToKey mtk = new MidiToKey();

        
        private Task _runningTask;
        private List<string> _tmpScore;
        private CancellationTokenSource cts = new CancellationTokenSource();
        internal HotKeyManager hkm;
        private ArrayList hotkeysArrayList;
        private int pauseTime = 0;
        //private Queue<KeyPlayList> keyPlayLists;
        private MidiController midiController;
        private NetworkClass net;

        LogForm logForm;
        public MainForm()
        {
            InitializeComponent();
            formUpdate();
            
            KeyBinding.LoadConfig();
            ThreadPool.SetMaxThreads(25, 50);

            CommonUtilities.syncSetting();
            //Task.Run(() => { CommonUtilities.GetLatestVersion(); });
            InitEventHandler();

            Text += $@" Ver{Assembly.GetExecutingAssembly().GetName().Version}";
            if (Settings.Default.IsBeta)
            {
                Text += " Beta";
                toolStripStatusLabel1.Visible = true;
            }
            cbMidiKeyboard.DataSource = KeyboardUtilities.GetKeyboardList();

        }

        private void InitEventHandler()
        {
            //ReadyFlag
            state.PropertyChanged += (sender,s) =>
            {
                if(s.PropertyName == nameof(state.ReadyFlag))
                {
                    if (state.ReadyFlag)
                    {
                        btnSyncReady.Text = "取消准备";
                        btnSyncReady.BackColor = Color.Orange;
                    }
                    else
                    {
                        btnSyncReady.Text = "准备好了";
                        btnSyncReady.BackColor = Color.FromArgb(255, 128, 128);
                    }

                    //准备好了就不要再切轨道了
                    trackComboBox.Enabled = !state.ReadyFlag;
                }

            };

            //IsCaptureFlag
            state.PropertyChanged += (sender, s) =>
            {
                if (s.PropertyName == nameof(state.IsCaptureFlag))
                {
                    if (state.IsCaptureFlag)
                    {
                        btnTimeSync.Text = "停止同步";
                        btnTimeSync.BackColor = Color.Aquamarine;
                    }
                    else
                    {
                        btnTimeSync.Text = "开始同步";
                        btnTimeSync.BackColor = Color.FromArgb(255, 128, 128);
                    }

                }

            };
        }

        private void InitHotKeyBiding()
        {
            try
            {


                hkm = new HotKeyManager(this);
                hkm.Enabled = false;
                if (KeyBinding.hotkeyArrayList == null || KeyBinding.hotkeyArrayList.Count < 5)
                {
                   
                    hotkeysArrayList = new ArrayList();
                    hotkeysArrayList.Clear();
                    hotkeysArrayList.Add(
                        new GlobalHotKey(
                            "Start", Modifiers.Control, Keys.F10, true));
                    hotkeysArrayList.Add(
                        new GlobalHotKey(
                            "Stop", Modifiers.Control, Keys.F11, true));
                    hotkeysArrayList.Add(
                        new GlobalHotKey(
                            "PitchUp", Modifiers.Control, Keys.F8, true));
                    hotkeysArrayList.Add(
                        new GlobalHotKey(
                            "PitchDown", Modifiers.Control, Keys.F9, true));
                    hotkeysArrayList.Add(
                        new GlobalHotKey(
                            "Capture", Modifiers.Control, Keys.F12, true));
                    KeyBinding.hotkeyArrayList = hotkeysArrayList;
                }
                else
                {
                    hotkeysArrayList = KeyBinding.hotkeyArrayList;
                }

                {
                    ((GlobalHotKey) hotkeysArrayList[0]).HotKeyPressed += Start_HotKeyPressed;
                    ((GlobalHotKey) hotkeysArrayList[1]).HotKeyPressed += Stop_HotKeyPressed;
                    ((GlobalHotKey) hotkeysArrayList[2]).HotKeyPressed += PitchUp_HotKeyPressed;
                    ((GlobalHotKey) hotkeysArrayList[3]).HotKeyPressed += PitchDown_HotKeyPressed;
                    ((GlobalHotKey) hotkeysArrayList[4]).HotKeyPressed += Capture_HotKeyPressed;
                }
                var ret = true;
                foreach (GlobalHotKey k in hotkeysArrayList)
                {

                    if (k.Enabled)
                    {
                        try
                        {
                            hkm.AddGlobalHotKey(k);
                        }
                        catch (Exception e)
                        {
                            
                            ret = false;
                        }

                    }

                }

                if (ret == false)
                {
                    
                    throw new Exception();
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(
                    $"一些快捷键无法注册,程序可能无法正常运行。\r\n请检查是否有其他程序占用。\r\n点击下方小齿轮重新配置快捷键+{JsonConvert.SerializeObject(hotkeysArrayList)}",
                    "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            finally
            {
                hkm.Enabled = true;
            }
        }

        private void Capture_HotKeyPressed(object sender, GlobalHotKeyEventArgs e)
        {
            if (!state.RunningFlag && !state.ReadyFlag)
            {
                BtnTimeSync_Click(sender,e);
            }
        }

        private void formUpdate()
        {
            if (Settings.Default.IsEightKeyLayout)
            {
                btn8key.BackgroundImage = Resources.ka1;
                btn22key.BackgroundImage = Resources.kb0;
                btnSwitch.BackgroundImage = Resources.a0;
                btn8key.Enabled = true;
                btn22key.Enabled = false;
            }
            else
            {
                btn8key.BackgroundImage = Resources.ka0;
                btn22key.BackgroundImage = Resources.kb1;
                btnSwitch.BackgroundImage = Resources.a1;
                btn8key.Enabled = false;
                btn22key.Enabled = true;
            }
        }

        private void Start_HotKeyPressed(object sender, GlobalHotKeyEventArgs e)
        {
            if (!state.RunningFlag)
            {
                Log.overlayLog($"快捷键：演奏开始");
                StartKeyPlay(1000);
            }
                
            //else
            //{

            //    Log.overlayLog($"快捷键：演奏恢复");
            //    kc.internalRunningFlag = true;
            //    kc.pauseOffset += Environment.TickCount - pauseTime;
            //}
        }

        private void Stop_HotKeyPressed(object sender, GlobalHotKeyEventArgs e)
        {
            Log.overlayLog($"快捷键：演奏停止");
            StopKeyPlay();
            timer1.Dispose();//释放队列中的播放


        }

        private void StartKeyPlay(int interval)//同步的改变GUI
        {
            try
            {
                StartKeyPlayback(interval);
            }
            catch (IllegalMIDIFileException e)
            {
                MessageBox.Show("没有midi你演奏个锤锤？", "喵喵喵？", MessageBoxButtons.OK, MessageBoxIcon.Question);
                Log.overlayLog($"错误：没有Midi文件");
                return;
            }
            CommonUtilities.SwitchToGameWindow();
            state.ReadyFlag = true;
        }
        private void StopKeyPlay()
        {
            KeyController.ResetKey();
            if (state.RunningFlag)
            {
                
                state.RunningFlag = false;
                midiController.StopPlayback();


            }
            timer1.Dispose();//释放队列中的播放
            state.ReadyFlag = false;
        }
        private void PitchUp_HotKeyPressed(object sender, GlobalHotKeyEventArgs e)
        {
            if (state.RunningFlag)
            {
                ParameterController.GetInstance().Pitch += 1;
                Log.overlayLog($"快捷键：向上移调 当前 {ParameterController.GetInstance().Pitch}");
            }
                
        }

        private void PitchDown_HotKeyPressed(object sender, GlobalHotKeyEventArgs e)
        {
            if (state.RunningFlag)
            {
                ParameterController.GetInstance().Pitch -= 1;
                Log.overlayLog($"快捷键：向下移调 当前 {ParameterController.GetInstance().Pitch}");
            }
                
        }

        /// <summary>
        /// MIDI 文件异常
        /// </summary>
        class IllegalMIDIFileException : ApplicationException
        {
            public string message;
            public IllegalMIDIFileException(string msg) : base(msg)
            {
                message = msg;
            }
        }
        private void StartKeyPlayback(int interval)
        {
            KeyController.ResetKey();
            if (Path.GetExtension(midFileDiag.FileName) != ".mid" && Path.GetExtension(midFileDiag.FileName) != ".midi")
            {                
                throw new IllegalMIDIFileException("没有midi");
            }

            if (!state.RunningFlag)
            {
                
                state.RunningFlag = true;
                timer1.Interval = interval < 1000 ? 1000 : interval;
                var sub = (long) (1000 - interval);
                
                timer1.Start();

                
                //var speed = manualBpmCheckBox.Checked ? (double)(mtk.GetBpm() / nudBpm.Value) : 1;

                Log.overlayLog($"文件名：{Path.GetFileName(midFileDiag.FileName)}");
                Log.overlayLog($"定时：{timer1.Interval / 1000}秒后演奏");
                
            }


        }

        //private Task NewCancellableTask(CancellationToken token)
        //{
        //    return Task.Run(() =>
        //    {
        //        //var keyPlayLists = mtk.ArrangeKeyPlays(mtk.Index);
        //        ParameterController.GetInstance().InternalOffset = (int) networkDelayInput.Value;
        //        ParameterController.GetInstance().Offset = 0;
        //        kc.KeyPlayBack(keyPlayLists, 1, cts.Token);

        //        /* --- Play Over --- */
        //        Invoke(new Action(() => {
        //            state.RunningFlag = false;
        //            state.ReadyFlag = false;
        //        }));
        //        Log.overlayLog($"演奏：演奏结束");
        //        kc.ResetKey();
        //    }, token);
        //}

        private void trackComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            midiController.trackIndex = trackComboBox.SelectedIndex;
        }


        private void selectFileButton_Click(object sender, EventArgs e)
        {
            if (midFileDiag.ShowDialog() == DialogResult.OK)
            {
                midiController = new MidiController();
                midiController.OpenFile(midFileDiag.FileName);
            }
            else
                return;

            pathTextBox.Text = midFileDiag.FileName;
            pathTextBox.SelectionStart = pathTextBox.Text.Length;//固定显示结尾

            _tmpScore = midiController.GetTracks(); //note tracks
            var bpm = midiController.GetBpm();
            var tmp = new List<string>();

            if (_tmpScore != null)
                for (var i = 0; i < _tmpScore.Count; i++)
                    tmp.Add($"track_{i}:{_tmpScore[i]}");


            trackComboBox.DataSource = tmp;
            trackComboBox.SelectedIndex = 0;
            if (bpm >= nudBpm.Maximum)
                nudBpm.Value = nudBpm.Maximum;
            else if (bpm <= nudBpm.Minimum)
                nudBpm.Value = nudBpm.Minimum;
            else
                nudBpm.Value = bpm;
        }


        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            //mtk.Offset = EnumPitchOffset.OctaveLower;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            //mtk.Offset = EnumPitchOffset.None;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            //mtk.Offset = EnumPitchOffset.OctaveHigher;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            //mtk.Bpm = (int) nudBpm.Value;
            
        }

        private void SyncButton_Click(object sender, EventArgs e)
        {
            if (!state.ReadyFlag)
            {
                var interval = dateTimePicker1.Value - DateTime.Now;
                StartKeyPlay((int)interval.TotalMilliseconds + (int)networkDelayInput.Value);
            }
            else
            {
                StopKeyPlay();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            KeyboardUtilities.Disconnect();
            hkm.Enabled = false;
            ArrayList tmp=new ArrayList();
            foreach (GlobalHotKey a in hkm.EnumerateGlobalHotKeys)
            {
                tmp.Add(a);
            }

            foreach (GlobalHotKey VARIABLE in tmp)
            {
                hkm.RemoveGlobalHotKey(VARIABLE);
            }
            hkm.Dispose();
            

        }

        private void button4_Click(object sender, EventArgs e)
        {
            keyForm8.ShowDialog();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            state.RunningFlag = true;
            //cts = new CancellationTokenSource();
            //_runningTask = NewCancellableTask(cts.Token);
            midiController.StartPlayback((_,ee)=> {
                Invoke(new Action(() =>
                {
                    state.RunningFlag = false;
                    state.ReadyFlag = false;
                }));
            });
            
        }


        private void btnKeyboardConnect_Click(object sender, EventArgs e)
        {
            if (cbMidiKeyboard.SelectedItem != null)
                if (cbMidiKeyboard.Enabled)
                {
                    if (KeyboardUtilities.Connect(cbMidiKeyboard.SelectedItem.ToString(), kc) == 0)
                    {
                        cbMidiKeyboard.Enabled = false;
                        btnKeyboardConnect.BackgroundImage = Resources.btn2;
                    }
                }
                else
                {
                    KeyboardUtilities.Disconnect();
                    cbMidiKeyboard.Enabled = true;
                    cbMidiKeyboard.DataSource = KeyboardUtilities.GetKeyboardList();
                    btnKeyboardConnect.BackgroundImage = Resources.btn1;
                }
        }


        protected override void WndProc(ref Message m)
        {
            try
            {
                const int WM_DEVICECHANGE = 0x0219;
                switch (m.Msg)
                {
                    case WM_DEVICECHANGE:
                        cbMidiKeyboard.DataSource = KeyboardUtilities.GetKeyboardList();
                        break;
                }
            }

            catch (Exception ex)
            {
            }

            base.WndProc(ref m);
        }

        private void cbMidiKeyboard_SelectedIndexChanged(object sender, EventArgs e)
        {
            //cbMidiKeyboard.DataSource = KeyboardUtilities.GetKeyboardList();
        }

        private void btnSwitch_Click(object sender, EventArgs e)
        {
            if (Settings.Default.IsEightKeyLayout)
                Settings.Default.IsEightKeyLayout = false;
            else
                Settings.Default.IsEightKeyLayout = true;

            Settings.Default.Save();
            formUpdate();
        }

        private void btn22key_Click(object sender, EventArgs e)
        {
            keyForm22.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            new AboutForm(kc).ShowDialog();
        }


        private void TimeSync()
        {
            double error;
            try
            {
                Log.overlayLog($"时间同步：NTP请求发送");
                var offset = new NtpClient(Settings.Default.NtpServer).GetOffset(out error);
                Log.overlayLog($"时间同步：与北京时间相差{offset.Milliseconds}毫秒");
                //TODO:error handler
                if (CommonUtilities.SetSystemDateTime.SetLocalTimeByStr(
                    DateTime.Now.AddMilliseconds(offset.TotalMilliseconds * -0.5)))
                    tlblTime.Text = "本地时钟已同步";
            }
            catch (Exception e)
            {
                tlblTime.Text = "设置时间出错";
                
            }

           
                
        }


        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (midiController.AuditionStart() == 0)
            {
                btnPlay.BackgroundImage = Resources.c_play_1;
                btnPause.BackgroundImage = Resources.c_pause;
                btnStop.BackgroundImage = Resources.c_stop;

                lblPlay.Text = "正在试听";
                lblMidiName.Text = Path.GetFileNameWithoutExtension(midFileDiag.FileName);
                state.PlayingFlag = true;
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (midiController.AuditionRestart() == 0)
            {
                btnPlay.BackgroundImage = Resources.c_play;
                btnPause.BackgroundImage = Resources.c_pause;
                btnStop.BackgroundImage = Resources.c_stop_1;
                lblPlay.Text = "试听已停止";
                timeLabel.Text = "";
                state.PlayingFlag = false;
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            if (midiController.AuditionPause() == 0)
            {
                btnPlay.BackgroundImage = Resources.c_play;
                btnPause.BackgroundImage = Resources.c_pause_1;
                btnStop.BackgroundImage = Resources.c_stop;
                lblPlay.Text = "试听暂停";
                state.PlayingFlag = false;
            }
        }

        private void BtnTimeSync_Click(object sender, EventArgs e)
        {
            if (state.IsCaptureFlag)
            {

                net._shouldStop = true;
                state.IsCaptureFlag = false;

            }
            else
            {
                TimeSync();

                net = new NetworkClass();

                if (!Settings.Default.IsBeta)
                {
                    state.IsCaptureFlag = true;
                    return;
                }
                net.Play += Net_Play;
                try
                {
                    var ffprocessList = FFProcess.FindFFXIVProcess();
                    if (ffprocessList.Count == 1)
                    {
                        Task.Run(() => { net.Run((uint) FFProcess.FindFFXIVProcess().First().Id); });

                        state.IsCaptureFlag = true;

                    }
                    else if( ffprocessList.Count==2)
                    {
                        if (FFProcess.FindDaigassouProcess().Count>1)
                        {
                            Task.Run(() => { net.Run((uint)FFProcess.FindFFXIVProcess()[1].Id); });
                            
                        }
                        else
                        {
                            Task.Run(() => { net.Run((uint)FFProcess.FindFFXIVProcess().First().Id); });
                            
                        }

                        state.IsCaptureFlag = true;

                    }
                    else
                    {
                        MessageBox.Show("你开游戏了吗？", "喵喵喵？", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    }
                }
                catch (Exception)
                {
                }
            }
        }
        private delegate void remotePlay(int time, int timeStamp, string name);
        private void Net_Play(object sender, PlayEvent e)
        {
            if (this.InvokeRequired)
            {
                if (e.Mode==0)
                {
                    var n = new remotePlay(NetPlay);
                    this.Invoke(n, e.Time,e.TimeStamp, e.Text);
                }
                else if (e.Mode == 1)
                {
                    var n = new remotePlay(NetStop);
                    this.Invoke(n, e.Time, e.TimeStamp, e.Text);
                }
                
            }
            else
            {
                if (e.Mode == 0)
                {
                    NetPlay(e.Time, e.TimeStamp, e.Text);
                }
                else if (e.Mode == 1)
                {
                    NetStop(e.Time, e.TimeStamp, e.Text);
                }
                
            }
            
        }

        private void NetPlay(int time, int timeStamp, string name)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            DateTime dt = startTime.AddSeconds(timeStamp+time);

            dateTimePicker1.Value = dt;
            var msTime = (dt - DateTime.Now).TotalMilliseconds;
            StartKeyPlay((int)msTime + (int)networkDelayInput.Value);
            Log.overlayLog($"网络控制：{name.Trim().Replace("\0", string.Empty)}发起倒计时，目标时间:{dt.ToString("HH:mm:ss")}");
            tlblTime.Text = $"{name.Trim().Replace("\0", string.Empty)}发起倒计时";
        }

        private void NetStop(int time,int timeStamp, string name)
        {
            if (!state.RunningFlag) return;//规避party check的多次取消
            StopKeyPlay();
            Log.overlayLog($"网络控制：{name.Trim().Replace("\0", string.Empty)}停止了演奏");
            tlblTime.Text = $"{name.Trim().Replace("\0", string.Empty)}停止了演奏";
        }
        private void RemoteStart(int Time,string Name)
        {
            
        }
        private void PlayTimer_Tick(object sender, EventArgs e)
        {
            if (state.PlayingFlag) timeLabel.Text = midiController.AuditionInfo();

            timeStripStatus.Text = DateTime.Now.ToString("T");
        }


        private void DateTimePicker1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.Alt && !e.Shift && e.Control && e.KeyCode == Keys.V)
            {
                var targetTime = Convert.ToString(Clipboard.GetDataObject().GetData(DataFormats.Text));
                try
                {
                    var dt = DateTime.ParseExact(targetTime, "HH:mm:ss", CultureInfo.CurrentCulture);
                    dateTimePicker1.Value = dt;
                }
                catch (Exception)
                {
                }
            }
            else if (!e.Alt && !e.Shift && e.Control && e.KeyCode == Keys.C)
            {
                var targetTime = dateTimePicker1.Value.ToString("HH:mm:ss");
                Clipboard.SetDataObject(targetTime);
            }
        }


        private void TrackComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Alt && e.Control && e.Shift && e.KeyCode == Keys.S) mtk.SaveToFile();
        }

        //private void ToolStripSplitButton1_ButtonClick(object sender, EventArgs e)
        //{
            
        //    var form=new ConfigForm(hotkeysArrayList,kc,hkm);
        //    form.ShowDialog();
        //}



        private void MainForm_Load(object sender, EventArgs e)
        {
            InitHotKeyBiding();
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {
            logForm = new LogForm();
            logForm.Show();
        }

        private void toolStripStatusLabel2_Click(object sender, EventArgs e)
        {
            var form = new ConfigForm(hotkeysArrayList, kc, hkm);
            form.ShowDialog();
        }

        private void manualBpmCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            nudBpm.Enabled = (sender as CheckBox).Checked;
        }
    }
}