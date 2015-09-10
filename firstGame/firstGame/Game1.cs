﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace firstGame
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D playerTexture;
        Texture2D attackTexture;
        Texture2D rightMoveTexture;
        Texture2D leftMoveTexture;
        Texture2D downMoveTexture;
        Texture2D upMoveTexture;
        Texture2D idleTexture;

        KeyboardState newState;

        Player player = new Player(0, 0);
        Vector2 spritePosition;

        List<Bullet> bullets = new List<Bullet>();
        Bullet bullet;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //graphics.PreferredBackBufferWidth = 1920;
            //graphics.PreferredBackBufferHeight = 1080;
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
            attackTexture = Content.Load<Texture2D>("pictures/blast");
            rightMoveTexture = Content.Load<Texture2D>("pictures/GokuSSJ/gokuRight");
            leftMoveTexture = Content.Load<Texture2D>("pictures/GokuSSJ/gokuLeft");
            downMoveTexture = Content.Load<Texture2D>("pictures/GokuSSJ/gokuUpDown");
            upMoveTexture = Content.Load<Texture2D>("pictures/GokuSSJ/gokuUpDown");
            idleTexture = Content.Load<Texture2D>("pictures/GokuSSJ/gokuIdle");
            bullet = new Bullet();
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
                bullet = new Bullet();
                bullet.activateBullet(spritePosition, attackTexture, 25, 15);
                bullets.Add(bullet);
            }

            bullet.updateBullets(bullets, 5, 0);//bullet movement
            bullet.deactivateBullet(bullets, graphics.PreferredBackBufferHeight);//delete off-screen bullets

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spritePosition = new Vector2(player.getX, player.getY);//player position

            spriteBatch.Begin();

            //player
            spriteBatch.Draw(playerTexture, spritePosition, Color.White);
           
            //bullets
            foreach (Bullet bullet in bullets)//check every bullet
            {
                if (bullet.IsActive == true)//only draw active bullets
                {
                    spriteBatch.Draw(bullet.bulletTexture, bullet.bulletPosition, Color.White);
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}


// System.Console.WriteLine("thing in here");
//System.Diagnostics.Debug.WriteLine("dfsdf");
// view->output