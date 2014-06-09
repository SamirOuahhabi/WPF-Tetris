using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public class SavedInstance
    {
        protected int _id;
        protected string _name;
        protected string _board;
        protected int _score;
        protected int _linesCleared;

        public SavedInstance() : this("", "", 0, 0)
        {
        }

        public SavedInstance(string name, string board, int score, int linesCleared)
        {
            _name = name;
            _board = board;
            _score = score;
            _linesCleared = linesCleared;
        }

        public override string ToString()
        {
            return _name + ": lvl " + (_linesCleared / 10 + 1) + ", score: " + _score;
        }

        public int Id
        {
            set
            {
                _id = value;
            }
            get
            {
                return _id;
            }
        }

        public string Name
        {
            set
            {
                _name = value;
            }
            get
            {
                return _name;
            }
        }

        public string Board
        {
            set
            {
                _board = value;
            }
            get
            {
                return _board;
            }
        }

        public int Score
        {
            set
            {
                _score = value;
            }
            get
            {
                return _score;
            }
        }

        public int LinesCleared
        {
            set
            {
                _linesCleared = value;
            }
            get
            {
                return _linesCleared;
            }
        }
    }
}
