using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Tetris
{
    public class Block
    {
        public static SolidColorBrush LINE = Brushes.Cyan;
        public static SolidColorBrush SQUARE = Brushes.Yellow;
        public static SolidColorBrush TEE = Brushes.HotPink;
        public static SolidColorBrush JAY = Brushes.Blue;
        public static SolidColorBrush EL = Brushes.Orange;
        public static SolidColorBrush SSKEW = Brushes.LightGreen;
        public static SolidColorBrush ZSKEW = Brushes.Red;

        protected Point[] _shape;
        protected Point _coordinates;
        protected SolidColorBrush _color;

        public Block()
        {
            generateBlock();
        }

        protected void generateBlock()
        {
            Random rand = new Random();

            switch (rand.Next() % 7)
            {
                case 0: //T
                    _color = Block.TEE;
                    _shape = new Point[]{
                        new Point(0,0),
                        new Point(-1,0),
                        new Point(0,-1),
                        new Point(1,0),
                    };
                    break;

                case 1: //L
                    _color = Block.EL;
                    _shape = new Point[]{
                        new Point(0,0),
                        new Point(0,-1),
                        new Point(0,1),
                        new Point(1,1),
                    };
                    break;

                case 2: // _
                    _color = Block.LINE;
                    _shape = new Point[]{
                        new Point(0,0),
                        new Point(-1,0),
                        new Point(1,0),
                        new Point(2,0),
                    };
                    break;

                case 3: // J
                    _color = Block.JAY;
                    _shape = new Point[]{
                        new Point(0,0),
                        new Point(0,-1),
                        new Point(0,1),
                        new Point(-1,1),
                    };
                    break;


                case 4: // Z
                    _color = Block.ZSKEW;
                    _shape = new Point[]{
                        new Point(0,0),
                        new Point(-1,0),
                        new Point(0,1),
                        new Point(1,1),
                    };
                    break;


                case 5: // S
                    _color = Block.SSKEW;
                    _shape = new Point[]{
                        new Point(0,0),
                        new Point(1,0),
                        new Point(0,1),
                        new Point(-1,1),
                    };
                    break;

                case 6: //[]
                    _color = Block.SQUARE;
                    _shape = new Point[]{
                        new Point(0,0),
                        new Point(0,1),
                        new Point(1,0),
                        new Point(1,1),
                    };
                    break;

                default:
                    _shape = null;
                    break;
            }
        }

        public Point Coordinates
        {
            get
            {
                return _coordinates;
            }
            set
            {
                _coordinates = value;
            }
        }

        public Point[] Shape
        {
            get
            {
                return _shape;
            }
        }

        public SolidColorBrush Color
        {
            get
            {
                return _color;
            }
        }
    }
}
