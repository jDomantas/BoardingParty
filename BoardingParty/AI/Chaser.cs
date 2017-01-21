using BoardingParty.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoardingParty.AI
{
    class Chaser : ComputerController
    {
        public override Vector? Move(Fighter fighter)
        {
            Vector targeting = Vector.Zero;

            for (int i = 0; i < fighter.World.Entities.Count; i++)
            {
                var target = fighter.World.Entities[i];
                if (target is Fighter && (target as Fighter).Team != fighter.Team)
                    targeting += (target.Position - fighter.Position).Normalized;
            }

            if (targeting.LengthSquared > 0.00001)
                return targeting.Normalized * 0.7;
            else
                return null;
        }
    }
}
