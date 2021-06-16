using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spacial_Partition.QuadTrees.Movers {
    class Game1 : Game{

		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;
		private Vector2 world;
		private QuadTree quad;
		private List<Mover> movers;

		public Game1() {
			graphics = new GraphicsDeviceManager(this);
			graphics.IsFullScreen = true;
			Content.RootDirectory = "Content";
			IsMouseVisible = false;
			world = new Vector2(1920, 1080);
		}

		
		protected override void Initialize() {
		
			graphics.PreferredBackBufferHeight = (int)world.Y;
			graphics.PreferredBackBufferWidth = (int)world.X;
			graphics.ApplyChanges();

			Random rand = new Random();
			movers = new List<Mover>();
			for (int i = 0; i < 2500; i++) {
				movers.Add(new Mover(new Rectangle(rand.Next((int)world.X), rand.Next((int)world.Y), 4, 4), world));
			}

			quad = new QuadTree(0, new Rectangle(0, 0, (int)world.X, (int)world.Y));
			
			

			base.Initialize();
		}

		protected override void LoadContent() {
			spriteBatch = new SpriteBatch(GraphicsDevice);

			QuadTextures.LoadContent(GraphicsDevice, Content);

		}

		protected override void Update(GameTime gameTime) {

			quad.Update(gameTime, movers);

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime) {
			GraphicsDevice.Clear(Color.DarkBlue);

			quad.Draw(spriteBatch, GraphicsDevice);
			
			foreach (Mover obj in movers) {
				obj.Draw(spriteBatch);
			}

			base.Draw(gameTime);
		}

	}
}
