using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace firstGame
{
    class Player
    {
        private int x = 0;
        private int y = 0;

        public Player(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int getX
        {
            get { return x; }
            set { x = value; }
        }

        public int getY
        {
            get { return y; }
            set { y = value; }
        }

        public int getSpeed
        {
            get { return speed; }
            set { speed = value; }
        }
    }
}
