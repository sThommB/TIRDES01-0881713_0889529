﻿using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace DesPatSampleUnstructured
{
    struct Entity
    {
        public Entity(Vector2 p, Texture2D a)
        {
            Position = p;
            Appearance = a;
        }

        public Vector2 Position { get; private set; }
        public Texture2D Appearance { get; private set; }

        public float X { get { return Position.X; } }
        public float Y { get { return Position.Y; } }

        public Entity CreateMoved(Vector2 deltaPosition)
        {
            return new Entity()
            {
                Position = this.Position + deltaPosition,
                Appearance = this.Appearance
            };
        }
    }

    interface InputController
    {
        bool Quit { get; }
        Vector2 ShipMovement { get; }
        bool Shooting { get; }

        void Update(float dt);
    }

    class KeyboardController : InputController
    {
        KeyboardState ks;

        public bool Quit
        {
            get
            {
                return ks.IsKeyDown(Keys.Escape);
            }
        }

        public Vector2 ShipMovement
        {
            get
            {
                var shipMovement = Vector2.Zero;
                if (ks.IsKeyDown(Keys.A))
                    shipMovement = shipMovement + new Vector2(-1.0f, 0.0f);
                if (ks.IsKeyDown(Keys.D))
                    shipMovement = shipMovement + new Vector2(1.0f, 0.0f);
                if (ks.IsKeyDown(Keys.W))
                    shipMovement = shipMovement + new Vector2(0.0f, -1.0f);
                if (ks.IsKeyDown(Keys.S))
                    shipMovement = shipMovement + new Vector2(0.0f, 1.0f);
                return shipMovement;
            }
        }

        public bool Shooting
        {
            get
            {
                return ks.IsKeyDown(Keys.Space);
            }
        }

        public void Update(float dt)
        {
            ks = Keyboard.GetState();
        }
    }

    class MouseController : InputController
    {
        MouseState ms;

        public bool Quit
        {
            get
            {
                return false;
            }
        }

        public Vector2 ShipMovement
        {
            get
            {
                return new Vector2(ms.X - 400, ms.Y - 300) * 0.01f;
            }
        }

        public bool Shooting
        {
            get
            {
                return ms.LeftButton == ButtonState.Pressed;
            }
        }

        public void Update(float dt)
        {
            ms = Mouse.GetState();
        }
    }

    class ControllerSum : InputController
    {
        InputController first, second;
        public ControllerSum(InputController a, InputController b)
        {
            first = a;
            second = b;
        }

        public bool Quit
        {
            get
            {
                return first.Quit || second.Quit;
            }
        }

        public Vector2 ShipMovement
        {
            get
            {
                return first.ShipMovement + second.ShipMovement;
            }
        }

        public bool Shooting
        {
            get
            {
                return first.Shooting || second.Shooting;
            }
        }

        public void Update(float dt)
        {
            first.Update(dt);
            second.Update(dt);
        }
    }

    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        int gameLogicScriptPC = 0;
        int rndNumberLine1, iLine1;
        float timeToWaitLine3, timeToWaitLine4, timeToWaitLine8, timeToWaitLine7;
        int rndNumberLine5, iLine5;

        InputController input =
          new ControllerSum(
            new KeyboardController(),
              new MouseController());

        Random randomGenerator = new Random();
        List<Entity> asteroids = new List<Entity>();
        List<Entity> plasmas = new List<Entity>();
        Entity ship;
        float shipSpeed;
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            ship = new Entity(new Vector2(300.0f, 400.0f),
              Content.Load<Texture2D>("ship.png"));
            shipSpeed = 100.0f;
        }

        protected override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            input.Update(deltaTime);
            if (input.Quit)
                Exit();

            var newPlasmas =
              (from plasma in plasmas
               let colliders =
                  from asteroid in asteroids
                  where Vector2.Distance(plasma.Position, asteroid.Position) < 20.0f
                  select asteroid
               where plasma.X > 50.0f &&
                     plasma.X < 600.0f &&
                     plasma.Y > 50.0f &&
                     plasma.Y < 600.0f &&
                     colliders.Count() == 0
               select plasma.CreateMoved(-Vector2.UnitY * 200.0f * deltaTime)).ToList();
            if (input.Shooting)
                newPlasmas.Add(
                  new Entity(ship.Position,
                    Content.Load<Texture2D>("plasmaSmall")));

            var newAsteroids =
              (from asteroid in asteroids
               let colliders =
                  from plasma in plasmas
                  where Vector2.Distance(plasma.Position, asteroid.Position) < 20.0f
                  select plasma
               where asteroid.X > 50.0f &&
                     asteroid.X < 600.0f &&
                     asteroid.Y > 50.0f &&
                     asteroid.Y < 300.0f &&
                     colliders.Count() == 0
               select asteroid.CreateMoved(Vector2.UnitY * 100.0f * deltaTime)).ToList();

            Vector2 shipVelocity = input.ShipMovement * shipSpeed;
            var newShip = ship.CreateMoved(shipVelocity * deltaTime);

            switch (gameLogicScriptPC)
            {
                case 0:
                    if (true)
                    {
                        gameLogicScriptPC = 1;
                        iLine1 = 1;
                        rndNumberLine1 = randomGenerator.Next(20, 60);
                    }
                    else
                        gameLogicScriptPC = 9;
                    break;
                case 1:
                    if (iLine1 <= rndNumberLine1)
                        gameLogicScriptPC = 2;
                    else
                    {
                        gameLogicScriptPC = 4;
                        timeToWaitLine4 = (float)(randomGenerator.NextDouble() * 2.0 + 5.0);
                    }
                    break;
                case 2:
                    newAsteroids.Add(
                      new Entity(new Vector2((float)(randomGenerator.NextDouble() * 500.0 + 51.0), 51.0f),
                        Content.Load<Texture2D>("asteroid")));
                    gameLogicScriptPC = 3;
                    timeToWaitLine3 = (float)(randomGenerator.NextDouble() * 0.2 + 0.1);
                    break;
                case 3:
                    timeToWaitLine3 -= deltaTime;
                    if (timeToWaitLine3 > 0.0f)
                        gameLogicScriptPC = 3;
                    else
                    {
                        gameLogicScriptPC = 1;
                        iLine1++;
                    }
                    break;
                case 4:
                    timeToWaitLine4 -= deltaTime;
                    if (timeToWaitLine4 > 0.0f)
                        gameLogicScriptPC = 4;
                    else
                    {
                        gameLogicScriptPC = 5;
                        iLine5 = 1;
                        rndNumberLine5 = randomGenerator.Next(10, 20);
                    }
                    break;
                case 5:
                    if (iLine5 <= rndNumberLine5)
                    {
                        gameLogicScriptPC = 6;
                    }
                    else
                    {
                        gameLogicScriptPC = 8;
                        timeToWaitLine8 = (float)(randomGenerator.NextDouble() * 2.0 + 5.0);
                    }
                    break;
                case 6:
                    newAsteroids.Add(
                      new Entity(new Vector2((float)(randomGenerator.NextDouble() * 500.0 + 51.0), 51.0f),
                        Content.Load<Texture2D>("asteroid")));
                    gameLogicScriptPC = 7;
                    timeToWaitLine7 = (float)(randomGenerator.NextDouble() * 1.5 + 0.5);
                    break;
                case 7:
                    timeToWaitLine7 -= deltaTime;
                    if (timeToWaitLine7 > 0)
                        gameLogicScriptPC = 7;
                    else
                    {
                        gameLogicScriptPC = 5;
                        iLine5++;
                    }
                    break;
                case 8:
                    timeToWaitLine8 -= deltaTime;
                    if (timeToWaitLine8 > 0.0f)
                        gameLogicScriptPC = 8;
                    else
                    {
                        gameLogicScriptPC = 0;
                    }
                    break;
                default:
                    break;
            }


            // COMMIT CHANGES TO THE STATE
            plasmas = newPlasmas;
            asteroids = newAsteroids;
            ship = newShip;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            spriteBatch.Draw(ship.Appearance, ship.Position, Color.White);
            foreach (var plasma in plasmas)
            {
                spriteBatch.Draw(plasma.Appearance, plasma.Position, Color.White);
            }
            foreach (var asteroid in asteroids)
            {
                spriteBatch.Draw(asteroid.Appearance, asteroid.Position, Color.White);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

/*
//hittest if player touches enemy
if (((player.getX + playerTexture.Width) >= 710 && player.getX <= (710 + enemyTexture.Width)) && ((player.getY + enemyTexture.Height) >= 500 && player.getY <= (500 + enemyTexture.Height)))
{
    System.Console.WriteLine("Hit");
    enemyTexture = friezaHurtTexture;
                
}
 //player
            spriteBatch.Draw(playerTexture, spritePosition, Color.White);
            spriteBatch.Draw(enemyTexture, spritePositionEnemy, Color.White);
 //bullets
            foreach (Bullet bullet in bullets)//check every bullet
            {
                if (bullet.IsActive == true)//only draw active bullets
                {
                    spriteBatch.Draw(bullet.bulletTexture, bullet.bulletPosition, Color.White);
                }
                //hittest if bullet hits asteroid
                /*foreach (Asteroid asteroid in asteroids)//check every asteroid
                {
                    if (((bullet.bulletPosition.X + bullet.bulletTexture.Width) >= 710 && bullet.bulletPosition.X <= (710 + asteroidTexture.Width)) && ((bullet.bulletPosition.Y + asteroidTexture.Height) >= 500 && bullet.bulletPosition.Y <= (500 + asteroidTexture.Height)))
                    {
                        System.Console.WriteLine("Hit");
                        enemyTexture = friezaHurtTexture;
                    }
                    if (asteroid.IsActive == true)//only draw active asteroid
                    {
                        spriteBatch.Draw(asteroidTexture, asteroid.asteroidPosition, Color.White);
                    }
                }           
            }

            foreach (Asteroid asteroid in asteroids)//check every asteroid
            {
                if (asteroid.IsActive == true)//only draw active asteroid
                {
                    spriteBatch.Draw(asteroidTexture, asteroid.asteroidPosition, Color.White);
                }
            }
            //plasma
            foreach (var plasmaPosition in plasmaPositions)
            {
                spriteBatch.Draw(attackTexture, plasmaPosition, Color.White);
            }
            //asteroid
            foreach (var asteroidPosition in asteroidPositions)
            {
                spriteBatch.Draw(asteroidTexture, asteroidPosition, Color.White);
            }

    //movement - right, left, down, up.
            if (newState.IsKeyDown(Keys.Right) || newState.IsKeyDown(Keys.D))
            {
                player.getX += player.getSpeed;
                playerTexture = rightMoveTexture;//right
            }
            else if (newState.IsKeyDown(Keys.Left) || newState.IsKeyDown(Keys.A))
            {
                player.getX += (player.getSpeed * -1);
                playerTexture = leftMoveTexture;//left
            }
            else if (newState.IsKeyDown(Keys.Down) || newState.IsKeyDown(Keys.S))
            {
                player.getY += player.getSpeed;
                playerTexture = downMoveTexture;//down
            }
            else if (newState.IsKeyDown(Keys.Up) || newState.IsKeyDown(Keys.W))
            {
                player.getY += (player.getSpeed * -1);
                playerTexture = upMoveTexture;//up
            }
            else {
                playerTexture = idleTexture;//idle
            }

            //attack
            if (newState.IsKeyDown(Keys.Z) )
            {
                playerTexture = fireTexture; // Fire
                bullet = new Bullet();
                bullet.activateBullet(spritePosition, attackTexture, 25, 15);
                bullets.Add(bullet);
            }
            */