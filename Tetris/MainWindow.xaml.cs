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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tetris
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Game : Window
    {
        protected const int _height = 18;
        protected const int _width = 10;
        protected Board _board;
        protected Timer _blockMoveTimer;
        protected Block _block;

        public Game()
        {
            InitializeComponent();
            initGrid();
            _blockMoveTimer = new Timer();
            _blockMoveTimer.Interval = 700;
            _blockMoveTimer.Tick += new EventHandler(blockStepTimer_Tick);
            _blockMoveTimer.Enabled = false;

            _board = new Board(this, grid, _width, _height, Brushes.White, Brushes.Black);
        }

        private void blockStepTimer_Tick(object sender, EventArgs e)
        {
            if(_block != null)
            {
                Debug.WriteLine("_block is not null, dropping...");
                if (!_board.dropBlock(_block))
                {
                    Debug.WriteLine("Cannot drop _block, setting it to null.");
                    _block = null;
                }
            }
            else
            {
                Debug.WriteLine("_block is null, spawning...");
                spawnBlock();
            }
        }

        private void spawnBlock()
        {
            _block = new Block();
            _block.Coordinates = new Point(4, 0);
            if (_board.canSpawn(_block))
            {
                Debug.WriteLine("_block spawned, drawing...");
                _board.Draw(_block);
            }
            else
            {
                _block = null;
                Debug.WriteLine("_block cannot spawn...\n GAME OVER");
                _blockMoveTimer.Stop();
            }
        }

        protected void initGrid()
        {
            for( int r = 0; r < _height; r ++ )
                grid.RowDefinitions.Add(new RowDefinition());

            ColumnDefinition col = new ColumnDefinition();
            for (int c = 0; c < _width; c++)
                grid.ColumnDefinitions.Add(new ColumnDefinition());
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                Debug.WriteLine("Enter key pressed.");
                if (_block == null)
                {
                    Debug.WriteLine("_block is null, spawning...");
                    spawnBlock();
                }
            }
            if(e.Key == Key.F2)
            {
                Debug.WriteLine("F2 key pressed.");
                _blockMoveTimer.Enabled = !_blockMoveTimer.Enabled;
            }
            if (e.Key == Key.Left)
            {
                if (_block != null)
                    _board.moveLeft(_block);
            }
            if (e.Key == Key.Right)
            {
                if (_block != null)
                    _board.moveRight(_block);
            }
            if (e.Key == Key.Down)
            {
                if (_block != null)
                    _board.dropBlock(_block);
            }
            if (e.Key == Key.Up)
            {
                if (_block != null)
                    _board.rotateBlock(_block);
            }
        }
    }
}
