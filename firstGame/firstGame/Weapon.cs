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
        float timeSinceLastShot = float.PositiveInfinity;//posinfinte is a number bigger than 3.402823e38
        public void PullTrigger()
        {
            //conditional statement
            if (charge >= 2 && timeSinceLastShot >= 0.2f)
            {
                charge -= 10;//shooting takes energy
                timeSinceLastShot = 0.0f;//we're gonna shoot now
                AddShots();//shooting
                //System.Console.WriteLine(charge);//for debug, view->output
            }
        }
        protected abstract void AddShots();
        protected Vector2 playerPosition;
        public void Update(float dt, Vector2 playerPosition)
        {
            charge += dt * 10.0f;//one second = +10.0f charge
            timeSinceLastShot += dt;//add time since last update
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
