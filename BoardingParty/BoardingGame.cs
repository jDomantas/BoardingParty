using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace BoardingParty
{
    public class BoardingGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        World world;

        public static int ScreenWidth = 1280;
        public static int ScreenHeight = 720;

        RenderTarget2D renderTarget;

        float time;

        public BoardingGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            IsMouseVisible = true;
            graphics.PreferredBackBufferWidth = ScreenWidth;
            graphics.PreferredBackBufferHeight = ScreenHeight;
        }
        

        protected override void Initialize()
        {
            base.Initialize();

            world = new World(new Vector(3300 * 1.8f, 3000 * 1.8f));
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Resources.Textures.Pixel = new Texture2D(GraphicsDevice, 1, 1);
            Resources.Textures.Pixel.SetData(new Color[] { Color.White });

            int radius = 50;
            Resources.Textures.Circle = new Texture2D(GraphicsDevice, radius * 2, radius * 2);
            Color[] color = new Color[radius * radius * 4];
            for (int x = 0; x < radius * 2; x++)
                for (int y = 0; y < radius * 2; y++)
                {
                    double dist = (x - radius + 0.5) * (x - radius + 0.5) + (y - radius + 0.5) * (y - radius + 0.5);
                    dist = Math.Sqrt(dist);
                    if (dist > radius)
                        color[x + y * radius * 2] = new Color(0, 0, 0, 0);
                    else if (dist < radius * 0.9)
                        color[x + y * radius * 2] = Color.White;
                    else
                    {
                        double blur = 0.05;
                        dist = Math.Min(1, Math.Max(0, 1 - (dist - radius * (1 - blur)) / (radius * blur)));
                        float val = (float)dist;
                        color[x + y * radius * 2] = new Color(val, val, val, val);
                    }
                }
            Resources.Textures.Circle.SetData(color);

            renderTarget = new RenderTarget2D(GraphicsDevice, ScreenWidth, ScreenHeight);
        }
        
        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            world.Update(gameTime.ElapsedGameTime.TotalSeconds);

            time += 1 / 60f;

            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(renderTarget);

            GraphicsDevice.Clear(new Color(0, 0, 0, 0));
            
            world.Draw(spriteBatch);

            GraphicsDevice.SetRenderTarget(null);

            Matrix w = Matrix.Identity;
            Vector2 camera = Vector2.Zero;
            camera = new Vector2((float)world.Gravity.X, (float)world.Gravity.Y) / -20000;
            Matrix view = Matrix.CreateLookAt(new Vector3(camera, 2.4f), Vector3.Zero, Vector3.UnitY);
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 1, 0.1f, 10000f);
            
            Matrix transform = w * view * projection * Matrix.CreateTranslation(ScreenWidth / 2, ScreenHeight / 2, 0);

            GraphicsDevice.RasterizerState = new RasterizerState() { CullMode = CullMode.None };

            GraphicsDevice.Clear(Color.CornflowerBlue);

            float dx = (float)Math.Cos(time) * 10;
            float dy = (float)Math.Sin(time) * 10;

            spriteBatch.Begin(transformMatrix: transform);
            spriteBatch.Draw(renderTarget, new Rectangle(-renderTarget.Width / 2, -renderTarget.Height / 2, renderTarget.Width, renderTarget.Height), Color.White);
            spriteBatch.End();

            spriteBatch.Begin();

            int team1 = world.TeamScores.ContainsKey(1) ? world.TeamScores[1] : 0;
            int team2 = world.TeamScores.ContainsKey(2) ? world.TeamScores[2] : 0;

            int x = 0;
            for (int i = 0; i < team1 / 10; i++)
            {
                spriteBatch.Draw(Resources.Textures.Pixel, new Rectangle(i * 14 + 2, 2, 10, 10), Color.Gold);
                x += 14;
            }
            for (int i = 0; i < team1 % 10; i++)
                spriteBatch.Draw(Resources.Textures.Pixel, new Rectangle(x + i * 14 + 5, 5, 4, 4), Color.Gold);

            x = 0;
            for (int i = 0; i < team2 / 10; i++)
            {
                spriteBatch.Draw(Resources.Textures.Pixel, new Rectangle(i * 14 + 2, 16, 10, 10), Color.Gold);
                x += 14;
            }
            for (int i = 0; i < team2 % 10; i++)
                spriteBatch.Draw(Resources.Textures.Pixel, new Rectangle(x + i * 14 + 5, 19, 4, 4), Color.Gold);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
