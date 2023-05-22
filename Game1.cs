using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Drawing;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace EE
{
    public class Game1 : Game
    {
        public GraphicsDeviceManager graphics;
        private GraphicsDevice graphicsDevice;
        SpriteBatch spriteBatch;
        private SpriteFont font;
        List<CustomSprite> sprites = new List<CustomSprite>();
        List<VegSprite> vegsprites = new List<VegSprite>();
        double elapsedTime;
        Random random = new Random();
        private int lastSpawnedIndex = 0;
        private Double lastspawnedTimer = 0.0;
        private CustomSprite selectedSprite;
        Texture2D selected;

        private void SpawnForest()
        {
            Texture2D texture = Content.Load<Texture2D>("forest1");
            Vector2 position = new Vector2(random.Next(0, 800), random.Next(0, 600));
            float rotation = (float)random.Next(0, 360);
            float scale = 1;
            float age = 0f;
            bool moving = false;
            float radius = random.Next(100, 100);
            float rotationSpeed = random.Next(-5, 5); // generates a number between -1 and 1
            while (rotationSpeed == 0) // keep generating new numbers until non-zero is produced
            {
                rotationSpeed = (float)random.NextDouble() * 2 - 1;
            }
            Color color = new Color(random.Next(255), random.Next(255), random.Next(255), 255);

            vegsprites.Add(new VegSprite(texture, position, rotation, scale, age, true, radius, color));
        }


        private void SpawnSprite()
        {
            Texture2D texture = Content.Load<Texture2D>("passerine1");
            Vector2 origin = (Vector2)vegsprites[lastSpawnedIndex].Position;
            Vector2 position = origin;
            Vector2 velocity = new Vector2((float)random.NextDouble() * 2 - 1, (float)random.NextDouble() * 2 - 1);
            float rotation = (float)random.Next(0, 360);
            float scale = 1;
            float age = 0f;
            bool moving = true;
            int movement = 1;
            float radius = (float)random.Next(10, 50);
            float rotationSpeed = (float)random.NextDouble() * 2 - 1; // generates a number between -1 and 1
            while (rotationSpeed == 0) // keep generating new numbers until non-zero is produced
            {
                rotationSpeed = (float)random.NextDouble() * 2 - 1;
            }
            Color color = vegsprites[lastSpawnedIndex].Color1;
            lastspawnedTimer = elapsedTime;
            float condition = 100;
            sprites.Add(new CustomSprite(texture, position, origin, velocity, rotation, scale, age, true, movement, radius, rotationSpeed, color, condition));

            // Increment the index to get the next vegsprite's position for the next spawn
            lastSpawnedIndex++;

        }

        public Game1()
        {
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }
        public void InitializeGraphics()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        protected override void Initialize()
        {
            graphicsDevice = GraphicsDevice;
            spriteBatch = new SpriteBatch(graphicsDevice);
            sprites = new List<CustomSprite>();
            SpriteFont font = Content.Load<SpriteFont>("Font");

            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;

            LoadContent();
            graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            font = Content.Load<SpriteFont>("Font");
            spriteBatch = new SpriteBatch(graphicsDevice);

            sprites = new List<CustomSprite>();
            vegsprites = new List<VegSprite>();

            // TODO: use this.Content to load your game content here

        }

        protected override void Update(GameTime gameTime)
        {
            // Handle mouse input
            MouseState mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);

                // Check if a sprite was clicked
                foreach (CustomSprite sprite in sprites)
                {
                    if (sprite.Bounds.Contains(mousePosition))
                    {
                        // Sprite clicked, do something
                        selectedSprite = sprite;
                    }
                }
            }
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;
            while (vegsprites.Count < 5)
            { SpawnForest(); }
            while (vegsprites.Count > sprites.Count && elapsedTime - lastspawnedTimer > 1)
            { SpawnSprite(); lastspawnedTimer = elapsedTime; }
            foreach (CustomSprite sprite in sprites)// Wrap sprites and increment position by velocity
            {
                if (sprite.Position.Y > graphics.PreferredBackBufferHeight)
                {
                    sprite.Position = new Vector2(sprite.Position.X, -sprite.Texture.Height);
                }
                if (sprite.Position.Y < -sprite.Texture.Height)
                {
                    sprite.Position = new Vector2(sprite.Position.X, graphics.PreferredBackBufferHeight);
                }
                if (sprite.Position.X > graphics.PreferredBackBufferWidth)
                {
                    sprite.Position = new Vector2(-sprite.Texture.Width, sprite.Position.Y);
                }
                if (sprite.Position.X < -sprite.Texture.Width)
                {
                    sprite.Position = new Vector2(graphics.PreferredBackBufferWidth, sprite.Position.Y);
                }
                sprite.Position += sprite.Velocity;
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Azure);

            spriteBatch.Begin();
            foreach (VegSprite sprite in vegsprites)
            {
                spriteBatch.Draw(sprite.Texture, sprite.Position, sprite.Color1);
            }
            int y = 10; // Starting y position of text
            foreach (CustomSprite sprite in sprites)
            {
                spriteBatch.Draw(sprite.Texture, sprite.Position, sprite.Color1);
                spriteBatch.DrawString(font, $"Velocity: {sprite.Velocity}", new Vector2(10, y), Color.Black);
                y += 20; // Increase y position for next line of text
            }
            foreach (CustomSprite sprite in sprites)
            {
                // Draw the selected sprite with a different texture
                if (sprite == selectedSprite)
                {
                    // Draw the selected texture on top of the sprite
                    Texture2D selected = Content.Load<Texture2D>("selected");
                    spriteBatch.Draw(selected, sprite.Position, Color.White);
                }
                else
                {
                    // Draw the regular sprite texture
                    spriteBatch.Draw(sprite.Texture, sprite.Position, Color.White);
                }
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
    public class VegSprite
    {
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public float Scale { get; set; }
        public float Age { get; set; }
        public bool Moving { get; set; }
        public float Radius { get; set; }
        public Color Color1 { get; set; }
        public VegSprite(Texture2D texture, Vector2 position, float rotation, float scale, float age, bool moving, float radius, Color color)
        {
            Texture = texture;
            Position = position;
            Rotation = rotation;
            Scale = scale;
            Age = age;
            Moving = moving;
            Radius = radius;
            Color1 = color;
        }
        public void Update(GameTime gameTime, float Age, float RotationSpeed)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Moving)
            {
                //Increase Age by 1 per second
                Age += (float)gameTime.ElapsedGameTime.TotalSeconds;

                //Set rotation angle
                Rotation += RotationSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }
    }
    public class CustomSprite
    {
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Origin { get; set; }
        public Vector2 Velocity { get; set; }
        public float Rotation { get; set; }
        public float Scale { get; set; }
        public float Age { get; set; }
        public bool Moving { get; set; }
        public int Movement { get; set; }
        public float Radius { get; set; }
        public float RotationSpeed { get; set; }
        public Color Color1 { get; set; }
        public float Condition { get; set; }
        public Rectangle Bounds
        {
            get { return new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height); }
        }
        public CustomSprite(Texture2D texture, Vector2 position, Vector2 origin, Vector2 velocity,
            float rotation, float scale, float age, bool moving, int movement, float radius, float rotationSpeed, Color color, float condition)
        {
            Texture = texture;
            Position = position;
            Origin = origin;
            Velocity = velocity;
            Rotation = rotation;
            Scale = scale;
            Age = age;
            Moving = moving;
            Movement = movement;
            Radius = radius;
            RotationSpeed = rotationSpeed;
            Color1 = color;
        }
        public void Update(GameTime gameTime, float Age, float RotationSpeed)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Moving)
            {
                //Increase Age by 1 per second
                Age += (float)gameTime.ElapsedGameTime.TotalSeconds;

                //Set rotation angle
                Rotation += RotationSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }
    }


}
