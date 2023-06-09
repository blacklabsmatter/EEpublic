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
using static System.Formats.Asn1.AsnWriter;

namespace EE
{
    public class Game1 : Game
    {
        public GraphicsDeviceManager graphics;
        public GraphicsDevice graphicsDevice;
        public SpriteBatch spriteBatch;
        public SpriteFont font;
        public List<CustomSprite> sprites = new List<CustomSprite>();
        public List<SequenceNode> behavior = new List<SequenceNode>();
        public List<VegSprite> vegsprites = new List<VegSprite>();
        public List<SequenceNode> vegbehavior = new List<SequenceNode>();
        public List<String> btresult = new List<String>();
        public double elapsedTime;
        public Random random = new Random();
        public int lastSpawnedIndex = 0;
        public double lastspawnedTimer = 0.0;
        public CustomSprite selectedSprite;
        public VegSprite selectedVegSprite;
        private SequenceNode behaviorTree;
        private BehaviorTreeData behaviorTreeData;

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
            int id = 1;
            Texture2D texture = Content.Load<Texture2D>("passerine1");
            Vector2 origin = (Vector2)vegsprites[lastSpawnedIndex].Position;
            Vector2 position = origin;
            Vector2 velocity = new Vector2(0,0);
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
            sprites.Add(new CustomSprite(id ,texture, position, origin, velocity, rotation, scale, age, true,
                radius, rotationSpeed, color, condition));

            // Increment the index to get the next vegsprite's position for the next spawn
            id++;
            lastSpawnedIndex++;

        }

        public Game1()
        {
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            // Create the behavior tree structure

            behaviorTree = new SequenceNode();
            ActionNode actionNode = new ActionNode();

            // Add the action node as a child of the sequence node
            behaviorTree.AddChild(actionNode);
        }
        public void InitializeGraphics()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        protected override void Initialize()
        {
            graphicsDevice = GraphicsDevice;
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;

            spriteBatch = new SpriteBatch(graphicsDevice);
            sprites = new List<CustomSprite>();
            behaviorTreeData = new BehaviorTreeData();

            SpriteFont font = Content.Load<SpriteFont>("Font");

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

            // Keyboard and gamead Input Handling
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Increment time as a double
            elapsedTime += gameTime.ElapsedGameTime.TotalSeconds; 
            
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Add sprite and vegsprite instances t their respective Lists and assign
            for (int i = 0; i < sprites.Count; i++)
            {
                CustomSprite sprite = sprites[i];
            }
            for (int i = 0; i < vegsprites.Count; i++)
            {
                VegSprite vegsprite = vegsprites[i];
            }

            // Sprite spawning instructions
            while (vegsprites.Count < 5)
            { 
                SpawnForest();
            }
            while (vegsprites.Count > sprites.Count && elapsedTime - lastspawnedTimer > 1)
            { 
                SpawnSprite(); lastspawnedTimer = elapsedTime;
            }

            // Wrap sprites, increment position by velocity, incremet age by time and update GameTime
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
                sprite.Age += elapsedSeconds;
                sprite.Update(gameTime);
            }

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
                foreach (VegSprite sprite in vegsprites)
                {
                    if (sprite.Bounds.Contains(mousePosition))
                    {
                        // Sprite clicked, do something
                        selectedVegSprite = sprite;
                    }
                }

            }

            btresult.Clear();
            foreach (CustomSprite sprite in sprites)
            {
                // Execute the behavior tree
                BehaviorTreeStatus treeStatus = behaviorTree.Tick(behaviorTreeData);

                if (treeStatus == BehaviorTreeStatus.Success)
                {
                    btresult.Add("Success".ToString());
                    // The behavior tree completed successfully
                    // Perform any necessary actions or logic
                }
                else if (treeStatus == BehaviorTreeStatus.Failure)
                {
                    btresult.Add("Failure".ToString());
                    // The behavior tree failed to complete
                    // Handle the failure case accordingly
                }
                else if (treeStatus == BehaviorTreeStatus.Running)
                {
                    btresult.Add("Running".ToString());
                    // The behavior tree is still running
                    // The update loop will continue executing the tree in the next frame
                }
            }
            // Update dynamic text based on game state, input, etc.
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Azure);

            // Draw sprite textures and selected textures
            spriteBatch.Begin();
            // Draw dynamic text using the font
            for (int i = 0; i < btresult.Count; i++)
            {
                Vector2 textPosition = new Vector2(10, 10 + (i * font.LineSpacing));
                spriteBatch.DrawString(font, btresult[i], textPosition, Color.Black);
            }
            foreach (VegSprite sprite in vegsprites)
            {

                // Draw the regular sprite texture
                spriteBatch.Draw(sprite.Texture, sprite.Position, sprite.Color1);
            }
            foreach (CustomSprite sprite in sprites)
            {
                // Draw the regular sprite texture
                spriteBatch.Draw(sprite.Texture, sprite.Position, sprite.Color1);
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
        public void Update(GameTime gameTime, float Age, float RotationSpeed, float elapsedSeconds)
        {

        }
    }
    public class CustomSprite
    {
        public int ID { get; set; }
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Origin { get; set; }
        public Vector2 Velocity { get; set; }
        public float Rotation { get; set; }
        public float Scale { get; set; }
        public float Age { get; set; }
        public bool Moving { get; set; }
        public float Radius { get; set; }
        public float RotationSpeed { get; set; }
        public Color Color1 { get; set; }
        public float Condition { get; set; }
        public Rectangle Bounds
        {
            get { return new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height); }
        }

        private Random random = new Random();
        private Vector2 GetRandomVector()
        {
            return new Vector2((((float)random.NextDouble() * 2) - 1), (((float)random.NextDouble() * 2) - 1));
        }
        public CustomSprite(int id, Texture2D texture, Vector2 position, Vector2 origin, Vector2 velocity,
            float rotation, float scale, float age, bool moving, float radius, float rotationSpeed, 
            Color color, float condition)
        {
            ID = id;
            Texture = texture;
            Position = position;
            Origin = origin;
            Velocity = velocity;
            Rotation = rotation;
            Scale = scale;
            Age = age;
            Moving = moving;
            Radius = radius;
            RotationSpeed = rotationSpeed;
            Color1 = color;
            Condition = condition;
        }

        public void Update(GameTime gameTime)
        {

        }
    }
}
