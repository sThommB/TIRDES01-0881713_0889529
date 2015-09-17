using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace firstGame
{
    class Asteroid
    {
        public Vector2 asteroidPosition;
        public bool IsActive = false;

        public Asteroid()
        {
        }

        public void activateAsteroid(Vector2 pos)
        {
            asteroidPosition = pos;
            IsActive = true;
        }

        //Asteroid movement
        public void updateAsteroid(List<Asteroid> x, float xSpeed, float ySpeed)
        {
            foreach (Asteroid asteroid in x)
            {
                asteroid.asteroidPosition.X -= xSpeed;
                asteroid.asteroidPosition.Y -= ySpeed;
            }
        }

        //'delete' off-screen Asteroid
        public void deactivateAsteroid(List<Asteroid> x, int stageHeight)
        {
            foreach (Asteroid asteroid in x)
            {
                if (asteroid.asteroidPosition.X < 0 || asteroid.asteroidPosition.Y > stageHeight) { }
                   // asteroid.IsActive = false;
            }
        }
    }
}
