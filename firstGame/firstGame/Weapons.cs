using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Linq;

interface Weapon<Ammunition>
{
    void PullTrigger();
    List<Ammunition> NewBullets { get; }
    void Update(float dt, Vector2 playerPosition);
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
            charge -= 2;
            timeSinceLastShot = 0.0f;
            AddShots();
        }
    }

    protected abstract void AddShots();
    protected Vector2 playerPosition;
    public void Update(float dt, Vector2 playerPosition)
    {
        charge += dt * 20.0f;
        timeSinceLastShot += dt;
        charge = MathHelper.Clamp(charge, 0, 100);
        this.shipPosition = playerPosition;
        barrel = new List<Entity>();
    }
}

class Blaster : TimedGenericBlaster
{
    public Blaster(ContentManager content) : base(content) { }

    protected override void AddShots()
    {
        barrel.Add(new Entity(shipPosition,
          Content.Load<Texture2D>("blast")));
    }
}