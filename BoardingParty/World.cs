﻿using BoardingParty.AI;
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
        public const double SwingTime = 8;
        public const double EdgeBouncines = 0.5;

        public Vector Size { get; }
        public List<Entity> Entities { get; }
        public Vector Gravity { get; private set; }

        private double Time;
        private Random Random { get; }

        public World(Vector size)
        {
            Random = new Random();
            Size = size;
            Entities = new List<Entity>();
            
            Entities.Add(new Barrel(this, 200) { Position = Size / 2 });
            /*Entities.Add(new Barrel(this, 200) { Position = Size / 2 + new Vector(300, 0), Friction = 0.003 });
            Entities.Add(new Barrel(this, 200) { Position = Size / 2 + new Vector(0, 500), Friction = 0 });
            Entities.Add(new Barrel(this, 200) { Position = Size / 2 + new Vector(500, 0), Friction = 0 });
            Entities.Add(new Barrel(this, 200) { Position = Size / 2 + new Vector(0, 300), Friction = 0.03 });*/
        }

        private int BarrelCount()
        {
            int cnt = 0;
            for (int i = 0; i < Entities.Count; i++)
                if (Entities[i] is Barrel)
                    cnt++;
            return cnt;
        }

        private int FighterCount(int team)
        {
            int cnt = 0;
            for (int i = 0; i < Entities.Count; i++)
                if (Entities[i] is Fighter && ((Fighter)Entities[i]).Team == team)
                    cnt++;
            return cnt;
        }

        public void Update(double dt)
        {
            Time += dt;
            Gravity = new Vector(Math.Sin(Time / SwingTime * Math.PI * 2) * 30, Math.Sin(Time / (SwingTime * 1.4) * Math.PI * 2 + 123) * 5);

            for (int i = 0; i < Entities.Count; i++)
                Entities[i].Update(dt);

            ResolveCollisions();

            for (int i = Entities.Count - 1; i >= 0; i--)
            {
                if (Entities[i].Dead)
                    Entities.RemoveAt(i);
            }

            while (BarrelCount() < 6)
            {
                double x = (Random.NextDouble() * 2 - 1) * Size.X * 0.7;
                double vy = 2000 + 500 * Random.NextDouble();
                Entities.Add(new Barrel(this, 200) { Position = new Vector(x, -Size.Y - 1000), Velocity = new Vector(0, vy) });
            }

            while (FighterCount(1) < 1)
            {
                double x = (Random.NextDouble() * 2 - 1) * Size.X * 0.7;
                double vy = 2000 + 500 * Random.NextDouble();
                Entities.Add(new Fighter(this, new ComputerController(), 1) { Position = new Vector(x, -Size.Y - 1000), Velocity = new Vector(0, vy) });
            }

            while (FighterCount(2) < 1)
            {
                double x = (Random.NextDouble() * 2 - 1) * Size.X * 0.7;
                double vy = 2000 + 500 * Random.NextDouble();
                Entities.Add(new Fighter(this, new PlayerController(), 2) { Position = new Vector(x, Size.Y + 1000), Velocity = new Vector(0, -vy) });
            }
        }

        private void ResolveCollisions()
        {
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

                b.Position += fix;
                a.Position -= fix;
                
                double totalMass = a.Mass + b.Mass;
                Vector av = a.Velocity - 2 * b.Mass * (a.Velocity - b.Velocity).Dot(a.Position - b.Position) * (a.Position - b.Position) / (totalMass * (a.Position - b.Position).LengthSquared);
                Vector bv = b.Velocity - 2 * a.Mass * (b.Velocity - a.Velocity).Dot(b.Position - a.Position) * (b.Position - a.Position) / (totalMass * (a.Position - b.Position).LengthSquared);

                Vector da = av - a.Velocity;
                Vector db = bv - b.Velocity;

                a.Hit(da);
                b.Hit(db);

            }
        }

        private void ResolveCollisions(Entity a)
        {
            if (a.Position.X - a.Radius < -Size.X)
            {
                if (ShouldKill(-a.Velocity.X, -Gravity.X))
                    a.Dead = true;
                else
                {
                    a.Position.X += (-Size.X - a.Position.X + a.Radius);
                    if (a.Velocity.X < 0)
                        a.Velocity.X *= -EdgeBouncines;
                }
            }
            else if (a.Position.X + a.Radius > Size.X)
            {
                if (ShouldKill(a.Velocity.X, Gravity.X))
                    a.Dead = true;
                else
                {
                    a.Position.X -= (a.Position.X + a.Radius - Size.X);
                    if (a.Velocity.X > 0)
                        a.Velocity.X *= -EdgeBouncines;
                }
            }

            if (a.Position.Y - a.Radius < -Size.Y)
            {
                if (a.Velocity.Y < 0)
                {
                    a.Position.Y += (-Size.Y - a.Position.Y + a.Radius);
                    if (a.Velocity.Y < 0)
                        a.Velocity.Y *= -EdgeBouncines;
                }
                else
                {
                    a.Velocity.Y = Math.Max(2000, a.Velocity.Y);
                }
            }
            else if (a.Position.Y + a.Radius > Size.Y)
            {
                if (a.Velocity.Y > 0)
                {
                    a.Position.Y -= (a.Position.Y + a.Radius - Size.Y);
                    if (a.Velocity.Y > 0)
                        a.Velocity.Y *= -EdgeBouncines;
                }
                else
                {
                    a.Velocity.Y = Math.Min(-2000, a.Velocity.Y);
                }
            }
        }

        private bool ShouldKill(double velocity, double gravity)
        {
            return velocity + gravity * 100 > 8000;
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

            //sb.Draw(Resources.Textures.Pixel, new Rectangle((int)(Gravity.X * 30) - 100, (int)(Gravity.Y * 30) - 100, 200, 200), Color.Blue);

            sb.End();
        }
    }
}