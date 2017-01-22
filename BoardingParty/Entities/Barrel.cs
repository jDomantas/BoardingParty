using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace BoardingParty.Entities
{
    class Barrel : Entity
    {
        public Barrel(World world, double radius) : base(world, radius)
        {
            Mass = 3.5;
            Friction = 200;
        }

        public override void Draw(SpriteBatch sb)
        {
            int x = (int)Math.Round(Position.X);
            int y = (int)Math.Round(Position.Y);
            int d = (int)Math.Round(Radius * 2);
            Rectangle rect = new Rectangle(x, y, d, d);
            int center = Resources.Textures.Circle.Width / 2;

            Color color = Color.White * (float)RemovalWait;

            sb.Draw(Resources.Textures.Barrel, new Rectangle(x, y, d * 2, d * 2), null, color, 0, new Vector2(250, 250), SpriteEffects.None, 0);
        }
    }
}
