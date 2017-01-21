using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoardingParty.Entities
{
    abstract class Entity
    {
        public Vector Position;
        public Vector Velocity;
        public Vector Force, PositionFix;
        public double Radius { get; }
        public World World { get; }
        public double Control;
        public double Friction = 0.015;
        public double Mass = 1;

        public Entity(World world, double radius)
        {
            World = world;
            Radius = radius;
            Control = 1;
        }
        
        public virtual void Update(double dt)
        {
            Velocity += World.Gravity;
            Position += Velocity * dt;

            Velocity *= (1 - Friction);
            
            double gain = 0.2 + (2000 - Velocity.Length) / 2000;
            if (gain < 0.2)
                gain = 0.2;

            Control += dt * gain;
            if (Control > 1)
                Control = 1;
        }

        public void GotHit(Vector speed)
        {
            if (speed.Length > 2000)
                Control = 0;
        }

        public virtual void Draw(SpriteBatch sb)
        {
            int x = (int)Math.Round(Position.X);
            int y = (int)Math.Round(Position.Y);
            int d = (int)Math.Round(Radius * 2);
            Rectangle rect = new Rectangle(x, y, d, d);
            int center = Resources.Textures.Circle.Width / 2;
            
            sb.Draw(Resources.Textures.Circle, rect, null, Color.Black, 0, new Vector2(center, center), SpriteEffects.None, 0);
        }
    }
}
