﻿using IntelligentMiner.WPF.Main;
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
using IntelligentMiner.Common.Utilities;

namespace IntelligentMiner.WPF.Game
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        IntelligentMiner.Common.Game game;
        Player player;
        public GameWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private DataView gridData;
        public DataView GridData { get { return gridData; } set { gridData = value; NotifyPropertyChanged("GridData"); } }

        private void NotifyPropertyChanged(string str)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(str));
            }
        }

        public void LoadGame(GameOptions gameOptions)
        {
            game = new Common.Game(gameOptions.Size);
            player = new Player()
            {
                Position = new Position()
                {
                    Row  = 0,
                    Column = 0
                }
            };
            game.Map[player.Position.Row, player.Position.Column] = player;
            var gold = new GoldenSquare();
            var goldCoordinates = gameOptions.Gold.Split(',');
            gold.Position.Row = int.Parse(goldCoordinates[0]);
            gold.Position.Column = int.Parse(goldCoordinates[1]);
            game.AddGold(gold.Position.Row, gold.Position.Column);
            RefreshGrid();
        }



        public void PlayRandom()
        {
            Task.Run(() =>

            {

                bool end = false;
                while (!end)
                {
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

                    this.Dispatcher.Invoke(() => RefreshGrid());

                    //stepCount++;
                    Console.WriteLine();
                }

            });

        }

        private void RefreshGrid()
        {
            GridData = GetBindable2DArray<BaseCellItem>(game.Map);
            c_dataGrid.ItemsSource = GridData;
        }

        private void c_dataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            DataGridTextColumn column = e.Column as DataGridTextColumn;
            Binding binding = column.Binding as Binding;
            binding.Path = new PropertyPath(binding.Path.Path + ".Value.Symbol");
        }

        /// <summary>
        /// [GIAN] this is wrong
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
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
            for (int i = 0; i < array.GetLength(1); i++)
            {
                for (int j = 0; j < array.GetLength(0); j++)
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
