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
        public static SolidColorBrush TEE = Brushes.Crimson;
        public static SolidColorBrush EL = Brushes.Teal;
        public static SolidColorBrush LINE = Brushes.SlateBlue;
        public static SolidColorBrush JAY = Brushes.YellowGreen;
        public static SolidColorBrush ZSKEW = Brushes.MediumOrchid;
        public static SolidColorBrush SSKEW = Brushes.SaddleBrown;
        public static SolidColorBrush SQUARE = Brushes.Chocolate;

        protected Point[][] _shape;
        protected Point _coordinates;
        protected SolidColorBrush _color;
        protected int _rotation;
        protected string _type;

        public Block(Random rand)
        {
            generateBlock(rand.Next() % 7);
            _rotation = rand.Next(0, 4);
        }

        public Block(int shape, int rotation)
        {
            generateBlock(shape);
            _rotation = rotation;
        }

        protected void generateBlock(int shape)
        {
            switch (shape)
            {
                case 0: //T
                    generateT();
                    break;

                case 1: //L
                    generateL();
                    break;

                case 2: // _
                    generateLine();
                    break;

                case 3: // J
                    generateJ();
                    break;


                case 4: // Z
                    generateZ();
                    break;


                case 5: // S
                    generateS();
                    break;

                case 6: //[]
                    generateSquare();
                    break;

                default:
                    _shape = null;
                    break;
            }
        }

        private void generateSquare()
        {
            _color = Block.SQUARE;
            _type = "Square";
            _shape = new Point[][]{
                    new Point[]{
                        new Point(0,0),
                        new Point(0,1),
                        new Point(1,0),
                        new Point(1,1)},
                    new Point[]{
                        new Point(0,0),
                        new Point(0,1),
                        new Point(1,0),
                        new Point(1,1)},
                    new Point[]{
                        new Point(0,0),
                        new Point(0,1),
                        new Point(1,0),
                        new Point(1,1)},
                    new Point[]{ 
                        new Point(0,0),
                        new Point(0,1),
                        new Point(1,0),
                        new Point(1,1)}
                    };
        }

        private void generateS()
        {
            _color = Block.SSKEW;
            _type = "S Skew";
            _shape = new Point[][]{
                    new Point[]{
                        new Point(-1,1),
                        new Point(0,1),
                        new Point(0,0),
                        new Point(1,0)},
                    new Point[]{   
                        new Point(0,-1),
                        new Point(0,0),
                        new Point(1,0),
                        new Point(1,1)},
                    new Point[]{   
                        new Point(-1,1),
                        new Point(0,1),
                        new Point(0,0),
                        new Point(1,0)},
                    new Point[]{   
                        new Point(0,-1),
                        new Point(0,0),
                        new Point(1,0),
                        new Point(1,1)}
                    };
        }

        private void generateZ()
        {
            _color = Block.ZSKEW;
            _type = "Z Skew";
            _shape = new Point[][]{
                    new Point[]{
                        new Point(-1,0),
                        new Point(0,0),
                        new Point(0,1),
                        new Point(1,1)},
                    new Point[]{   
                        new Point(0,1),
                        new Point(0,0),
                        new Point(1,0),
                        new Point(1,-1)},
                    new Point[]{   
                        new Point(-1,0),
                        new Point(0,0),
                        new Point(0,1),
                        new Point(1,1)},
                    new Point[]{   
                        new Point(0,1),
                        new Point(0,0),
                        new Point(1,0),
                        new Point(1,-1)}
                    };
        }

        private void generateJ()
        {
            _color = Block.JAY;
            _type = "Jay";
            _shape = new Point[][]{
                    new Point[]{
                        new Point(-1,0),
                        new Point(0,0),
                        new Point(1,0),
                        new Point(1,1)},
                    new Point[]{   
                        new Point(0,-1),
                        new Point(0,0),
                        new Point(0,1),
                        new Point(-1,1)},
                    new Point[]{   
                        new Point(-1,0),
                        new Point(0,0),
                        new Point(1,0),
                        new Point(-1,-1)},
                    new Point[]{   
                        new Point(0,-1),
                        new Point(0,0),
                        new Point(0,1),
                        new Point(1,-1)}
                    };
        }

        private void generateLine()
        {
            _color = Block.LINE;
            _type = "Straight line";
            _shape = new Point[][]{
                    new Point[]{
                        new Point(0,0),
                        new Point(-1,0),
                        new Point(1,0),
                        new Point(2,0)},
                    new Point[]{
                        new Point(1,-2),
                        new Point(1,-1),
                        new Point(1,0),
                        new Point(1,1)},
                    new Point[]{
                        new Point(0,0),
                        new Point(-1,0),
                        new Point(1,0),
                        new Point(2,0)},
                    new Point[]{
                        new Point(1,-2),
                        new Point(1,-1),
                        new Point(1,0),
                        new Point(1,1)}
                    };
        }

        private void generateL()
        {
            _color = Block.EL;
            _type = "El";
            _shape = new Point[][]{
                    new Point[]{
                        new Point(-1,0),
                        new Point(0,0),
                        new Point(1,0),
                        new Point(-1,1)},
                    new Point[]{   
                        new Point(0,-1),
                        new Point(0,0),
                        new Point(0,1),
                        new Point(-1,-1)},
                    new Point[]{   
                        new Point(-1,0),
                        new Point(0,0),
                        new Point(1,0),
                        new Point(1,-1)},
                    new Point[]{   
                        new Point(0,-1),
                        new Point(0,0),
                        new Point(0,1),
                        new Point(1,1)}
                    };
        }

        private void generateT()
        {
            _color = Block.TEE;
            _type = "Tee";
            _shape = new Point[][]{
                    new Point[]{   
                        new Point(0,0),
                        new Point(-1,0),
                        new Point(0,1),
                        new Point(1,0)},
                    new Point[]{   
                        new Point(0,-1),
                        new Point(0,0),
                        new Point(0,1),
                        new Point(-1,0)},
                    new Point[]{   
                        new Point(0,0),
                        new Point(-1,0),
                        new Point(0,-1),
                        new Point(1,0)},
                    new Point[]{   
                        new Point(0,-1),
                        new Point(0,0),
                        new Point(0,1),
                        new Point(1,0)}
                    };
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

        public void Rotate(string direction)
        {
            if (direction.Equals("up", StringComparison.OrdinalIgnoreCase))
                _rotation++;
            else
                _rotation += 3;
        }

        public Point[] Shape
        {
            get
            {
                return _shape[_rotation % 4];
            }
        }

        public Point[] NextRotation
        {
            get
            {
                return _shape[(_rotation + 1) % 4];
            }
        }

        public Point[] PreviousRotation
        {
            get
            {
                return _shape[(_rotation +3) % 4];
            }
        }

        public SolidColorBrush Color
        {
            get
            {
                return _color;
            }
        }

        public string Type
        {
            get
            {
                return _type;
            }
        }

        public override string ToString()
        {
            return _type;
        }
    }
}
