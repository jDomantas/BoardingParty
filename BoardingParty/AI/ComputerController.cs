using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoardingParty.Entities;

namespace BoardingParty.AI
{
    class ComputerController : FighterAI
    {
        public Vector? Move(Fighter fighter)
        {
            Vector targeting = Vector.Zero;

            for (int i = 0; i < fighter.World.Entities.Count; i++)
            {
                var target = fighter.World.Entities[i];
                if (target is Fighter && target != fighter)
                    targeting += (target.Position - fighter.Position) / (target.Position - fighter.Position).LengthSquared;
            }

            if (targeting.LengthSquared > 0)
                targeting = targeting.Normalized;

            Vector gravityAvoidance = new Vector(-fighter.World.Gravity.X, 0);

            if (gravityAvoidance.LengthSquared > 0)
                gravityAvoidance = gravityAvoidance.Normalized;

            return (targeting + gravityAvoidance).Normalized;
        }

        public Entity Strike(Fighter fighter)
        {
            Entity closest = null;
            for (int i = 0; i < fighter.World.Entities.Count; i++)
            {
                if (fighter.World.Entities[i] == fighter || !(fighter.World.Entities[i] is Fighter))
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

            if (closest != null)
            {
                double dist = (closest.Position - fighter.Position).Length;
                if (dist > Fighter.AttackRange - 150)
                    return null;
            }

            return closest;
        }
    }
}
