using BoardingParty.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoardingParty.AI
{
    class BarrelLauncher : FighterAI
    {
        public Vector? Move(Fighter fighter)
        {
            Entity target = fighter.World.Entities.FirstOrDefault(s => s is Fighter && ((Fighter)s).Team != fighter.Team);
            if (target == null)
                return null;
            
            Entity nearest = null;
            Vector destination = Vector.Zero;
            for (int i = 0; i < fighter.World.Entities.Count; i++)
            {
                var barrel = fighter.World.Entities[i] as Barrel;
                if (barrel == null)
                    continue;

                var targetPos = barrel.Position + (barrel.Position - target.Position).Normalized * Fighter.AttackRange * 0.9;
                if (nearest == null || (fighter.Position - targetPos).LengthSquared < (fighter.Position - destination).LengthSquared)
                {
                    nearest = barrel;
                    destination = targetPos;
                }
            }

            if (nearest == null)
                return null;

            Vector navigation = (nearest.Position - fighter.Position).Normalized;

            Vector avoidance = Vector.Zero;
            for (int i = 0; i < fighter.World.Entities.Count; i++)
            {
                var e = fighter.World.Entities[i];
                if (e == fighter) continue;

                if ((e.Position - fighter.Position).Length < Fighter.AttackRange * 0.7)
                    avoidance += (fighter.Position - e.Position).Normalized;
            }

            if (navigation.LengthSquared > 0.0001 && avoidance.LengthSquared > 0.0001 &&
                navigation.Cross(avoidance) / navigation.Length / avoidance.Length < 0.2)
            {
                navigation = navigation.Left;
            }

            Vector total = navigation + avoidance;
            if (total.LengthSquared <= 0.000001)
                return Vector.Zero;
            else
                return total.Normalized;
        }

        public void Update(double dt) { }

        public Entity Strike(Fighter fighter)
        {
            Entity target = fighter.World.Entities.FirstOrDefault(s => s is Fighter && ((Fighter)s).Team != fighter.Team);
            if (target == null)
                return null;

            Entity nearest = null;
            double bestProduct = 0;
            for (int i = 0; i < fighter.World.Entities.Count; i++)
            {
                var e = fighter.World.Entities[i];
                if (e == fighter)
                    continue;

                if ((fighter.Position - e.Position).LengthSquared > Fighter.AttackRange * Fighter.AttackRange * 4)
                    continue;

                double dot = (e.Position - fighter.Position).Dot(target.Position - e.Position) / (e.Position - fighter.Position).Length / (target.Position - e.Position).Length;
                if ((nearest == null || dot < bestProduct) && dot > 0.93)
                {
                    nearest = e;
                    bestProduct = dot;
                }
            }

            return nearest;
        }
    }
}
