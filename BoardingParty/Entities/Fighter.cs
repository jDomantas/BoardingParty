using BoardingParty.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace BoardingParty.Entities
{
    class Fighter : Entity
    {
        public const double Acceleration = 25000;
        public const double AttackRange = 1200;
        public const double TypicalAttackStrength = 7000;

        public double AttackStrength;
        public double KnockoutResistance;
        public bool HasControl;
        public FighterAI AI;

        public int Team;

        public Fighter(World world, FighterAI ai, int team) : base(world, 200)
        {
            AI = ai;
            Friction = 1000;
            HasControl = true;
            Team = team;
            AttackStrength = 7000;
            KnockoutResistance = 3300;
        }

        public override void Update(double dt)
        {
            AI.Update(dt);

            if (HasControl)
            {
                Vector movement = AI.Move(this).GetValueOrDefault();
                Vector targetVelocity = movement * 4000;

                Vector diff = targetVelocity - Velocity;
                double acc = Acceleration * dt;
                if (diff.LengthSquared > acc * acc)
                    diff = diff.Normalized * acc;

                Velocity += diff;
            }

            Friction = HasControl ? 2000 : 1000;

            if (Velocity.LengthSquared < KnockoutResistance * KnockoutResistance * 0.6)
                HasControl = true;

            if (HasControl)
            {
                Entity target = AI.Strike(this);
                if (target != null && (target.Position - Position).LengthSquared < AttackRange * AttackRange)
                {
                    Vector d = (target.Position - Position).Normalized;
                    double str = AttackStrength;
                    if (target.Velocity.LengthSquared < str * str / 2)
                    {
                        target.Velocity = Vector.Zero;
                        target.Hit(str * d);
                    }
                }
            }
            
            base.Update(dt);
        }

        public override void Draw(SpriteBatch sb)
        {
            int x = (int)Math.Round(Position.X);
            int y = (int)Math.Round(Position.Y);
            int d = (int)Math.Round(Radius * 2);
            Rectangle rect = new Rectangle(x, y, d, d);
            int center = Resources.Textures.Circle.Width / 2;

            /*var color = HasControl ? Color.Black : new Color(64, 64, 64);
            if (Team == 2) color = HasControl ? Color.Blue : new Color(64, 64, 255);
            sb.Draw(Resources.Textures.Circle, rect, null, color, 0, new Vector2(center, center), SpriteEffects.None, 0);
            if (AI is PlayerController)
                sb.Draw(Resources.Textures.Circle, new Rectangle(x, y, d / 3, d / 3), null, Color.Yellow, 0, new Vector2(center, center), SpriteEffects.None, 0);*/
            sb.Draw(
                AI is PlayerController ? Resources.Textures.Pirate : Resources.Textures.Defender,
                new Rectangle(x, y, d * 7 / 4, d * 7 / 4),
                null,
                Color.White,
                (float)Rotation - MathHelper.PiOver2,
                new Vector2(Resources.Textures.Pirate.Width / 2, Resources.Textures.Pirate.Height / 2),
                SpriteEffects.None,
                0);

            /*sb.Draw(
                Resources.Textures.Circle,
                new Rectangle(x, y, d, d),
                null,
                Color.White * 0.4f,
                0,
                new Vector2(Resources.Textures.Circle.Width / 2, Resources.Textures.Circle.Height / 2),
                SpriteEffects.None,
                0);*/
        }

        public override void Hit(Vector deltav)
        {
            base.Hit(deltav);
            if (deltav.LengthSquared > KnockoutResistance * KnockoutResistance)
                HasControl = false;
        }

        public static Fighter CreatePlayer(World world, Vector position, Vector velocity)
        {
            return new Fighter(world, new PlayerController(), 1)
            {
                Position = position,
                Velocity = velocity,
                AttackStrength = 10000,
                KnockoutResistance = 5000,
            };
        }

        public static Fighter CreateEnemy(World world, Vector position, Vector velocity)
        {
            FighterAI ai = null;
            do
            {
                int aiType = world.Random.Next(3);
                if (aiType == 0 && world.Entities.All(e => !(e is Fighter) || !((e as Fighter).AI is Avoider))) ai = new Avoider();
                if (aiType == 1 && world.Entities.All(e => !(e is Fighter) || !((e as Fighter).AI is Chaser))) ai = new Chaser();
                if (aiType == 2 && world.Entities.All(e => !(e is Fighter) || !((e as Fighter).AI is BarrelLauncher))) ai = new BarrelLauncher();
            } while (ai == null);

            return new Fighter(world, ai, 2)
            {
                Position = position,
                Velocity = velocity,
            };
        }
    }
}
