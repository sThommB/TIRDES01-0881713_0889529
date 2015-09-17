using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Linq;

namespace firstGame
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D playerTexture, attackTexture, rightMoveTexture, leftMoveTexture, downMoveTexture, upMoveTexture, idleTexture;//player
        Texture2D enemyTexture, friezaHurtTexture, background;//enemy
        Texture2D asteroidTexture;//asteroid
        Texture2D fireTexture;//plasma

        Rectangle mainFrame;//background

        KeyboardState newState;

        Player player = new Player(50, 500);
        Vector2 spritePosition, spritePositionEnemy, plasmaPosition;

        List<Bullet> bullets = new List<Bullet>();//bullets
        Bullet bullet;
        List<Asteroid> asteroids = new List<Asteroid>();//asteroids
        Asteroid asteroid;

        List<Vector2> asteroidPositions = new List<Vector2>();
        List<Vector2> plasmaPositions = new List<Vector2>();

        Random randomGenerator = new Random();

        int gameLogicScriptPC = 0;
        int rndNumberLine1, iLine1;
        float timeToWaitLine3, timeToWaitLine4, timeToWaitLine8, timeToWaitLine7;
        int rndNumberLine5, iLine5;

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

            // TODO: use this.Content to load your game content here
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

            bullet = new Bullet();
            asteroid = new Asteroid();
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            newState = Keyboard.GetState();//read keyboard
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            var newPlasmaPositions =
                (from plasmaPosition in plasmaPositions
                let colliders =
                    from asteroidPosition in asteroidPositions
                    where Vector2.Distance(plasmaPosition, asteroidPosition) < 20.0f
                
                select asteroidPosition
                where plasmaPosition.X > 0.0f &&
                    plasmaPosition.X < 800.0f &&
                    plasmaPosition.Y > 0.0f &&
                    plasmaPosition.Y < 600.0f &&
                    colliders.Count() == 0

                select plasmaPosition + Vector2.UnitX * 200.0f * deltaTime).ToList();
            if (newState.IsKeyDown(Keys.Space))
            {
                plasmaPosition = spritePosition;
                plasmaPosition.X += 25;
                plasmaPosition.Y += 15;
                newPlasmaPositions.Add(plasmaPosition);
            }

            var newAsteroidPositions =
                (from asteroidPosition in asteroidPositions
                let colliders =
                    from plasmaPosition in plasmaPositions
                    where Vector2.Distance(plasmaPosition, asteroidPosition) < 20.0f
                    select plasmaPosition
                where asteroidPosition.X > 0.0f &&
                        asteroidPosition.X < 800.0f &&
                        asteroidPosition.Y > 0.0f &&
                        asteroidPosition.Y < 600.0f &&
                        colliders.Count() == 0
                select asteroidPosition - Vector2.UnitX * 100.0f * deltaTime).ToList();

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
                    newAsteroidPositions.Add(new Vector2(600.0f, (float)(randomGenerator.NextDouble() * 800.0)));

                    /* asteroid = new Asteroid();
                     asteroid.activateAsteroid(new Vector2(800.0f, (float)(randomGenerator.NextDouble() * 600.0)));
                     asteroids.Add(asteroid);*/

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
                    newAsteroidPositions.Add(new Vector2(600.0f, (float)(randomGenerator.NextDouble() * 800.0)));

                    /*asteroid = new Asteroid();
                    asteroid.activateAsteroid(new Vector2(800.0f, (float)(randomGenerator.NextDouble() * 600.0)));
                    asteroids.Add(asteroid);*/

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

            // COMMIT CHANGES TO THE STATE
            plasmaPositions = newPlasmaPositions;
            asteroidPositions = newAsteroidPositions;

            bullet.updateBullets(bullets, 5, 0);//bullet movement
            bullet.deactivateBullet(bullets, graphics.PreferredBackBufferHeight);//delete off-screen bullets

            asteroid.updateAsteroid(asteroids, 5, 0);//bullet movement
            asteroid.deactivateAsteroid(asteroids, graphics.PreferredBackBufferHeight);//delete off-screen bullets

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spritePosition = new Vector2(player.getX, player.getY);//player position
            spritePositionEnemy = new Vector2(710, 500);

            //prevents border leaving  'border control'
            if (player.getX < 0)
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
            }

            spriteBatch.Begin();

            //hittest if player touches enemy
            if (((player.getX + playerTexture.Width) >= 710 && player.getX <= (710 + enemyTexture.Width)) && ((player.getY + enemyTexture.Height) >= 500 && player.getY <= (500 + enemyTexture.Height)))
            {
                System.Console.WriteLine("Hit");
                enemyTexture = friezaHurtTexture;
                
            }

            //Background
            spriteBatch.Draw(background, mainFrame, Color.White);
            mainFrame = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

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
                }*/               
            }

           /* foreach (Asteroid asteroid in asteroids)//check every asteroid
            {
                if (asteroid.IsActive == true)//only draw active asteroid
                {
                    spriteBatch.Draw(asteroidTexture, asteroid.asteroidPosition, Color.White);
                }
            }*/
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

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}


// System.Console.WriteLine("thing in here");
//System.Diagnostics.Debug.WriteLine("dfsdf");
// view->output