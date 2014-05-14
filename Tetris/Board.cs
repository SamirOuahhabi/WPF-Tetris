using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
                    _cells[r, c].Name = "rect_" + r + "_" + c;
                    window.RegisterName(_cells[r, c].Name, _cells[r, c]);
                    _cells[r, c].Stretch = Stretch.Fill;
                    _cells[r, c].Fill = _background;
                    _cells[r, c].Stroke = _borders;

                    Grid.SetColumn(_cells[r, c], c);
                    Grid.SetRow(_cells[r, c], r);

                    grid.Children.Add(_cells[r, c]);
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
                if (!inBound((int)(b.Shape[i].Y + b.Coordinates.Y),
                        (int)(b.Shape[i].X + b.Coordinates.X) + 1) ||
                        isOccupied((int)(b.Shape[i].Y + b.Coordinates.Y),
                        (int)(b.Shape[i].X + b.Coordinates.X) + 1))
                {
                    Draw(b);
                    return false;
                }
            }

            b.Coordinates = new Point(b.Coordinates.X + 1, b.Coordinates.Y);
            Draw(b);

            return true;
        }

        public bool moveLeft(Block b)
        {

            Erase(b);
            for (int i = 0; i < b.Shape.Length; i++)
            {
                if (!inBound((int)(b.Shape[i].Y + b.Coordinates.Y),
                        (int)(b.Shape[i].X + b.Coordinates.X) - 1) ||
                        isOccupied((int)(b.Shape[i].Y + b.Coordinates.Y),
                        (int)(b.Shape[i].X + b.Coordinates.X) - 1))
                {
                    Draw(b);
                    return false;
                }
            }

            b.Coordinates = new Point(b.Coordinates.X - 1, b.Coordinates.Y);
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
                if( !inBound((int)(_block.Shape[i].Y + _block.Coordinates.Y)+1,
                    (int)(_block.Shape[i].X + _block.Coordinates.X)) || 
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

        public void rotateBlock(Block _block)
        {
            throw new NotImplementedException();
        }
    }
}
