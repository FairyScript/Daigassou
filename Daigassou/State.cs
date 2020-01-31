
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Daigassou
{
    class State : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private State() { }

        public static readonly State state = new State();

        private bool _playingFlag;
        public bool PlayingFlag
        {
            get => _playingFlag;
            set
            {
                if (value != _playingFlag)
                {
                    _playingFlag = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _readyFlag;
        public bool ReadyFlag
        {
            get => _readyFlag;
            set
            {
                if(value != _readyFlag)
                {
                    _readyFlag = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _runningFlag;
        public bool RunningFlag
        {
            get => _runningFlag;
            set
            {
                if (value != _runningFlag)
                {
                    _runningFlag = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isCaptureFlag;
        public bool IsCaptureFlag
        {
            get => _isCaptureFlag;
            set
            {
                if (value != _isCaptureFlag)
                {
                    _isCaptureFlag = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}