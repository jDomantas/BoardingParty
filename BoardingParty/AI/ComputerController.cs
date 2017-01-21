using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoardingParty.Entities;

namespace BoardingParty.AI
{
    abstract class ComputerController : FighterAI
    {
        private double AttackTimer;
        private Entity Target;


        public abstract Vector? Move(Fighter target);

        private Entity Mark(Entity target)
        {
            if (Target != target)
                AttackTimer = 0.33;

            Target = target;
            
            if (AttackTimer <= 0)
                return Target;
            else
                return null;
        }

        public void Update(double dt)
        {
            AttackTimer -= dt;
            if (AttackTimer <= 0)
                AttackTimer = 0;
        }

        public Entity Strike(Fighter fighter)
        {
            Entity closest = null;
            for (int i = 0; i < fighter.World.Entities.Count; i++)
            {
                if (fighter.World.Entities[i] == fighter)
                    continue;

                Fighter f = fighter.World.Entities[i] as Fighter;
                if (f == null)
                    continue;

                if (f.Team == fighter.Team)
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
                double dist = (closest.Position - fighter.Position).LengthSquared;
                if (dist > Fighter.AttackRange * Fighter.AttackRange)
                    return Mark(null);
            }

            return Mark(closest);
        }
    }
}
