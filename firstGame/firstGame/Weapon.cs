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
        public void PullTrigger()
        {
            //place for conditional statements
            AddShots();
        }

        protected abstract void AddShots();
        protected Vector2 playerPosition;
        public void Update(float dt, Vector2 playerPosition)
        {
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
