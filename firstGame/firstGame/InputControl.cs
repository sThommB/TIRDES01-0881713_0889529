﻿using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Content;

namespace firstGame
{
    abstract class InputController
    {
        static public InputController operator +(InputController a, InputController b)
        {
            return new ControllerSum(a, b);
        }

        public abstract bool Quit { get; }
        public abstract Vector2 PlayerMovement { get; }
        public abstract bool Shooting { get; }

        public abstract void Update(float dt);
    }

    class KeyboardController : InputController
    {
        KeyboardState ks;

        public override bool Quit
        {
            get
            {
                return ks.IsKeyDown(Keys.Escape);
            }
        }

        public override Vector2 PlayerMovement
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

        public override bool Shooting
        {
            get
            {
                return ks.IsKeyDown(Keys.Space);
            }
        }

        public override void Update(float dt)
        {
            ks = Keyboard.GetState();
        }
    }

    class MouseController : InputController
    {
        MouseState ms;

        public override bool Quit
        {
            get
            {
                return false;
            }
        }

        public override Vector2 PlayerMovement
        {
            get
            {
                return new Vector2(ms.X - 400, ms.Y - 300) * 0.01f;
            }
        }

        public override bool Shooting
        {
            get
            {
                return ms.LeftButton == ButtonState.Pressed;
            }
        }

        public override void Update(float dt)
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

        public override bool Quit
        {
            get
            {
                return first.Quit || second.Quit;
            }
        }

        public override Vector2 PlayerMovement
        {
            get
            {
                return first.PlayerMovement + second.PlayerMovement;
            }
        }

        public override bool Shooting
        {
            get
            {
                return first.Shooting || second.Shooting;
            }
        }

        public override void Update(float dt)
        {
            first.Update(dt);
            second.Update(dt);
        }
    }
}