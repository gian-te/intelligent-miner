using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntelligentMiner.Common.Enums;
using System.ComponentModel;

namespace IntelligentMiner.Common
{
    public class PlayerMetrics : INotifyPropertyChanged
    {
        private void NotifyPropertyChanged(string str)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(str));
            }
        }

        private string _positionHistory;
        private string _facing;
        private int _scanCount;
        private int _moveCount;
        private int _rotateCount;
        private int _backtrackCount;

        public string PositionHistory { get { return _positionHistory; } set { _positionHistory = value; NotifyPropertyChanged("PositionHistory"); } }

        public string Facing { get { return _facing; } set{ _facing = value; NotifyPropertyChanged("Facing"); } }

        public int scanCount { get { return _scanCount; } set { _scanCount = value; NotifyPropertyChanged("scanCount"); } }

        public int moveCount { get { return _moveCount; } set { _moveCount = value; NotifyPropertyChanged("moveCount"); } }

        public int rotateCount { get { return _rotateCount; } set { _rotateCount = value; NotifyPropertyChanged("rotateCount"); } }

        public int backtrackCount { get { return _backtrackCount; } set { _backtrackCount = value; NotifyPropertyChanged("backtrackCount"); } }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
