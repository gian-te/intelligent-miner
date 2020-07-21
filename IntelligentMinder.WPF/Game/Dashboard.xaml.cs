using IntelligentMiner.WPF.Main;
using IntelligentMiner.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Data;
using System.Threading;

namespace IntelligentMiner.WPF.Game
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    ///

    public partial class Dashboard : INotifyPropertyChanged
    {
        PlayerInfo _viewModel;
        Player player;
        public event PropertyChangedEventHandler PropertyChanged;

        public Dashboard(Player p)
        {
            InitializeComponent();

            player = p;
            _viewModel = new PlayerInfo()
            {
                Facing = p.Facing.ToString(),
                PositionHistory = "Starting at position: 0,0\r\nFacing: " + p.Facing.ToString() + "\r\n",
                scanCount = p.scanCount,
                moveCount = p.moveCount,
                rotateCount = p.rotateCount,
                backtrackCount = p.backtrackCount
            };

            this.DataContext = _viewModel;
        }
        private void NotifyPropertyChanged(string str)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(str));
            }
        }

        public void UpdateDashboard(Player p, string action)
        {
            if (action == "rotate")
            {
                _viewModel.Facing = p.Facing.ToString();
                _viewModel.PositionHistory += String.Concat("Rotated: ", p.Facing.ToString());
                _viewModel.rotateCount = p.rotateCount;
            }
            else if (action == "move")
            {
                _viewModel.PositionHistory += String.Concat("Moved to: ", p.Position.Row, ", ", p.Position.Column);
                _viewModel.moveCount = p.moveCount;
            }
            _viewModel.PositionHistory += Environment.NewLine;

            _viewModel.scanCount = p.scanCount;
            _viewModel.backtrackCount = p.backtrackCount;
        }
    }
}
