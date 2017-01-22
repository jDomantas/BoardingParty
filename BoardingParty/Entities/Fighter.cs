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
        public const double AttackAnimationLength = 0.25;

        public const double Acceleration = 25000;
        public const double AttackRange = 1200;
        public const double TypicalAttackStrength = 7000;

        public double AttackStrength;
        public double KnockoutResistance;
        public bool HasControl;
        public FighterAI AI;
        public double DistanceWalked;
        public double AttackTimer;
        public Vector AttackTarget;

        public double LastRotation;

        public int Team;

        public Fighter(World world, FighterAI ai, int team) : base(world, 200)
        {
            AI = ai;
            Friction = 1000;
            HasControl = true;
            Team = team;
            AttackStrength = 6500;
            KnockoutResistance = 3300;
        }

        public override void Update(double dt)
        {
            if (!Dead)
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
                    DistanceWalked += Velocity.Length;
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

                        AttackTarget = target.Position;
                        if (AttackTimer <= 0)
                            AttackTimer = AttackAnimationLength;
                    }
                }

                AttackTimer -= dt;
                if (AttackTimer < 0)
                    AttackTimer = 0;
                else
                    Rotation = Math.Atan2(AttackTarget.Y - Position.Y, AttackTarget.X - Position.X);
            }

            double rotDelta = Rotation - LastRotation;
            while (rotDelta > Math.PI) rotDelta -= Math.PI * 2;
            while (rotDelta < -Math.PI) rotDelta += Math.PI * 2;

            double rotSpeed = (Math.Exp(Math.Abs(rotDelta)) * 2 + 3);
            rotSpeed *= dt;
            if (rotDelta < -rotSpeed) LastRotation -= rotSpeed;
            else if (rotDelta > rotSpeed) LastRotation += rotSpeed;
            else LastRotation = Rotation;

            base.Update(dt);
        }

        public override void Draw(SpriteBatch sb)
        {
            double scale = 1;
            if (Dead)
            {
                if (RemovalWait > 0.7)
                    scale = 1.4 - (RemovalWait - 0.7) * (RemovalWait - 0.7) * 0.4 / 0.09;
                else
                    scale = 1.4 - (RemovalWait - 0.7) * (RemovalWait - 0.7) * 1.5;
            }

            int x = (int)Math.Round(Position.X);
            int y = (int)Math.Round(Position.Y);
            int d = (int)Math.Round(Radius * 2 * scale);
            Rectangle rect = new Rectangle(x, y, d, d);
            int center = Resources.Textures.Circle.Width / 2;
            
            int walkFrame = (int)Math.Floor(DistanceWalked / 10000) % 8;

            var texture = AI is NotSoArtificial ?
                (HasControl ? Resources.Textures.Pirate : Dead ? Resources.Textures.PirateDead : Resources.Textures.PirateOw) :
                (HasControl ? Resources.Textures.Defender : Dead ? Resources.Textures.DefenderDead : Resources.Textures.DefenderOw);

            if (HasControl && !Dead)
            {
                texture = AI is NotSoArtificial ?
                    Resources.Textures.PirateWalk[walkFrame] :
                    Resources.Textures.DefenderWalk[walkFrame];

                d *= 2;
                rect.Width *= 2;
                rect.Height *= 2;

                if (AttackTimer > 0)
                {
                    int frame = 5 - Math.Min(5, Math.Max(1, (int)Math.Ceiling(AttackTimer / AttackAnimationLength * 5)));

                    texture = AI is NotSoArtificial ?
                        Resources.Textures.PirateAttack[frame] :
                        Resources.Textures.DefenderAttack[frame];
                }
            }

            Color color = Color.White * (float)RemovalWait;

            sb.Draw(
                texture,
                new Rectangle(x, y, d * 7 / 4, d * 7 / 4),
                null,
                color,
                (float)LastRotation - MathHelper.PiOver2 + (HasControl ? 0 : MathHelper.Pi),
                new Vector2(texture.Width / 2, texture.Height / 2),
                SpriteEffects.None,
                0);

            /*sb.Draw(
                Resources.Textures.Pixel,
                new Rectangle(x, y, d * 7 / 4, d * 7 / 4),
                null,
                Color.White * 0.7f,
                (float)LastRotation - MathHelper.PiOver2 + (HasControl ? 0 : MathHelper.Pi),
                new Vector2(0.5f, 0.5f),
                SpriteEffects.None,
                0);*/

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
            return new Fighter(world, new NotSoArtificial(), 1)
            {
                Position = position,
                Velocity = velocity,
                AttackStrength = 9000,
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
