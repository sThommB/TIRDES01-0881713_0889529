using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Content;

namespace firstGame
{
    interface Weapon<Ammunition>
    {
        void PullTrigger();
        List<Ammunition> NewBullets { get; }
        void Update(float dt, Vector2 shipPosition);
    }

    abstract class TimedGenericBlaster : Weapon<Entity>
    {
        public List<Entity> NewBullets
        {
            get
            {
                return barrel;
            }
        }

        protected ContentManager Content;
        public TimedGenericBlaster(ContentManager content)
        {
            Content = content;
        }

        protected List<Entity> barrel = new List<Entity>();
        float charge = 100;
        float timeSinceLastShot = float.PositiveInfinity;
        public void PullTrigger()
        {
            if (charge >= 2 && timeSinceLastShot >= 0.2f)
            {
                //place for conditional statements
                charge -= 10;
                timeSinceLastShot = 0.0f;
                AddShots();
            }
        }
        protected abstract void AddShots();
        protected Vector2 playerPosition;
        public void Update(float dt, Vector2 playerPosition)
        {
            charge += dt * 10.0f; //recharge time
            timeSinceLastShot += dt;
            charge = MathHelper.Clamp(charge, 0, 100); //giving it a range form 0-100
            this.playerPosition = playerPosition;//for placing bullet near player
            barrel = new List<Entity>();//overwrites the old list, for removing old barrel plasma?
        }
    }

    class Blaster : TimedGenericBlaster
    {
        public Blaster(ContentManager content) : base(content) { }

        protected override void AddShots()
        {
            barrel.Add(new Entity(playerPosition,
              Content.Load<Texture2D>("pictures/blast")));
        }
    }

    class BigBlaster : TimedGenericBlaster
    {
        public BigBlaster(ContentManager content) : base(content) { }

        protected override void AddShots()
        {
            barrel.Add(new Entity(playerPosition,
              Content.Load<Texture2D>("pictures/blast3")));
        }
    }
}
