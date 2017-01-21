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
        public const double Acceleration = 12000;
        public const double AttackRange = 700;
        public const double AttackStrength = 7000;

        public bool HasControl;
        public FighterAI AI;

        public int Team;

        public Fighter(World world, FighterAI ai, int team) : base(world, 200)
        {
            AI = ai;
            Friction = 1000;
            HasControl = true;
            Team = team;
        }

        public override void Update(double dt)
        {
            if (HasControl)
            {
                Vector movement = AI.Move(this).GetValueOrDefault();
                Vector targetVelocity = movement * 3500;

                Vector diff = targetVelocity - Velocity;
                double acc = Acceleration;
                //acc -= Math.Sqrt(Velocity.Length) * 30;
                acc *= dt;
                if (diff.LengthSquared > acc * acc)
                    diff = diff.Normalized * acc;

                Velocity += diff;
            }

            Friction = HasControl ? 2000 : 1000;

            if (Velocity.LengthSquared < 2500 * 2500)
                HasControl = true;

            if (HasControl)
            {
                Entity target = AI.Strike(this);
                if (target != null && (target.Position - Position).LengthSquared < AttackRange * AttackRange)
                {
                    Vector d = (target.Position - Position).Normalized;
                    if (target.Velocity.LengthSquared < AttackStrength * AttackStrength / 2)
                    {
                        target.Velocity = Vector.Zero;
                        target.Hit(AttackStrength * d);
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

            var color = HasControl ? Color.Black : new Color(64, 64, 64);
            sb.Draw(Resources.Textures.Circle, rect, null, color, 0, new Vector2(center, center), SpriteEffects.None, 0);
        }

        public override void Hit(Vector deltav)
        {
            base.Hit(deltav);
            if (deltav.LengthSquared > 3000 * 3000)
                HasControl = false;
        }
    }
}
