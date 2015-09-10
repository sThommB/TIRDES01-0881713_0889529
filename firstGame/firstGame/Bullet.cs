using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace firstGame
{
    class Bullet
    {
        public Texture2D bulletTexture;
        public Vector2 bulletPosition;
        public bool IsActive = false;
        //int speed = 2;

        public Bullet()
        {
        }

        public void activateBullet(Vector2 center, Texture2D texture, float x, float y)
        {
            x = center.X + x;//player positon + pre-defined x
            y = center.Y + y;//player positon + pre-defined y
            bulletPosition = new Vector2(x, y);
            bulletTexture = texture;
            IsActive = true;
        }

        //bullet movement
        public void updateBullets(List<Bullet> x, float xSpeed, float ySpeed)
        {
            foreach (Bullet bullet in x)
            {
                bullet.bulletPosition.X += xSpeed;
                bullet.bulletPosition.Y += ySpeed;
            }
        }

        //'delete' off-screen bullets
        public void deactivateBullet(List<Bullet> x, int stageHeight)
        {
            foreach (Bullet bullet in x)
            {
                if (bullet.bulletPosition.X < 0 || bullet.bulletPosition.Y > stageHeight)
                {
                    bullet.IsActive = false;
                }
            }
        }
    }
}
