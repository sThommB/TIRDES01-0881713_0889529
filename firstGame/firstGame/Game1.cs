using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Linq;
using firstGame.Scripts;

namespace firstGame
{
    enum InstructionResult
    {
        Done,
        DoneAndCreateAsteroid,
        Running,
        RunningAndCreateAsteroid
    }

    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D attackTexture, rightMoveTexture, leftMoveTexture, downMoveTexture, upMoveTexture, idleTexture;//player
        Texture2D enemyTexture, friezaHurtTexture, background;//enemy
        Texture2D asteroidTexture;//asteroid
        Texture2D fireTexture, fire3Texture;//plasma

        Rectangle mainFrame;//background

        static Random randomGenerator = new Random();
        List<Entity> asteroids = new List<Entity>();
        List<Entity> plasmas = new List<Entity>();
        Entity player;
        float playerSpeed;

        Instruction gameLogic =
            new Repeat(
                new For(0, 10, i =>
                      new Wait(() => i * 0.1f) +
                      new CreateAsteroid()) +
                new Wait(() => randomGenerator.Next(1, 5)) +
                new For(0, 10, i =>
                      new Wait(() => (float)randomGenerator.NextDouble() * 1.0f + 0.2f) +
                      new CreateAsteroid()) +
                new Wait(() => randomGenerator.Next(2, 3)));

        //Controllers
        InputController input = new KeyboardController();   //keyboard only
        //InputController input = new ControllerSum(new KeyboardController(), new MouseController());   //both mouse and keyboard

        //player weapon
        Weapon<Entity> currentWeapon;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //screen size
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);            // Create a new SpriteBatch, which can be used to draw textures.

            //player
            attackTexture = Content.Load<Texture2D>("pictures/GokuSSJ/gokuFire");
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
            fireTexture = Content.Load<Texture2D>("pictures/blast");
            fire3Texture = Content.Load<Texture2D>("pictures/blast3");
            //asteroid
            asteroidTexture = Content.Load<Texture2D>("pictures/Asteroid");

            player = new Entity(new Vector2(300.0f, 400.0f),
              Content.Load<Texture2D>("pictures/GokuSSJ/gokuIdle"));
            playerSpeed = 200.0f;
            //currentWeapon = new Blaster(Content);
            currentWeapon = new BigBlaster(Content);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;//time since last update
            input.Update(deltaTime);
            if (input.Quit)
                Exit();
            
            var newPlasmas =
              (from plasma in plasmas
               let colliders =
                  from asteroid in asteroids
                  where Vector2.Distance(plasma.Position, asteroid.Position) < 20.0f //hittest, does asteroid hit plasma?
                  select asteroid
               where plasma.X > 0.0f &&         //only on-screen plasma
                     plasma.X < 800.0f &&
                     plasma.Y > 0.0f &&
                     plasma.Y < 800.0f &&
                     colliders.Count() == 0
               select plasma.CreateMoved(Vector2.UnitX * 200.0f * deltaTime)).ToList();//move plasma to right
            currentWeapon.Update(deltaTime, player.Position);
            if (input.Shooting)     //if 'shooting' key down, create more plasma
                currentWeapon.PullTrigger();
            newPlasmas.AddRange(currentWeapon.NewBullets);//add 'barrel' list to regular plasma list

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

            switch (gameLogic.Execute(deltaTime))
            {
                case InstructionResult.DoneAndCreateAsteroid:
                    newAsteroids.Add(
                      new Entity(new Vector2(600.0f, (float)(randomGenerator.NextDouble() * 800.0)),
                        asteroidTexture));
                    break;
                case InstructionResult.RunningAndCreateAsteroid:
                    newAsteroids.Add(
                      new Entity(new Vector2(600.0f, (float)(randomGenerator.NextDouble() * 800.0)),
                        asteroidTexture));
                    break;
            }
          
            plasmas = newPlasmas;       //throws away old* plasma.
            asteroids = newAsteroids;   //throws away old* asteroids.   *out of bounds or hit asteroid/plasma object
            player = newplayer;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

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

//for debug
// System.Console.WriteLine("thing in here");
//System.Diagnostics.Debug.WriteLine("dfsdf");
// view->output