using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace BoardingParty.Entities
{
    class Obstacle : Entity
    {
        public Obstacle(World world, Vector position, double radius) : base(world, radius)
        {
            Position = position;
            Mass = 1000000;
        }

        public override void Update(double dt)
        {
            Velocity = Vector.Zero;
        }

        public override void Draw(SpriteBatch sb)
        {
            // base.Draw(sb);
        }

        public override void Hit(Vector deltav)
        {
            
        }
    }
}
