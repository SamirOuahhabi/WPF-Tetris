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
        protected LoadGame _loadGame;
        protected int _highScore;
        protected string _playerName;
        protected bool _gameRunning;
        protected AboutBox _about;

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
            _gameRunning = false;

            _board = new Board(this, grid, _width, _height, Brushes.White, Brushes.LightBlue);
            _predict = new Board(this, predictGrid, _predictHeight, _predictWidth, Brushes.Beige, Brushes.White);
            _rand = new Random((int)DateTime.Now.Ticks);
            _soundLib = new SoundLibrary();

            _score = 0;
            _level = 1;
            _linesCleared = 0;
            _highScore = _database.getHighScore();
            updateScoreBoard(0);

            _playerName = "Player1";
            nameLabel.Content = string.Format("Name: {0}", _playerName);
            handleMenuItems();
        }

        private void handleMenuItems()
        {
            newMenu.IsEnabled = true;
            if(_gameRunning)
            {
                if(_blockMoveTimer.Enabled)
                {
                    pauseMenu.IsEnabled = true;
                    resumeMenu.IsEnabled = false;
                    loadMenu.IsEnabled = false;
                    saveMenu.IsEnabled = false;
                    pausedLabel.Text = "";
                }
                else
                {
                    pauseMenu.IsEnabled = false;
                    resumeMenu.IsEnabled = true;
                    loadMenu.IsEnabled = true;
                    saveMenu.IsEnabled = true;
                    pausedLabel.Text = "Paused";
                }
            }
            else
            {
                pauseMenu.IsEnabled = false;
                resumeMenu.IsEnabled = false;
                loadMenu.IsEnabled = true;
                saveMenu.IsEnabled = false;
            }
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
            scoreBoard.Content = string.Format("Level {0}\nScore: {1}\nLines cleared: {2}\nHigh-score: {3}", 
                _level, _score, _linesCleared, _highScore);
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

                if(_score > _database.getHighScore())
                    System.Windows.Forms.MessageBox.Show(
                    "Congrats "+_playerName+"! You got a new highscore!", "New Highscore", MessageBoxButtons.OK);

                if (_score > 0)
                    _database.saveNewScore(_score, _playerName);

                _highScore = _database.getHighScore();
                updateScoreBoard(0);

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
            _level = 1;
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
            DialogResult dialogResult = System.Windows.Forms.MessageBox.Show(
                        "Are you sure you want to exit the program?", "Exit", MessageBoxButtons.YesNo);
            if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                Close();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.N)
            {
                if (_gameRunning)
                {
                    DialogResult dialogResult = System.Windows.Forms.MessageBox.Show(
                        "Another game is already running, would you like to reset the board?", "Game running", MessageBoxButtons.YesNo);
                    if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                    {
                        _board.Reset();
                        Reset();
                        _blockMoveTimer.Start();
                    }
                }
                else
                {
                    _board.Reset();
                    Reset();
                    _blockMoveTimer.Start();
                    _gameRunning = true;
                }

                handleMenuItems();
            }

            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.P)
            {
                if (_gameRunning && _blockMoveTimer.Enabled)
                    _blockMoveTimer.Enabled = false;

                handleMenuItems();
            }

            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.G)
            {
                if (_gameRunning && !_blockMoveTimer.Enabled)
                    _blockMoveTimer.Enabled = true;

                handleMenuItems();
            }

            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.S)
            {
                SaveMenu_Click(null, null);
            }

            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.O)
            {
                LoadMenu_Click(null, null);
            }

            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.X)
            {
                DialogResult dialogResult = System.Windows.Forms.MessageBox.Show(
                        "Are you sure you want to exit the program?", "Exit", MessageBoxButtons.YesNo);
                if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                    Close();
            }

            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.A)
                AboutMenu_Click(null, null);

            if (_blockMoveTimer.Enabled && e.Key == Key.Left)
            {
                if (_block != null)
                {
                    _board.moveLeft(_block);
                    _soundLib.tic();
                }
            }
            if (e.Key == Key.Right)
            {
                if (_blockMoveTimer.Enabled && _block != null)
                {
                    _board.moveRight(_block);
                    _soundLib.tic();
                }
            }
            if (e.Key == Key.Space)
            {
                if (_blockMoveTimer.Enabled && _block != null)
                {
                    _board.dropBlock(_block);
                    _soundLib.tic();
                }
            }
            if (e.Key == Key.Up)
            {
                if (_blockMoveTimer.Enabled && _block != null)
                    _board.rotateBlock(_block, "up");
            }
            if (e.Key == Key.Down)
            {
                if (_blockMoveTimer.Enabled && _block != null)
                    _board.rotateBlock(_block, "down");
            }
            if(e.Key == Key.Home)
            {
                if (_gameRunning)
                {
                    _linesCleared += 10;
                    _level = _linesCleared / 10 + 1;
                    _blockMoveTimer.Interval = (int)(500 / (Math.Pow(1.25, _level - 1)))+1;
                    updateScoreBoard(0);
                }
            }
        }

        private void SaveMenu_Click(object sender, RoutedEventArgs e)
        {
            if (_gameRunning && !_blockMoveTimer.Enabled)
            {
                StringBuilder b = new StringBuilder(_board.ToString());
                if (_block != null)
                    foreach (Point p in _block.Shape)
                        b[(int)(p.Y + _block.Coordinates.Y) * _width + (int)(p.X + _block.Coordinates.X)] = '#';

                string name = Util.ShowDialog("Choose a name:", "Save as");
                Debug.WriteLine(name);

                SavedInstance si = new SavedInstance(name, b.ToString(), _score, _linesCleared);
                _database.saveNewInstance(si);
            }
            handleMenuItems();
        }

        private void LoadMenu_Click(object sender, RoutedEventArgs e)
        {
            if (!_blockMoveTimer.Enabled)
            {
                _loadGame = new LoadGame(this);
                _loadGame.Show();
            }
            handleMenuItems();
        }

        private void NewMenu_Click(object sender, RoutedEventArgs e)
        {
            if (_gameRunning)
            {
                DialogResult dialogResult = System.Windows.Forms.MessageBox.Show(
                    "Another game is already running, would you like to reset the board?", "Game running", MessageBoxButtons.YesNo);
                if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                {
                    _board.Reset();
                    Reset();
                    _blockMoveTimer.Start();
                }
            }
            else
            {
                _board.Reset();
                Reset();
                _blockMoveTimer.Start();
                _gameRunning = true;
            }

            handleMenuItems();
        }

        private void PauseMenu_Click(object sender, RoutedEventArgs e)
        {
            if (_gameRunning && _blockMoveTimer.Enabled)
                _blockMoveTimer.Enabled = false;

            handleMenuItems();
        }

        private void ResumeMenu_Click(object sender, RoutedEventArgs e)
        {
            if (_gameRunning && !_blockMoveTimer.Enabled)
                _blockMoveTimer.Enabled = true;

            handleMenuItems();
        }

        public void Load(SavedInstance si)
        {
            Reset();
            _board.Reset();
            _board.Load(si.Board);
            _block = null;
            _nextBlock = null;
            _score = si.Score;
            _linesCleared = si.LinesCleared;
            _level = _linesCleared / 10 + 1;
            _blockMoveTimer.Interval = (int)(500 / (Math.Pow(1.25, _level - 1)));
            updateScoreBoard(0);
            _blockMoveTimer.Enabled = true;
            _gameRunning = true;
            handleMenuItems();
        }

        public SQLiteDatabase Database
        {
            get
            {
                return _database;
            }
        }

        public int BoardWidth
        {
            get
            {
                return _width;
            }
        }

        public int BoardHeight
        {
            get
            {
                return _height;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (_loadGame != null && _loadGame.IsEnabled)
                _loadGame.Close();
        }

        private void change_Click(object sender, RoutedEventArgs e)
        {
            _playerName = Util.ShowDialog("Enter a new name:", "Change player name");
            nameLabel.Content = string.Format("Name: {0}", _playerName);
            var scope = FocusManager.GetFocusScope(change);
            FocusManager.SetFocusedElement(scope, null);
            Keyboard.ClearFocus();
            Focus();
        }

        private void AboutMenu_Click(object sender, RoutedEventArgs e)
        {
            if(_about == null || _about.IsDisposed)
            {
                _about = new AboutBox();
                _about.Show();
            }
            else
                if(!_about.Visible)
                    _about.Show();
        }
    }
}
