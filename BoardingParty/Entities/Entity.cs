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
        public double Radius { get; }
        public World World { get; }
        public double Friction = 1000;
        public double Mass = 1;
        public bool Dead;
        public double Rotation = 0;

        public Entity(World world, double radius)
        {
            World = world;
            Radius = radius;
        }
        
        public virtual void Update(double dt)
        {
            if (Velocity.X != 0 || Velocity.Y != 0)
                Rotation = Math.Atan2(Velocity.Y, Velocity.X);

            Velocity += World.Gravity * 2;

            Velocity *= 0.995;
            double len = Velocity.Length;
            if (len > Friction * dt)
                Velocity = Velocity.Normalized * (len - Friction * dt);
            else
                Velocity = Vector.Zero;
            
            Position += Velocity * dt;        }
        
        public virtual void Hit(Vector deltav)
        {
            Velocity += deltav;
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
