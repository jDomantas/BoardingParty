using BoardingParty.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoardingParty.AI
{
    class Avoider : ComputerController
    {
        public override Vector? Move(Fighter fighter)
        {
            Vector targeting = Vector.Zero;

            for (int i = 0; i < fighter.World.Entities.Count; i++)
            {
                var target = fighter.World.Entities[i];
                if (target is Fighter && target != fighter && (target as Fighter).Team != fighter.Team)
                    targeting += (target.Position - fighter.Position) / (target.Position - fighter.Position).LengthSquared;
            }

            if (targeting.LengthSquared > 0)
                targeting = targeting.Normalized;

            Vector gravityAvoidance = new Vector(-fighter.World.Gravity.X, 0);

            if (gravityAvoidance.LengthSquared > 0)
                gravityAvoidance = gravityAvoidance.Normalized;

            return (targeting + gravityAvoidance).Normalized;
        }
    }
}
