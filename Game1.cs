using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Drawing;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using MonoGame.Extended.Sprites;
using static EE.Game1;

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
        private VegSprite selectedVegSprite;

        public enum VelocityModifierMethod
        {
            Method1,
            Circle,
            Method3,
            // Add more methods as needed
        }


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
            float condition = 100;
            bool isTextureLoaded = false;
            vegsprites.Add(new VegSprite(texture, position, rotation, scale, age, true, radius, color, condition, isTextureLoaded));
        }


        private void SpawnSprite()
        {
            Texture2D texture = Content.Load<Texture2D>("passerine1");
            Vector2 origin = (Vector2)vegsprites[lastSpawnedIndex].Position;
            Vector2 position = origin;
            VelocityModifierMethod movement = VelocityModifierMethod.Method1;
            Vector2 velocity = new Vector2((((float)random.NextDouble() * 2) - 1), (((float)random.NextDouble() * 2) - 1));
            float rotation = (float)random.Next(0, 360);
            float scale = 1;
            float age = 0f;
            bool moving = true;
            float radius = 0;
            float rotationSpeed = (float)random.NextDouble() * 2 - 1; // generates a number between -1 and 1
            while (rotationSpeed == 0) // keep generating new numbers until non-zero is produced
            {
                rotationSpeed = (float)random.NextDouble() * 2 - 1;
            }
            Color color = vegsprites[lastSpawnedIndex].Color1;
            lastspawnedTimer = elapsedTime;
            float condition = 100;
            bool isTextureLoaded= false;
            sprites.Add(new CustomSprite(texture, position, origin, velocity, rotation, scale, age, true, movement, 
                radius, rotationSpeed, color, condition, isTextureLoaded));

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
                        sprite.IsTextureLoaded = true;
                    }
                }
                foreach (VegSprite sprite in vegsprites)
                {
                    if (sprite.Bounds.Contains(mousePosition))
                    {
                        // Sprite clicked, do something
                        selectedVegSprite = sprite;
                        sprite.IsTextureLoaded = true;
                    }
                }

            }
            // Keyboard and gamead Input Handling
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Increment time as a double
            elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;
                // Update the age of each sprite
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            foreach (CustomSprite sprite in sprites)
            {
                sprite.Age += elapsedSeconds;
            }
            foreach (VegSprite sprite in vegsprites)
            {
                sprite.Age += elapsedSeconds;
            }

            // Sprite spawning instructions
            while (vegsprites.Count < 5)
            { SpawnForest(); }
            while (vegsprites.Count > sprites.Count && elapsedTime - lastspawnedTimer > 1)
            { SpawnSprite(); lastspawnedTimer = elapsedTime; }
            // Wrap sprites and increment position by velocity
            foreach (CustomSprite sprite in sprites)
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

                // Main velocity production
                sprite.Position += sprite.Velocity;
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Azure);

            // Draw sprite textures and selected textures
            spriteBatch.Begin();
            foreach (VegSprite sprite in vegsprites)
            {
                if (sprite == selectedVegSprite)
                {
                    Texture2D selected = Content.Load<Texture2D>("selected");
                    spriteBatch.Draw(selected, sprite.Position, Color.White);
                }
                else
                {
                    // Draw the regular sprite texture
                    spriteBatch.Draw(sprite.Texture, sprite.Position, sprite.Color1);
                }
            }
            
            foreach (CustomSprite sprite in sprites)
            {
                // Draw the selected sprite with a different texture
                if (sprite == selectedSprite)
                {
                    // Draw the selected texture on top of the sprite
                    int y = 10; // Starting y position of text
                    Texture2D selected = Content.Load<Texture2D>("selected");
                    spriteBatch.Draw(selected, sprite.Position, Color.White);
                    spriteBatch.DrawString(font, $"VelocityModifier: {sprite.VelocityModifier}", new Vector2(10, y), Color.Black);
                    y += 20; // Increase y position for next line of text
                }
                else
                {
                    // Draw the regular sprite texture
                    spriteBatch.Draw(sprite.Texture, sprite.Position, sprite.Color1);
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
        public Rectangle Bounds
        {
            get { return new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height); }
        }
        public float Condition;
        public bool IsTextureLoaded = false;
        public VegSprite(Texture2D texture, Vector2 position, float rotation, float scale, float age, 
            bool moving, float radius, Color color, float condition, bool isTextureLoaded)
        {
            Texture = texture;
            Position = position;
            Rotation = rotation;
            Scale = scale;
            Age = age;
            Moving = moving;
            Radius = radius;
            Color1 = color;
            IsTextureLoaded = isTextureLoaded;
            Condition = condition;
        }
        public void Update(GameTime gameTime, float Age, float RotationSpeed)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Moving)
            {
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
        public VelocityModifierMethod VelocityModifier { get; set; }
        public float Radius { get; set; }
        public float RotationSpeed { get; set; }
        public Color Color1 { get; set; }
        public float Condition { get; set; }
        public Rectangle Bounds
        {
            get { return new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height); }
        }
        public bool IsTextureLoaded = false;
        private Vector2 GetCircleVector()
        {
            float angle = (float)Math.Atan2(Position.Y - Origin.Y, Position.X - Origin.X);
            angle += RotationSpeed;
            float distance = Vector2.Distance(Position, Origin);
            float x = Origin.X + (float)Math.Cos(angle) * distance;
            float y = Origin.Y + (float)Math.Sin(angle) * distance;
            return new Vector2(x - Position.X, y - Position.Y);
        }
        private Random random = new Random();
        private Vector2 GetRandomVector()
        {
            return new Vector2((((float)random.NextDouble() * 2) - 1), (((float)random.NextDouble() * 2) - 1));
        }
        public CustomSprite(Texture2D texture, Vector2 position, Vector2 origin, Vector2 velocity,
            float rotation, float scale, float age, bool moving, VelocityModifierMethod movement, float radius, float rotationSpeed, 
            Color color, float condition, bool isTextureLoaded)
        {
            Texture = texture;
            Position = position;
            Origin = origin;
            Velocity = velocity;
            Rotation = rotation;
            Scale = scale;
            Age = age;
            Moving = moving;
            VelocityModifier = movement;
            Radius = radius;
            RotationSpeed = rotationSpeed;
            Color1 = color;
            Condition = condition;
            IsTextureLoaded = isTextureLoaded;
        }
        public void Update(GameTime gameTime, float Age, float RotationSpeed)
        {
            //measure distace from origin
            Radius = Vector2.Distance(Position, Origin);

            switch (VelocityModifier)
            {
                case VelocityModifierMethod.Method1:
                    Velocity += GetRandomVector();
                    break;
                case VelocityModifierMethod.Circle:
                    Velocity += GetCircleVector();
                    break;
            }
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Age >= 5f && VelocityModifier != VelocityModifierMethod.Circle)
            {
                VelocityModifier = VelocityModifierMethod.Circle;
            }

            if (Moving)
            {


                //Set rotation angle
                Rotation += RotationSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }
    }


}
