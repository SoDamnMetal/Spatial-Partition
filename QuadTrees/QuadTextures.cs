using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Spacial_Partition.QuadTrees {
	public static class QuadTextures {
		
		public static Texture2D Quad;
		public static Texture2D Mover;
		public static Texture2D Boid;
		public static void LoadContent(GraphicsDevice g, ContentManager c) {
			Quad = new Texture2D(g, 1, 1);
			Quad.SetData(new Color[] { Color.Yellow });

			Mover = new Texture2D(g, 1, 1);
			Mover.SetData(new Color[] { Color.White });

			Boid = c.Load<Texture2D>("Boid");
		}
	}
}
