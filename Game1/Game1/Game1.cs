using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Game1
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        Vector2 spookPosition;
        Texture2D spookAppearance;
        protected override void Initialize()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spookPosition = new Vector2(400, 400);
            spookAppearance = Content.Load<Texture2D>("Spook.png");

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();
            Func<bool, float> toUnit = b => b ? 1.0f : 0.0f;
            var spookDelta = Vector2.Zero;
            spookDelta += Vector2.UnitX * toUnit(keyboardState.IsKeyDown(Keys.D));
            spookDelta -= Vector2.UnitX * toUnit(keyboardState.IsKeyDown(Keys.A));
            spookDelta += Vector2.UnitY * toUnit(keyboardState.IsKeyDown(Keys.S));
            spookDelta -= Vector2.UnitY * toUnit(keyboardState.IsKeyDown(Keys.W));
            spookPosition += spookDelta * 5.0f;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            spriteBatch.Draw(spookAppearance, spookPosition, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}