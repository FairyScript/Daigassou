using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Melanchall.DryWetMidi.Devices;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Daigassou.Input_Midi
{
    public class MidiController
    {
        private MidiFile midi;
        private List<TrackChunk> tracks;
        public int trackIndex;
        private Playback playback;


        public void OpenFile(string path)
        {
            midi = MidiFile.Read(path);
            tracks = new List<TrackChunk>();
            foreach (var track in midi.GetTrackChunks())
                if (track.ManageNotes().Notes.Count() != 0)
                {
                    tracks.Add(track);
                }
        }
        public List<string> GetTracks()
        {
            try
            {
                var score = new List<string>();

                for (var i = 0; i < tracks.Count; i++)
                {
                    var name = "Untitled";
                    foreach (var trunkEvent in tracks[i].Events)
                        if (trunkEvent is SequenceTrackNameEvent)
                        {
                            var e = trunkEvent as SequenceTrackNameEvent;
                            name = e.Text;
                            break;
                        }

                    score.Add(name);
                }

                return score;
            }
            catch (Exception e)
            {
                MessageBox.Show($"这个Midi文件读取出错！请使用其他软件重新保存。\r\n异常信息：{e.Message}\r\n 异常类型{e.GetType()}",
                    "读取错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public int GetBpm()
        {
            return (int)midi.GetTempoMap().Tempo.AtTime(0).BeatsPerMinute;
        }
        public void SelectTrack(int index)
        {
            trackIndex = index;
        }

        public void StartPlayback()
        {
            //if (time != null) playback.MoveToTime(time);
            playback = new Playback(tracks.ElementAt(trackIndex).Events,midi.GetTempoMap());
            playback.EventPlayed += Playback_EventPlayed;
            playback.Start();
        }

        /// <summary>
        /// 传递一个参数,用于处理播放结束事件
        /// </summary>
        /// <param name="FinishedEvent"></param>
        public void StartPlayback(EventHandler FinishedEvent)
        {
            StartPlayback();
            playback.Finished += FinishedEvent;
        }

        public void StopPlayback()
        {
            if (playback != null) playback.Stop();
        }
        
        private void Playback_EventPlayed(object sender, MidiEventPlayedEventArgs e)
        {
            switch (e.Event)
            {
                case NoteOnEvent @event:
                    {
                        KeyController.KeyboardPress(@event.NoteNumber);
                    }
                    break;
                case NoteOffEvent @event:
                    {
                        KeyController.KeyboardRelease(@event.NoteNumber);
                    }
                    break;
                case SetTempoEvent @event:
                    {
                        
                    }
                    break;
                default:
                    {
                        
                    }
                    break;
            }
        }

        //试听部分
        public int AuditionStart()
        {
            if (midi == null) return -1;
            if (OutputDevice.GetDevicesCount() == 0) return -2;

            playback = new Playback(tracks.ElementAt(trackIndex).Events, midi.GetTempoMap(), OutputDevice.GetByName("Microsoft GS Wavetable Synth"));
            playback.Start();

            return 0;
        }

        public int AuditionPause()
        {
            if (playback == null) return -1;

            if (!playback.IsRunning) return -2;
            playback.Stop();
            return 0;
        }

        public string AuditionInfo()
        {
            var ret = "";
            var expression = @"\d*:(?<time>.+):\d+";
            if (playback.IsRunning)
            {
                var cur = playback.GetCurrentTime(TimeSpanType.Metric);
                var dur = playback.GetDuration(TimeSpanType.Metric);

                ret = Regex.Match(cur.ToString(), expression).Groups["time"].Value + "/" +
                      Regex.Match(dur.ToString(), expression).Groups["time"].Value;
            }

            return ret;
        }

        public int AuditionRestart()
        {
            if (midi == null) return -1;
            if (playback == null) return -2;
            playback.Stop();
            playback.Dispose();
            playback = null;
            return 0;
        }
    }
}
