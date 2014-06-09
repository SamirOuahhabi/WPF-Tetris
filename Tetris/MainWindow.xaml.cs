using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Resources;
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
        protected const int _predictHeight = 6;
        protected const int _predictWidth = 6;
        protected Board _board;
        protected Board _predict;
        protected Timer _blockMoveTimer;
        protected Block _block;
        protected Block _nextBlock;
        protected int _level;
        protected int _score;
        protected int _linesCleared;
        protected Random _rand;
        protected SoundLibrary _soundLib;
        protected SQLiteDatabase _database;

        // TO DO LIST:
        // - Implement SQLite
        // - LeaderBoard stored in SQLite
        // - Config stored in SQLite

        public Game()
        {
            InitializeComponent();
            initGrids();

            _database = new SQLiteDatabase("tetris.db");

            Icon = Util.ConvertToImageSource(Properties.Resources.tetris);

            _blockMoveTimer = new Timer();
            _blockMoveTimer.Interval = 500;
            _blockMoveTimer.Tick += new EventHandler(blockStepTimer_Tick);
            _blockMoveTimer.Enabled = false;

            _board = new Board(this, grid, _width, _height, Brushes.White, Brushes.Gold);
            _predict = new Board(this, predictGrid, _predictHeight, _predictWidth, Brushes.Beige, Brushes.White);
            _rand = new Random((int)DateTime.Now.Ticks);
            _soundLib = new SoundLibrary();

            _score = 0;
            _level = 1;
            _linesCleared = 0;
            updateScoreBoard(0);
        }

        private void blockStepTimer_Tick(object sender, EventArgs e)
        {
            if(_block != null)
            {
                Debug.WriteLine("_block is not null, dropping...");
                if (!_board.dropBlock(_block))
                {
                    int lines = _board.checkLines();
                    updateScoreBoard(lines);
                    Debug.WriteLine("Cannot drop _block, setting it to null.");
                    _block = null;
                }
                else
                    _soundLib.tic();
            }
            else
            {
                Debug.WriteLine("_block is null, spawning...");
                spawnBlock();
            }
        }

        protected void updateScoreBoard(int lines)
        {
            if(lines>0)
            {
                _score += _level * (100 + 150 * (lines - 1));
                _linesCleared += lines;
                _level = _linesCleared / 10 + 1;
                _blockMoveTimer.Interval = (int) (500 / (Math.Pow(1.25, _level - 1)));
                if (lines == 4)
                    _soundLib.Yeah();
                else
                    _soundLib.Success();
            }
            scoreBoard.Content = string.Format("Level {0}\nScore: {1}\nLines cleared: {2}\nTime interval: {3}", 
                _level, _score, _linesCleared, _blockMoveTimer.Interval);
        }

        private void spawnBlock()
        {
            if(_nextBlock==null)
            {
                _nextBlock = new Block(_rand);
                _nextBlock.Coordinates = new Point(2,2);
                Debug.WriteLine("Next block spawned: "+_nextBlock.Type);
            }

            _block = _nextBlock;
            _block.Coordinates = new Point(_rand.Next(3, _width-3), 0);
            Debug.WriteLine("_block becomes: " + _block.Type);
            _nextBlock = new Block(_rand);
            _nextBlock.Coordinates = new Point(2, 2);
            Debug.WriteLine("Next block spawned: " + _nextBlock.Type);

            if (_board.canSpawn(_block))
            {
                Debug.WriteLine("_block spawned, drawing...");
                _predict.Reset();
                _predict.Draw(_nextBlock);
                _board.Draw(_block);
            }
            else
            {
                _block = null;
                Debug.WriteLine("_block cannot spawn...\n GAME OVER");
                _blockMoveTimer.Stop();
                _board.gameOver();
                _predict.gameOver();
                _soundLib.gameOver();
                DialogResult dialogResult = System.Windows.Forms.MessageBox.Show(
                    "Game Over!\nWould you like to play again?", "Game Over", MessageBoxButtons.YesNo);
                if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                {
                    _board.Reset();
                    Reset();
                    _blockMoveTimer.Start();
                }
                else if (dialogResult == System.Windows.Forms.DialogResult.No)
                {
                    this.Close();
                }
            }
        }

        protected void Reset()
        {
            _score = 0;
            _level = 0;
            _linesCleared = 0;
            _blockMoveTimer.Interval = 500;
            updateScoreBoard(0);
            _block = null;
            _nextBlock = null;
        }

        protected void initGrids()
        {
            // Main Grid
            for( int r = 0; r < _height; r ++ )
                grid.RowDefinitions.Add(new RowDefinition());
            for (int c = 0; c < _width; c++)
                grid.ColumnDefinitions.Add(new ColumnDefinition());

            //Prediction Grid
            for (int r = 0; r < _predictHeight; r++)
                predictGrid.RowDefinitions.Add(new RowDefinition());
            for (int c = 0; c < _predictWidth; c++)
                predictGrid.ColumnDefinitions.Add(new ColumnDefinition());
        }

        private void ExitMenu_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == Key.F2)
            {
                Debug.WriteLine("F2 key pressed.");
                _blockMoveTimer.Enabled = !_blockMoveTimer.Enabled;
            }
            if (e.Key == Key.Left)
            {
                if (_block != null)
                {
                    _board.moveLeft(_block);
                    _soundLib.tic();
                }
            }
            if (e.Key == Key.Right)
            {
                if (_block != null)
                {
                    _board.moveRight(_block);
                    _soundLib.tic();
                }
            }
            if (e.Key == Key.Down)
            {
                if (_block != null)
                {
                    _board.dropBlock(_block);
                    _soundLib.tic();
                }
            }
            if (e.Key == Key.Up)
            {
                if (_block != null)
                    _board.rotateBlock(_block);
            }
            if(e.Key == Key.Home)
            {
                _linesCleared += 10;
                _level = _linesCleared / 10 + 1;
                _blockMoveTimer.Interval = (int)(500 / (Math.Pow(1.25, _level - 1))); 
                scoreBoard.Content = string.Format("Level {0}\nScore: {1}\nLines cleared: {2}\nTime interval: {3}",
                    _level, _score, _linesCleared, _blockMoveTimer.Interval);
            }
        }

        private void SaveMenu_Click(object sender, RoutedEventArgs e)
        {
            _blockMoveTimer.Enabled = false;
            StringBuilder b = new StringBuilder(_board.ToString());
            if(_block != null)
                foreach (Point p in _block.Shape)
                    b[(int)(p.Y + _block.Coordinates.Y) * _width + (int) (p.X + _block.Coordinates.X)] = '#';

            string name = Util.ShowDialog("Choose a name:", "Save as");
            Debug.WriteLine(name);

            SavedInstance si = new SavedInstance(name, b.ToString(), _score, _linesCleared);
            _database.saveNewInstance(si);
        }

        private void LoadMenu_Click(object sender, RoutedEventArgs e)
        {
            _blockMoveTimer.Enabled = false;
            /*SavedInstance si = _database.getSavedInstanceById(1);
            Reset();
            _board.Reset();
            _board.Load(si.Board);
            _block = null;
            _nextBlock = null;
            _score = si.Score;
            _linesCleared = si.LinesCleared;
            _level = _linesCleared / 10 + 1;
            _blockMoveTimer.Interval = (int)(500 / (Math.Pow(1.25, _level - 1)));
            scoreBoard.Content = string.Format("Level {0}\nScore: {1}\nLines cleared: {2}\nTime interval: {3}",
                _level, _score, _linesCleared, _blockMoveTimer.Interval);*/
            LoadGame l = new LoadGame();
            l.Show();
        }
    }
}
