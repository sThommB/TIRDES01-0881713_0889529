using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Linq;

namespace firstGame
{
    struct Entity //for asteroids and plasma objects
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
        Vector2 PlayerMovement { get; }
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

        public Vector2 PlayerMovement
        {
            get
            {
                var PlayerMovement = Vector2.Zero;
                if (ks.IsKeyDown(Keys.A))//left
                    PlayerMovement.X -= 1.0f;
                if (ks.IsKeyDown(Keys.D))//right
                    PlayerMovement.X += 1.0f;
                if (ks.IsKeyDown(Keys.W))//up
                    PlayerMovement.Y -= 1.0f;
                if (ks.IsKeyDown(Keys.S))//down
                    PlayerMovement.Y += 1.0f;
                return PlayerMovement;
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

        public Vector2 PlayerMovement
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

        public Vector2 PlayerMovement
        {
            get
            {
                return first.PlayerMovement + second.PlayerMovement;
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
        Texture2D attackTexture, rightMoveTexture, leftMoveTexture, downMoveTexture, upMoveTexture, idleTexture;//player
        Texture2D enemyTexture, friezaHurtTexture, background;//enemy
        Texture2D asteroidTexture;//asteroid
        Texture2D fireTexture;//plasma

        Rectangle mainFrame;//background

        Random randomGenerator = new Random();
        List<Entity> asteroids = new List<Entity>();
        List<Entity> plasmas = new List<Entity>();
        Entity player;
        float playerSpeed;

        int gameLogicScriptPC = 0;
        int rndNumberLine1, iLine1;
        float timeToWaitLine3, timeToWaitLine4, timeToWaitLine8, timeToWaitLine7;
        int rndNumberLine5, iLine5;

        InputController input =
         new ControllerSum(
           new KeyboardController(),
             new MouseController());

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);            // Create a new SpriteBatch, which can be used to draw textures.

            //player
            attackTexture = Content.Load<Texture2D>("pictures/blast");
            rightMoveTexture = Content.Load<Texture2D>("pictures/GokuSSJ/gokuRight");
            leftMoveTexture = Content.Load<Texture2D>("pictures/GokuSSJ/gokuLeft");
            downMoveTexture = Content.Load<Texture2D>("pictures/GokuSSJ/gokuUpDown");
            upMoveTexture = Content.Load<Texture2D>("pictures/GokuSSJ/gokuUpDown");
            idleTexture = Content.Load<Texture2D>("pictures/GokuSSJ/gokuIdle");
            //enemy
            enemyTexture = Content.Load<Texture2D>("pictures/Frieza/FriezaIdle");
            friezaHurtTexture = Content.Load<Texture2D>("pictures/Frieza/FriezaHurt1");
            background = Content.Load<Texture2D>("pictures/Background/Arena");
            //plasma
            fireTexture = Content.Load<Texture2D>("pictures/GokuSSJ/gokuFire");
            //asteroid
            asteroidTexture = Content.Load<Texture2D>("pictures/Asteroid");

            player = new Entity(new Vector2(300.0f, 400.0f),
              Content.Load<Texture2D>("pictures/GokuSSJ/gokuIdle"));
            playerSpeed = 100.0f;
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

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
               where plasma.X > 0.0f &&
                     plasma.X < 800.0f &&
                     plasma.Y > 0.0f &&
                     plasma.Y < 800.0f &&
                     colliders.Count() == 0
               select plasma.CreateMoved(-Vector2.UnitX * -200.0f * deltaTime)).ToList();
                if (input.Shooting)
                    newPlasmas.Add(
                      new Entity(player.Position,
                        attackTexture));

            var newAsteroids =
                (from asteroid in asteroids
                let colliders =
                    from plasma in plasmas
                    where Vector2.Distance(plasma.Position, asteroid.Position) < 20.0f
                    select plasma
                where asteroid.X > 0.0f &&
                        asteroid.X < 800.0f &&
                        asteroid.Y > 0.0f &&
                        asteroid.Y < 600.0f &&
                        colliders.Count() == 0
                select asteroid.CreateMoved(Vector2.UnitX * -100.0f * deltaTime)).ToList();

            Vector2 playerVelocity = input.PlayerMovement * playerSpeed;
            var newplayer = player.CreateMoved(playerVelocity * deltaTime); 

            switch (gameLogicScriptPC)
            {
                case 0://always true
                    if (true)
                    {
                        gameLogicScriptPC = 1;
                        iLine1 = 1;
                        rndNumberLine1 = randomGenerator.Next(20, 60);
                    }
                    else
                        gameLogicScriptPC = 9;
                    break;
                case 1://'for loop'
                    if (iLine1 <= rndNumberLine1)
                        gameLogicScriptPC = 2;
                    else
                    {
                        gameLogicScriptPC = 4;//nxt loop
                        timeToWaitLine4 = (float)(randomGenerator.NextDouble() * 2.0 + 5.0);
                    }
                    break;
                case 2://create asteroid
                    newAsteroids.Add(
                     new Entity(new Vector2(600.0f, (float)(randomGenerator.NextDouble() * 800.0)),
                       asteroidTexture));
                    gameLogicScriptPC = 3;
                    timeToWaitLine3 = (float)(randomGenerator.NextDouble() * 0.2 + 0.1);

                    gameLogicScriptPC = 3;
                    timeToWaitLine3 = (float)(randomGenerator.NextDouble() * 0.2 + 0.1);
                    break;
                case 3://wait per asteroid
                    timeToWaitLine3 -= deltaTime;
                    if (timeToWaitLine3 > 0.0f)
                        gameLogicScriptPC = 3;
                    else
                    {
                        gameLogicScriptPC = 1;
                        iLine1++;
                    }
                    break;
                case 4://wait nxt loop
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
                case 5://'for loop'
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
                case 6://create asteroid
                    newAsteroids.Add(
                      new Entity(new Vector2(600.0f, (float)(randomGenerator.NextDouble() * 800.0)),
                        asteroidTexture));

                    gameLogicScriptPC = 7;
                    timeToWaitLine7 = (float)(randomGenerator.NextDouble() * 1.5 + 0.5);
                    break;
                case 7://wait for asteroid
                    timeToWaitLine7 -= deltaTime;
                    if (timeToWaitLine7 > 0)
                        gameLogicScriptPC = 7;
                    else
                    {
                        gameLogicScriptPC = 5;
                        iLine5++;
                    }
                    break;
                case 8://wait and go to 0
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

            plasmas = newPlasmas;
            asteroids = newAsteroids;
            player = newplayer;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //prevents border leaving  'border control'
           /* if (player.getX < 0)
            {
                player.getX = 0;//left border control
            }

            if (player.getX + playerTexture.Width > 800)
            {
                player.getX = 800 - playerTexture.Width;//right border control
            }

            if (player.getY < 0)
            {
                player.getY = 0;//up border control
            }

            if (player.getY + playerTexture.Height > 600)
            {
                player.getY = 600 - playerTexture.Height;//down border control
            }*/

            spriteBatch.Begin();

            //Background
            spriteBatch.Draw(background, mainFrame, Color.White);
            mainFrame = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            spriteBatch.Draw(player.Appearance, player.Position, Color.White);
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


// System.Console.WriteLine("thing in here");
//System.Diagnostics.Debug.WriteLine("dfsdf");
// view->output