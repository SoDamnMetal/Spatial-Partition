using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spacial_Partition.QuadTrees.Movers {
	// from https://gamedev.stackexchange.com/questions/131133/monogames-most-efficient-way-of-checking-intersections-between-multiple-objects
	public class QuadTree {


		private readonly int MAX_OBJECTS = 10;
		private readonly int MAX_LEVELS = 7;

		private int level;
		private List<Mover> objects; //convert to your object type
		private List<Mover> returnObjects;
		private Rectangle bounds;
		private QuadTree[] nodes;
		

		public QuadTree(int pLevel, Rectangle pBounds) {
			level = pLevel;
			objects = new List<Mover>();
			bounds = pBounds;
			nodes = new QuadTree[4];
			returnObjects = new List<Mover>();
		}

		public void Clear() {
			objects.Clear();

			for (int i = 0; i < nodes.Length; i++) {
				if (nodes[i] != null) {
					nodes[i].Clear();
					nodes[i] = null;
				}
			}
		}


		private void Split() {
			int subWidth = (int)(bounds.Width / 2);
			int subHeight = (int)(bounds.Height / 2);
			int x = (int)bounds.X;
			int y = (int)bounds.Y;

			nodes[0] = new QuadTree(level + 1, new Rectangle(x + subWidth, y, subWidth, subHeight));
			nodes[1] = new QuadTree(level + 1, new Rectangle(x, y, subWidth, subHeight));
			nodes[2] = new QuadTree(level + 1, new Rectangle(x, y + subHeight, subWidth, subHeight));
			nodes[3] = new QuadTree(level + 1, new Rectangle(x + subWidth, y + subHeight, subWidth, subHeight));
		}

		
		// Determine which node the object belongs to. -1 means
		// object cannot completely fit within a child node and is part
		// of the parent node
		
		private int GetIndex(Mover pRect) {
			int index = -1;
			double verticalMidpoint = bounds.X + (bounds.Width / 2);
			double horizontalMidpoint = bounds.Y + (bounds.Height / 2);

			// Object can completely fit within the top quadrants
			bool topQuadrant = (pRect.Bounds.Y < horizontalMidpoint && pRect.Bounds.Y + pRect.Bounds.Height < horizontalMidpoint);
			// Object can completely fit within the bottom quadrants
			bool bottomQuadrant = (pRect.Bounds.Y > horizontalMidpoint);

			// Object can completely fit within the left quadrants
			if (pRect.Bounds.X < verticalMidpoint && bounds.X + pRect.Bounds.Width < verticalMidpoint) {
				if (topQuadrant) {
					index = 1;
				} else if (bottomQuadrant) {
					index = 2;
				}
			}
			 // Object can completely fit within the right quadrants
			 else if (pRect.Bounds.X > verticalMidpoint) {
				if (topQuadrant) {
					index = 0;
				} else if (bottomQuadrant) {
					index = 3;
				}
			}

			return index;
		}

		public void Insert(Mover pRect) {
			if (nodes[0] != null) {
				int index = GetIndex(pRect);

				if (index != -1) {
					nodes[index].Insert(pRect);

					return;
				}
			}

			objects.Add(pRect);

			if (objects.Count > MAX_OBJECTS && level < MAX_LEVELS) {
				if (nodes[0] == null) {
					Split();
				}

				int i = 0;
				while (i < objects.Count) {
					int index = GetIndex(objects[i]);
					if (index != -1) {
						nodes[index].Insert(objects[i]);
						objects.RemoveAt(i);
					} else {
						i++;
					}
				}
			}
		}

		
		// Return all objects that could collide with the given object (recursive)
		
		public void Retrieve(List<Mover> returnedObjs, Mover obj) {
			if (nodes[0] != null) {
				var index = GetIndex(obj);
				if (index != -1) {
					nodes[index].Retrieve(returnedObjs, obj);
				} else {
					for (int i = 0; i < nodes.Length; i++) {
						nodes[i].Retrieve(returnedObjs, obj);
					}
				}
			}
			returnedObjs.AddRange(objects);
		}

		public void Draw(SpriteBatch spriteBatch, GraphicsDevice g) {
			
			foreach (QuadTree node in nodes) {
				if (node != null) {
					node.Draw(spriteBatch, g);
					spriteBatch.Begin();
					spriteBatch.Draw(QuadTextures.Quad, new Rectangle(node.bounds.Left - 1, node.bounds.Top - 1, node.bounds.Width + 2, 1), Color.White);
					spriteBatch.Draw(QuadTextures.Quad, new Rectangle(node.bounds.Left - 1, node.bounds.Top - 1, 1, node.bounds.Height + 2), Color.White);
					spriteBatch.End();
				}
			}
		}

		public void Update(GameTime gameTime, List<Mover> movers) {

			Clear();
			foreach (var m in movers) {
				Insert(m);
			}

			for (int i = 0; i < movers.Count; i++) {
				returnObjects.Clear();
				Retrieve(returnObjects, movers[i]);

				for (int x = 0; x < returnObjects.Count; x++) {
					// Run collision detection algorithm between objects
					// i.e. your Rectangle.IntersectsWith(x)

					// Don't check for collisions with self object
					if (movers[i] == returnObjects[x]) { continue; }

					if (movers[i].Bounds.Intersects(returnObjects[x].Bounds)) {
						movers[i].Velocity = -movers[i].Velocity;
					}
				}
				movers[i].Update(gameTime);
			}
		}
	}
}

