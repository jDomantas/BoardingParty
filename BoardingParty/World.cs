using BoardingParty.AI;
using BoardingParty.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoardingParty
{
    class World
    {
        public const double SwingTime = 7;
        public const double EdgeBouncines = 0.2;
        public const double Bounciness = 0.7;

        public Vector Size { get; }
        public List<Entity> Entities { get; }
        public Vector Gravity { get; private set; }

        private double Time;

        public World(Vector size)
        {
            Size = size;
            Entities = new List<Entity>();

            Entities.Add(new Fighter(this, new PlayerController()));
            Entities.Add(new Barrel(this, 200) { Position = Size / 2 });
            Entities.Add(new Barrel(this, 200) { Position = Size / 2 + new Vector(300, 0), Friction = 0.003 });
            Entities.Add(new Barrel(this, 200) { Position = Size / 2 + new Vector(0, 500), Friction = 0 });
            Entities.Add(new Barrel(this, 200) { Position = Size / 2 + new Vector(500, 0), Friction = 0 });
            Entities.Add(new Barrel(this, 200) { Position = Size / 2 + new Vector(0, 300), Friction = 0.03 });
        }

        public void Update(double dt)
        {
            Time += dt;
            Gravity = new Vector(Math.Sin(Time / SwingTime * Math.PI * 2) * 90, Math.Sin(Time / (SwingTime * 1.4) * Math.PI * 2 + 123) * 15);

            for (int i = 0; i < Entities.Count; i++)
                Entities[i].Update(dt);

            ResolveCollisions();
        }

        private void ResolveCollisions()
        {
            for (int i = 0; i < Entities.Count; i++)
                Entities[i].Force = Entities[i].PositionFix = Vector.Zero;

            for (int i = 0; i < Entities.Count; i++)
            {
                ResolveCollisions(Entities[i]);

                for (int j = 0; j < Entities.Count; j++)
                {
                    if (i == j)
                        continue;

                    ResolveCollisions(Entities[i], Entities[j]);
                }
            }

            for (int i = 0; i < Entities.Count; i++)
            {
                Entities[i].Position += Entities[i].PositionFix;
                Entities[i].Velocity += Entities[i].Force;
            }
        }

        private void ResolveCollisions(Entity a, Entity b)
        {
            if ((a.Position - b.Position).LengthSquared >= (a.Radius + b.Radius) * (a.Radius + b.Radius))
                return;

            Vector diff = (b.Position - a.Position);
            if (diff.LengthSquared > 1)
            {
                double dist = diff.Length;
                Vector fix = diff.Normalized * (a.Radius + b.Radius - dist) / 2;

                b.PositionFix += fix;
                a.PositionFix -= fix;

                Vector v = (b.Velocity - a.Velocity);
                double collisionSpeed = v.Length;

                double bounceSpeed = diff.Normalized.Dot(v);
                Vector bounce = bounceSpeed * diff.Normalized;

                Vector aDelta = bounce / (a.Mass + b.Mass) * b.Mass * Bounciness;
                Vector bDelta = bounce / (a.Mass + b.Mass) * a.Mass * Bounciness;

                a.Force += aDelta;
                b.Force -= bDelta;

                a.GotHit(bounce);
                b.GotHit(bounce);
            }
        }

        private void ResolveCollisions(Entity a)
        {
            if (a.Position.X - a.Radius < -Size.X)
            {
                a.PositionFix.X += (-Size.X - a.Position.X + a.Radius);
                if (a.Velocity.X < 0)
                    a.Velocity.X *= -EdgeBouncines;
            }
            else if (a.Position.X + a.Radius > Size.X)
            {
                a.PositionFix.X -= (a.Position.X + a.Radius - Size.X);
                if (a.Velocity.X > 0)
                    a.Velocity.X *= -EdgeBouncines;
            }

            if (a.Position.Y - a.Radius < -Size.Y)
            {
                a.PositionFix.Y += (-Size.Y - a.Position.Y + a.Radius);
                if (a.Velocity.Y < 0)
                    a.Velocity.Y *= -EdgeBouncines;
            }
            else if (a.Position.Y + a.Radius > Size.Y)
            {
                a.PositionFix.Y -= (a.Position.Y + a.Radius - Size.Y);
                if (a.Velocity.Y > 0)
                    a.Velocity.Y *= -EdgeBouncines;
            }
        }

        public void Draw(SpriteBatch sb)
        {
            double viewHeight = Size.Y * 2.2;
            double viewScale = BoardingGame.ScreenHeight / viewHeight;
            
            Matrix scale = Matrix.CreateScale((float)viewScale);
            Matrix translation = Matrix.CreateTranslation(BoardingGame.ScreenWidth / 2f, BoardingGame.ScreenHeight / 2f, 0);
            
            sb.Begin(transformMatrix: scale * translation);
            
            Color deckColor = new Color(207, 161, 119);
            sb.Draw(Resources.Textures.Pixel, new Rectangle((int)-Size.X, (int)-Size.Y, (int)(Size.X * 2), (int)(Size.Y * 2)), deckColor);

            for (int i = 0; i < Entities.Count; i++)
                Entities[i].Draw(sb);

            sb.Draw(Resources.Textures.Pixel, new Rectangle((int)(Gravity.X * 30) - 100, (int)(Gravity.Y * 30) - 100, 200, 200), Color.Blue);

            sb.End();
        }
    }
}
