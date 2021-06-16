using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spacial_Partition.QuadTrees.Movers {
	public class Mover {

		public int Height;
		public int Width;
		public Vector2 Position;
		public Vector2 Velocity;
		public Vector2 World;
		public Rectangle Bounds {
			get {
				return new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
			}
		}

		public Mover(Rectangle bounds, Vector2 world) {
			Height = bounds.Height;
			Width = bounds.Width;
			Position = new Vector2(bounds.Left, bounds.Top);
			Velocity = GetRandomVector();
			World = world;
		}

		public void Update(GameTime gameTime) {

			UpdatePosition();
			BounceOffEdges();
		}

		public void Draw(SpriteBatch spriteBatch) {
			spriteBatch.Begin();
			spriteBatch.Draw(QuadTextures.Mover, new Rectangle((int)Position.X, (int)Position.Y, Width, Height), Color.White);
			spriteBatch.End();
		}

		private void BounceOffEdges() {
			if (Position.X + Width > World.X) {
				Velocity.X = -Velocity.X;

			} else if (Position.X < 0) {
				Velocity.X = -Velocity.X;
			}
			if (Position.Y + Height > World.Y) {
				Velocity.Y = -Velocity.Y;
			} else if (Position.Y < 0) {
				Velocity.Y = -Velocity.Y;
			}
		}

		public Vector2 GetRandomVector() {
			Random rand = new Random();

			Vector2 vector = new Vector2((float)Math.Cos(rand.Next(90)), -(float)Math.Sin(rand.Next(90)));
			vector.Normalize();

			return vector;
		}

		public void UpdatePosition() {

			Position += Velocity;

		}
	}
}
