using BoardingParty.Entities;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoardingParty.AI
{
    class PlayerController : FighterAI
    {
        public Vector? Move(Fighter fighter)
        {
            KeyboardState keys = Keyboard.GetState();
            Vector movement = Vector.Zero;
            if (keys.IsKeyDown(Keys.W) || keys.IsKeyDown(Keys.Up))
                movement.Y -= 1;
            if (keys.IsKeyDown(Keys.S) || keys.IsKeyDown(Keys.Down))
                movement.Y += 1;
            if (keys.IsKeyDown(Keys.A) || keys.IsKeyDown(Keys.Left))
                movement.X -= 1;
            if (keys.IsKeyDown(Keys.D) || keys.IsKeyDown(Keys.Right))
                movement.X += 1;

            return movement.LengthSquared > 0 ? movement : default(Vector?);
        }

        public Vector? Strike(Fighter fighter)
        {
            return null;
        }
    }
}
