using IntelligentMiner.WPF.Main;
using IntelligentMiner.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        IntelligentMiner.Common.Game game;
        public GameWindow()
        {
            InitializeComponent();
        }


        public void LoadGame(GameOptions gameOptions)
        {
            game = new Common.Game(gameOptions.Size);
            var gold = new GoldenSquare();
            var goldCoordinates = gameOptions.Gold.Split(',');
            gold.Position.Row = int.Parse(goldCoordinates[0]) ;
            gold.Position.Column = int.Parse(goldCoordinates[1]);
            game.AddGold(gold.Position.Row, gold.Position.Column);
            ReloadGrid();
        }

     

        public void PlayRandom()
        {
            bool end = false;
            Player player = new Player();
            while (!end)
            {
                Thread.Sleep(500);
                // 1. randomize move between Rotate or Move
                var action = player.RandomizeAction();

                if (action == "rotate")
                {
                    // 2. if Rotate, randomize how many times it will rotate 90-degrees
                    player.RotateRandomTimes();
                }
                else if (action == "move")
                {
                    // 3. if Move, randomize how many times it will move
                    var cell = player.Move(game);
                    if (cell.CellItemType == Common.Enums.CellItemType.GoldenSquare || cell.CellItemType == Common.Enums.CellItemType.Pit)
                    {
                        end = true;
                    }

                }
                Dispatcher.Invoke(() => ReloadGrid() );                
                //stepCount++;
                Console.WriteLine();
            }
            
        }

        private void ReloadGrid()
        {
            c_dataGrid.ItemsSource = null;
            c_dataGrid.ItemsSource = GetBindable2DArray<BaseCellItem>(game.Map);
        }

        private void c_dataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            DataGridTextColumn column = e.Column as DataGridTextColumn;
            Binding binding = column.Binding as Binding;
            binding.Path = new PropertyPath(binding.Path.Path + ".Value.Symbol");
        }

        public DataView GetBindable2DArray<T>(T[,] array)
        {
            DataTable dataTable = new DataTable();
            for (int i = 0; i < array.GetLength(1); i++)
            {
                dataTable.Columns.Add(i.ToString(), typeof(Ref<T>));
            }
            for (int i = 0; i < array.GetLength(0); i++)
            {
                DataRow dataRow = dataTable.NewRow();
                dataTable.Rows.Add(dataRow);
            }
            DataView dataView = new DataView(dataTable);
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    int a = i;
                    int b = j;
                    Ref<T> refT = new Ref<T>(() => array[a, b], z => { array[a, b] = z; });
                    dataView[i][j] = refT;
                }
            }
            return dataView;
        }
    }

    public class Ref<T>
    {
        private readonly Func<T> getter;
        private readonly Action<T> setter;
        public Ref(Func<T> getter, Action<T> setter)
        {
            this.getter = getter;
            this.setter = setter;
        }
        public T Value { get { return getter(); } set { setter(value); } }
    }
}
