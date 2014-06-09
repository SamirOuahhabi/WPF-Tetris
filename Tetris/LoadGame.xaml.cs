using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Tetris
{
    /// <summary>
    /// Interaction logic for LoadGame.xaml
    /// </summary>
    public partial class LoadGame : Window
    {
        protected Game _game;
        protected Board _preview;

        public LoadGame(Game game)
        {
            InitializeComponent();
            _game = game;
            loadFromDatabase();
            initGrid();

            _preview = new Board(this, grid, _game.BoardWidth, _game.BoardHeight, Brushes.White, Brushes.White);
        }

        private void loadFromDatabase()
        {
            object[] instances = _game.Database.getAllInstances().ToArray();
            foreach (SavedInstance si in instances)
                list.Items.Add(si);
        }

        protected void initGrid()
        {
            // Preview Grid
            for( int r = 0; r < _game.BoardHeight; r ++ )
                grid.RowDefinitions.Add(new RowDefinition());
            for (int c = 0; c < _game.BoardWidth; c++)
                grid.ColumnDefinitions.Add(new ColumnDefinition());
        }

        private void preview_Click(object sender, RoutedEventArgs e)
        {
            if (list.SelectedItems.Count == 0)
                return;
            SavedInstance si = (SavedInstance)list.SelectedItem;
            _preview.Reset();
            _preview.Load(si.Board);
        }

        private void load_Click(object sender, RoutedEventArgs e)
        {
            if (list.SelectedItems.Count == 0)
                return;
            SavedInstance si = (SavedInstance)list.SelectedItem;
            _preview.Reset();
            _preview.Load(si.Board);
            _game.Load(si);
            Close();
        }
    }
}
