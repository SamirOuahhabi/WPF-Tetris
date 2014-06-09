using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public class SoundLibrary
    {
        protected SoundPlayer _success;
        protected SoundPlayer _gameOver;
        protected SoundPlayer _yeah;
        protected SoundPlayer _tic;

        public SoundLibrary()
        {
            _success = new SoundPlayer();
            _success.Stream = Properties.Resources.success;

            _gameOver = new SoundPlayer();
            _gameOver.Stream = Properties.Resources.game_over;

            _yeah = new SoundPlayer();
            _yeah.Stream = Properties.Resources.yeah;

            _tic = new SoundPlayer();
            _tic.Stream = Properties.Resources.tic;
        }

        public void Success()
        {
            _success.Play();
        }

        public void gameOver()
        {
            _gameOver.Play();
        }

        public void Yeah()
        {
            _yeah.Play();
        }

        public void tic()
        {
            _tic.Play();
        }
    }
}
