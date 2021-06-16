using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spacial_Partition.QuadTrees.Boids {
	// from https://gamedev.stackexchange.com/questions/131133/monogames-most-efficient-way-of-checking-intersections-between-multiple-objects
	public class QuadTree {

		private readonly int MAX_OBJECTS = 4;
		private readonly int MAX_LEVELS = 7;

		private int level;
		private List<Boid> objects; // convert to your object type
		private List<Boid> returnObjects;
		private List<Boid> nearby;
		private Rectangle bounds;
		private QuadTree[] nodes;


		public QuadTree(int pLevel, Rectangle pBounds) {
			level = pLevel;
			objects = new List<Boid>();
			bounds = pBounds;
			nodes = new QuadTree[4];
			returnObjects = new List<Boid>();
			nearby = new List<Boid>();
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
			int subWidth = bounds.Width / 2;
			int subHeight = bounds.Height / 2;
			int x = bounds.X;
			int y = bounds.Y;

			nodes[0] = new QuadTree(level + 1, new Rectangle(x + subWidth, y, subWidth, subHeight));
			nodes[1] = new QuadTree(level + 1, new Rectangle(x, y, subWidth, subHeight));
			nodes[2] = new QuadTree(level + 1, new Rectangle(x, y + subHeight, subWidth, subHeight));
			nodes[3] = new QuadTree(level + 1, new Rectangle(x + subWidth, y + subHeight, subWidth, subHeight));
		}

	
		// Determine which node the object belongs to. -1 means
		// object cannot completely fit within a child node and is part
		// of the parent node

		private int GetIndex(Boid pRect) {
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

		public void Insert(Boid pRect) {
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

		
		//Return all objects that could collide with the given object (recursive)
		public void Retrieve(List<Boid> returnedObjs, Boid obj) {
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
					spriteBatch.Draw(QuadTextures.Quad, new Rectangle(node.bounds.Left - 1, node.bounds.Top - 1, node.bounds.Width, 1), Color.White);
					spriteBatch.Draw(QuadTextures.Quad, new Rectangle(node.bounds.Left - 1, node.bounds.Top - 1, 1, node.bounds.Height), Color.White);
					spriteBatch.End();
				}
			}
		}

		public void Update(GameTime gameTime, List<Boid> boids) {

			Clear();
			foreach (Boid boid in boids) {
				Insert(boid);
			}

			for (int i = 0; i < boids.Count; i++) {
				
				nearby.Clear();
				returnObjects.Clear();
				Retrieve(returnObjects, boids[i]);
				
				for (int x = 0; x < returnObjects.Count; x++) {

					// Don't check distance with self
					if (boids[i] == returnObjects[x]) { continue; }

					
					if (boids[i].Distance(boids[i], returnObjects[x]) <= boids[i].VisualRange) {
						nearby.Add(returnObjects[x]);
						
					}

				}
				boids[i].DoBoidThings(nearby);
				boids[i].Update(gameTime);
			}
		}
	}
}