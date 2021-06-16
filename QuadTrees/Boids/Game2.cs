using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Spacial_Partition.QuadTrees.Boids {
	class Game2 : Game {

		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;
		private Vector2 world;
		private QuadTree quad;
		private List<Boid> boids;

		public Game2() {
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
			boids = new List<Boid>();
			for (int i = 0; i < 1500; i++) {
				boids.Add(new Boid(new Vector2(rand.Next((int)world.X), rand.Next((int)world.Y)), GetRandomVector(), new Vector2(10, 10), world));
			}

			quad = new QuadTree(0, new Rectangle(0, 0, (int)world.X, (int)world.Y));



			base.Initialize();
		}

		protected override void LoadContent() {
			spriteBatch = new SpriteBatch(GraphicsDevice);

			QuadTextures.LoadContent(GraphicsDevice, Content);

		}

		protected override void Update(GameTime gameTime) {

			quad.Update(gameTime, boids);

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime) {
			GraphicsDevice.Clear(new Color(40, 44, 52));

			quad.Draw(spriteBatch, GraphicsDevice);

			foreach (Boid obj in boids) {
				obj.Draw(spriteBatch);
			}

			base.Draw(gameTime);
		}
		public Vector2 GetRandomVector() {
			Random rand = new Random();
			Vector2 dxdy = new Vector2((float)rand.NextDouble() * 10 - 5, (float)rand.NextDouble() * 10 - 5);
			dxdy.Normalize();
			return dxdy;
		}
	}
}

