using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Tetris
{
    public class Board
    {
        protected Rectangle[,] _cells;
        protected SolidColorBrush _background;
        protected SolidColorBrush _borders;
        protected bool[,] _occupied;
        protected int _height;
        protected int _width;

        public Board(Window window, Grid grid, int width, int height, SolidColorBrush background, SolidColorBrush borders)
        {
            _cells = new Rectangle[height, width];
            _occupied = new bool[height, width];
            _background = background;
            _borders = borders;
            _width = width;
            _height = height;

            for( int r = 0; r < height; r ++ )
                for( int c = 0; c < width; c ++ )
                {
                    _occupied[r, c] = false;
                    _cells[r, c] = new Rectangle();
                    _cells[r, c].Name = grid.Name+"_rect_" + r + "_" + c;
                    window.RegisterName(_cells[r, c].Name, _cells[r, c]);
                    _cells[r, c].Stretch = Stretch.Fill;
                    _cells[r, c].Fill = _background;
                    _cells[r, c].Stroke = _borders;

                    Grid.SetColumn(_cells[r, c], c);
                    Grid.SetRow(_cells[r, c], r);

                    grid.Children.Add(_cells[r, c]);
                }
        }

        public void Reset()
        {
            for( int r = 0; r < _height; r ++ )
                for (int c = 0; c < _width; c++)
                {
                    _occupied[r, c] = false;
                    _cells[r, c].Fill = _background;
                    _cells[r, c].Stroke = _borders;
                }
        }

        public void Load(string board)
        {
            StringBuilder str = new StringBuilder(board);
            int i, r, c;
            for (i = 0; i < str.Length; i ++ )
            {
                r = i / _width;
                c = i % _width;
                if (str[i] == 'T')
                {
                    _occupied[r, c] = true;
                    _cells[r, c].Fill = Block.TEE;
                }

                if (str[i] == 'E')
                {
                    _occupied[r, c] = true;
                    _cells[r, c].Fill = Block.EL;
                }

                if (str[i] == 'L')
                {
                    _occupied[r, c] = true;
                    _cells[r, c].Fill = Block.LINE;
                }

                if (str[i] == 'J')
                {
                    _occupied[r, c] = true;
                    _cells[r, c].Fill = Block.JAY;
                }

                if (str[i] == 'Z')
                {
                    _occupied[r, c] = true;
                    _cells[r, c].Fill = Block.ZSKEW;
                }

                if (str[i] == 'S')
                {
                    _occupied[r, c] = true;
                    _cells[r, c].Fill = Block.SSKEW;
                }

                if (str[i] == 'Q')
                {
                    _occupied[r, c] = true;
                    _cells[r, c].Fill = Block.SQUARE;
                }
            }
        }

        public bool isOccupied(int r, int c)
        {
            if (!inBound(r, c))
                return false;
            return _occupied[r, c];
        }

        public bool canSpawn(Block b)
        {
            for(int i=0;i<b.Shape.Length;i++)
            {
                if (isOccupied((int)(b.Shape[i].Y + b.Coordinates.Y), (int)(b.Shape[i].X + b.Coordinates.X)))
                {
                    Debug.WriteLine("({0}, {1}) is occupied...", (int)(b.Shape[i].X + b.Coordinates.X), 
                        (int)(b.Shape[i].Y + b.Coordinates.Y));
                    return false;
                }
            }
            return true;
        }

        public bool moveRight(Block b)
        {
            Erase(b);


            for (int i = 0; i < b.Shape.Length; i++)
            {
                if (isOccupied((int)(b.Shape[i].Y + b.Coordinates.Y),
                        (int)(b.Shape[i].X + b.Coordinates.X) + 1))
                {
                    Draw(b);
                    return false;
                }
            }

            b.Coordinates = new Point(b.Coordinates.X + 1, b.Coordinates.Y);
            adjustBlock(b);
            Draw(b);

            return true;
        }

        public bool moveLeft(Block b)
        {

            Erase(b);
            for (int i = 0; i < b.Shape.Length; i++)
            {
                if (isOccupied((int)(b.Shape[i].Y + b.Coordinates.Y),
                        (int)(b.Shape[i].X + b.Coordinates.X) - 1))
                {
                    Draw(b);
                    return false;
                }
            }

            b.Coordinates = new Point(b.Coordinates.X - 1, b.Coordinates.Y);
            adjustBlock(b);
            Draw(b);

            return true;
        }

        public void Draw(Block b)
        {
            int r, c;
            for (int i = 0; i < b.Shape.Length; i++)
            {
                r = (int)(b.Shape[i].Y + b.Coordinates.Y);
                c = (int)(b.Shape[i].X + b.Coordinates.X);
                if (inBound(r, c))
                {
                    _cells[r, c].Fill = b.Color;
                    _occupied[r, c] = true;
                }
            }
        }

        public void Erase(Block b)
        {
            int r, c;
            for (int i = 0; i < b.Shape.Length; i++)
            {
                r = (int)(b.Shape[i].Y + b.Coordinates.Y);
                c = (int)(b.Shape[i].X + b.Coordinates.X);
                if (inBound(r, c))
                {
                    _cells[r, c].Fill = _background;
                    _occupied[r, c] = false;
                }
            }
        }

        private bool inBound(int r, int c)
        {
            if (r < 0 || r >= _height || c < 0 || c >= _width)
                return false;
            return true;
        }

        public bool dropBlock(Block _block)
        {
            Erase(_block);
            for(int i=0; i<_block.Shape.Length; i++)
            {
                if( (_block.Shape[i].Y + _block.Coordinates.Y)+1 >= _height || 
                    isOccupied((int)(_block.Shape[i].Y + _block.Coordinates.Y)+1,
                    (int)(_block.Shape[i].X + _block.Coordinates.X)))
                {
                    Draw(_block);
                    return false;
                }
            }

            _block.Coordinates = new Point(_block.Coordinates.X, _block.Coordinates.Y + 1);
            Draw(_block);

            return true;
        }

        public bool rotateBlock(Block _block)
        {
            Erase(_block);

            for (int i = 0; i < _block.Shape.Length; i++ )
                if (isOccupied((int)(_block.NextRotation[i].Y + _block.Coordinates.Y),
                    (int)(_block.NextRotation[i].X + _block.Coordinates.X)))
                {
                    Draw(_block);
                    return false;
                }

            _block.Rotate();
            adjustBlock(_block);
            Draw(_block);
            return true;
        }

        private void adjustBlock(Block _block)
        {
            Erase(_block);
            int minL = 0;
            int maxR = 0;
            for (int i = 0; i < _block.Shape.Length; i++)
            {
                if(_block.Shape[i].X+_block.Coordinates.X<minL)
                {
                    minL = (int)_block.Shape[i].X + (int)_block.Coordinates.X;
                    if (minL != 0)
                        Debug.WriteLine("Block is off by " + minL + " to the left");
                }

                if(_block.Shape[i].X+_block.Coordinates.X>=(maxR+_width))
                {
                    maxR = (int)_block.Shape[i].X + (int)_block.Coordinates.X - _width + 1;
                    Debug.WriteLine("maxR is " + maxR);
                    if (maxR != 0)
                        Debug.WriteLine("Block is off by " + maxR + " to the right");
                }
            }
            _block.Coordinates = new Point(_block.Coordinates.X-minL-maxR, _block.Coordinates.Y);
            Draw(_block);
        }

        public int checkLines()
        {
            bool[] lines = new bool[_height];
            int numCompleted = 0;
            for(int r=_height-1; r >= 0; r-- )
            {
                lines[r] = true;
                for(int c=0; c<_width; c++)
                    if (!isOccupied(r, c))
                        lines[r] = false;
            }
            for (int i = 0; i < lines.Length; i++)
                if (lines[i])
                {
                    numCompleted++;
                    resetLine(i);
                }
            return numCompleted;
        }

        protected void resetLine(int row)
        {
            for (int c = 0; c < _width; c++)
                _cells[row, c].Fill = _background;

            Debug.WriteLine("Resetting line {0}", row);
            for(int r = row - 1; r >= 0; r --)
                for(int c = 0; c < _width; c ++)
                {
                    _occupied[r + 1, c] = _occupied[r, c];
                    _cells[r + 1, c].Fill = _cells[r, c].Fill;
                    _cells[r, c].Fill = _background;
                    _occupied[r, c] = false;
                }
        }

        public void gameOver()
        {
            for (int r = 0; r < _height; r++)
                for (int c = 0; c < _width; c++)
                {
                    if (_occupied[r, c])
                        _cells[r, c].Fill = Brushes.Gray;
                    else
                        _cells[r, c].Fill = Brushes.White;
                    _cells[r, c].Stroke = Brushes.Black;
                }

        }

        public override string ToString()
        {
            string str = "", temp;

            for (int r = 0; r < _height; r++)
            {
                for (int c = 0; c < _width; c++)
                {
                    temp = _cells[r, c].Fill.ToString();
                    if (temp.Equals(Block.TEE.ToString(), StringComparison.OrdinalIgnoreCase))
                        str+="T";
                    else if (temp.Equals(Block.EL.ToString(), StringComparison.OrdinalIgnoreCase))
                        str+="E";
                    else if (temp.Equals(Block.LINE.ToString(), StringComparison.OrdinalIgnoreCase))
                        str+="L";
                    else if (temp.Equals(Block.JAY.ToString(), StringComparison.OrdinalIgnoreCase))
                        str+="J";
                    else if (temp.Equals(Block.ZSKEW.ToString(), StringComparison.OrdinalIgnoreCase))
                        str+="Z";
                    else if (temp.Equals(Block.SSKEW.ToString(), StringComparison.OrdinalIgnoreCase))
                        str+="S";
                    else if (temp.Equals(Block.SQUARE.ToString(), StringComparison.OrdinalIgnoreCase))
                        str+="Q";
                    else
                        str+="#";
                }
            }

            return str;
        }
    }
}
