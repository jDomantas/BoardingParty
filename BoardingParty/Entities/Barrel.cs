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
            Mass = 1;
            Friction = 200;
        }

        public override void Draw(SpriteBatch sb)
        {
            int x = (int)Math.Round(Position.X);
            int y = (int)Math.Round(Position.Y);
            int d = (int)Math.Round(Radius * 2);
            Rectangle rect = new Rectangle(x, y, d, d);
            int center = Resources.Textures.Circle.Width / 2;

            var color = Color.Red;
            sb.Draw(Resources.Textures.Circle, rect, null, color, 0, new Vector2(center, center), SpriteEffects.None, 0);
        }
    }
}
