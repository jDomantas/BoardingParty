using BoardingParty.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoardingParty.Entities
{
    class Fighter : Entity
    {
        public FighterAI AI;

        public Fighter(World world, FighterAI ai) : base(world, 200)
        {
            AI = ai;
            Friction = 0;
        }

        public override void Update(double dt)
        {
            Vector movement = AI.Move(this).GetValueOrDefault();
            Vector targetVelocity = movement * 4700;
            
            Velocity = (Velocity * (10 - Control) + targetVelocity * Control) / 10;

            base.Update(dt);
        }
    }
}
