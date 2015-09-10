using Microsoft.Xna.Framework;
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
        Texture2D enemyTexture;
        Texture2D friezaHurtTexture;
        Texture2D background;


        KeyboardState newState;

        Player player = new Player(0, 0);
        Vector2 spritePosition;
        Vector2 spritePositionEnemy;

        List<Bullet> bullets = new List<Bullet>();
        Bullet bullet;

        Rectangle mainFrame;

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
            attackTexture = Content.Load<Texture2D>("pictures/blast");
            rightMoveTexture = Content.Load<Texture2D>("pictures/GokuSSJ/gokuRight");
            leftMoveTexture = Content.Load<Texture2D>("pictures/GokuSSJ/gokuLeft");
            downMoveTexture = Content.Load<Texture2D>("pictures/GokuSSJ/gokuUpDown");
            upMoveTexture = Content.Load<Texture2D>("pictures/GokuSSJ/gokuUpDown");
            idleTexture = Content.Load<Texture2D>("pictures/GokuSSJ/gokuIdle");
            enemyTexture = Content.Load<Texture2D>("pictures/Frieza/FriezaIdle");
            friezaHurtTexture = Content.Load<Texture2D>("pictures/Frieza/FriezaHurt1");
            background = Content.Load<Texture2D>("pictures/Background/Arena");
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
            spritePositionEnemy = new Vector2(300, 300);

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
            if (((player.getX + playerTexture.Width) >= 300 && player.getX <= (300 + enemyTexture.Width)) && ((player.getY + enemyTexture.Height) >= 300 && player.getY <= (300 + enemyTexture.Height)))
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
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}


// System.Console.WriteLine("thing in here");
//System.Diagnostics.Debug.WriteLine("dfsdf");
// view->output