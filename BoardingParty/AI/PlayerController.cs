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
        KeyboardState OldKeys, CurrentKeys;

        public Vector? Move(Fighter fighter)
        {
            OldKeys = CurrentKeys;
            CurrentKeys = Keyboard.GetState();
            
            Vector movement = Vector.Zero;
            if (CurrentKeys.IsKeyDown(Keys.W) || CurrentKeys.IsKeyDown(Keys.Up))
                movement.Y -= 1;
            if (CurrentKeys.IsKeyDown(Keys.S) || CurrentKeys.IsKeyDown(Keys.Down))
                movement.Y += 1;
            if (CurrentKeys.IsKeyDown(Keys.A) || CurrentKeys.IsKeyDown(Keys.Left))
                movement.X -= 1;
            if (CurrentKeys.IsKeyDown(Keys.D) || CurrentKeys.IsKeyDown(Keys.Right))
                movement.X += 1;

            return movement.LengthSquared > 0 ? movement.Normalized : default(Vector?);
        }

        public Entity Strike(Fighter fighter)
        {
            if (!OldKeys.IsKeyDown(Keys.Space) && CurrentKeys.IsKeyDown(Keys.Space))
            {
                Entity closest = null;
                for (int i = 0; i < fighter.World.Entities.Count; i++)
                {
                    if (fighter.World.Entities[i] == fighter)
                        continue;

                    if (closest == null)
                        closest = fighter.World.Entities[i];
                    else
                    {
                        double curr = (fighter.Position - closest.Position).LengthSquared;
                        double n = (fighter.Position - fighter.World.Entities[i].Position).LengthSquared;
                        if (n < curr)
                            closest = fighter.World.Entities[i];
                    }
                    
                }

                return closest;
            }

            return null;
        }
    }
}
