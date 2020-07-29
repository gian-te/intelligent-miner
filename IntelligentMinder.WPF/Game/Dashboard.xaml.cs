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
using IntelligentMiner.Common.Enums;

namespace IntelligentMiner.WPF.Game
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    ///

    public partial class Dashboard : INotifyPropertyChanged
    {
        PlayerMetrics _viewModel;
        Player player;
        public event PropertyChangedEventHandler PropertyChanged;

        public Dashboard(Player p)
        {
            InitializeComponent();

            player = p;
            _viewModel = p.Metrics;
            _viewModel.isPaused = false;
            //_viewModel.gameSpeed = 500;

            this.DataContext = _viewModel;
        }
        private void NotifyPropertyChanged(string str)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(str));
            }
        }

        public void UpdateDashboard(Player p, ActionType action, CellItemType celltype = CellItemType.Empty)
        {
            if (action == ActionType.Rotate)
            {
                _viewModel.Facing = p.Facing.ToString();
                _viewModel.PositionHistory += String.Concat("Rotated to: ", p.Facing.ToString());
                _viewModel.rotateCount = p.Metrics.rotateCount;
            }
            else if (action == ActionType.Scan)
            {
                _viewModel.Facing = p.Facing.ToString();
                _viewModel.PositionHistory += String.Concat("Scanned: ", celltype.ToString());
                _viewModel.scanCount = p.Metrics.scanCount;
            }
            else if (action == ActionType.Move)
            {
                _viewModel.PositionHistory += String.Concat("Moved to: ", p.Position.Row, ", ", p.Position.Column);
                _viewModel.moveCount = p.Metrics.moveCount;
            }
            else if (action == ActionType.Die)
            {
                _viewModel.PositionHistory += String.Concat("The player died a horrible death.");
            }
            else if (action == ActionType.Win)
            {
                _viewModel.PositionHistory += String.Concat("The player has struck gold!");
            }
            else if  (action == ActionType.NoPossible)
            {
                _viewModel.PositionHistory += String.Concat("No more possible moves.\nGame over!");
            }

            _viewModel.PositionHistory += Environment.NewLine;

            _viewModel.backtrackCount = p.Metrics.backtrackCount;
            if(action == ActionType.Win)
            {
                _viewModel.isPaused = false;
            }
            // not advisable
            //Thread.Sleep(_viewModel.gameSpeed);
        }

        private async void txtActions_TextChanged(object sender, TextChangedEventArgs e)
        {
            await Task.Delay(200);
            ScrollToEnd();
        }

        public void ScrollToEnd()
        {
            Task.Run(() =>
                {
                    Dispatcher.Invoke( () => txtActions.ScrollToEnd());

                });
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.isPaused = true;
        }

        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {

            _viewModel.isPaused = false;
        }

        public bool pauseStatus()
        {
            return _viewModel.isPaused;
        }
    }
}
