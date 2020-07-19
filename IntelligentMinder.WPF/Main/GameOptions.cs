using IntelligentMiner.Common.Enums;
using IntelligentMiner.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace IntelligentMiner.WPF.Main
{
    public class GameOptions: INotifyPropertyChanged
    {
        private void NotifyPropertyChanged(string str)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(str));
            }
        }
        private string _gold;
        private string _beacons;
        private string _pits;

        public int Size { get; set; }

        public bool MovesIntelligently { get; set; }

        public bool MovesRandomly { get; set; }

        public bool RandomInit { get; set; }

        public bool ManualInit { get; set; }

        public string pits { get { return _pits; } set { _pits = value; NotifyPropertyChanged("pits"); } }

        public string beacons { get { return _beacons; } set { _beacons = value; NotifyPropertyChanged("beacons"); } }

        public string Gold { get { return _gold; } set { _gold = value; NotifyPropertyChanged("Gold"); } }

        public List<string> Pits { get; set; }

        public List<string> Beacons { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
