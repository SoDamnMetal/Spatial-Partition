using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spacial_Partition.QuadTrees.Movers;

namespace Spacial_Partition.Extras {
	//idea from
	//https://conkerjo.wordpress.com/2009/06/13/spatial-hashing-implementation-for-fast-2d-collisions/
	//referencing
	//http://www.cs.ucf.edu/~jmesit/publications/scsc%202005.pdf
	public class Grid {

		List<Mover> movers;
		// key = index, value = list of objects
		Dictionary<int, List<Mover>> hashtable;

		int Width;
		int Height;
		int Cellsize;
		float Conversionfactor;

		Texture2D texture;

		public Grid(int worldwidth, int worldheight, int cellsize, List<Mover> allmovers) {
			Cellsize = cellsize;
			Width = worldwidth / cellsize;
			Height = worldheight / cellsize;
			hashtable = new Dictionary<int, List<Mover>>(Width * Height);
			movers = allmovers;
			Conversionfactor = 1f / cellsize;

			InitializeHashTable();
		}

		public void InitializeHashTable() {
			for (int i = 0; i < Width * Height; i++) {
				hashtable.Add(i, new List<Mover>());
			}
		}


		public void AddObjectsToHashTable() {
			foreach (Mover m in movers) {
				int[] bucketlist = GetBucketsObjectIsIn(m);
				foreach (int bucket in bucketlist) {
					hashtable[bucket].Add(m);
				}
			}
		}

		//the main hash function that retrieves an index value or "bucket id" for the given object
		public int[] GetBucketsObjectIsIn(Mover mover) {

			//get the objects width and height
			int width = (int)mover.Width;
			int height = (int)mover.Height;

			//get the objects 4 corners
			Vector2 position = new Vector2(mover.Position.X, mover.Position.Y);
			Vector2 topright = new Vector2(position.X + width, position.Y);
			Vector2 bottomleft = new Vector2(position.X, position.Y + height);
			Vector2 bottomright = new Vector2(position.X + width, position.Y + height);

			//Get all the buckets the object could be in
			//top left corner (position)
			int tlcellid = (int)(Math.Floor(position.X * Conversionfactor) + Math.Floor(position.Y * Conversionfactor) * Width);
			//top right corner
			int trcellid = (int)(Math.Floor(topright.X * Conversionfactor) + Math.Floor(topright.Y * Conversionfactor) * Width);
			//bottom left corner
			int blcellid = (int)(Math.Floor(bottomleft.X * Conversionfactor) + Math.Floor(bottomleft.Y * Conversionfactor) * Width);
			//bottom right corner
			int brcellid = (int)(Math.Floor(bottomright.X * Conversionfactor) + Math.Floor(bottomright.Y * Conversionfactor) * Width);

			int[] buckets = { tlcellid, trcellid, blcellid, brcellid };
			
			//return the array with duplicates omitted
			return buckets.Distinct().ToArray();

		}

		//return all objects in the cell or "bucket"
		public List<Mover> CellQuery(int index) {
			List<Mover> movers = new List<Mover>();
			hashtable[index].AddRange(movers);
			return movers;
		}

		//returns array of "buckets" the specific object is in
		public int[] ObjectQuery(Mover obj) {

			return GetBucketsObjectIsIn(obj);
		}

		//which objects are near object x
		public List<Mover> ProximityQuery(Mover obj) {
			List<Mover> objects = new List<Mover>();
			//get all the cells or "buckets" this object is in...
			int[] bucketIds = ObjectQuery(obj);
			
			//foreach of the cells the object is in, get all the objects in those cells from the hash table, and return them
			foreach (int index in bucketIds) {
				objects.AddRange(hashtable[index]);
			}
			return objects;
		}


		public virtual void Draw(SpriteBatch spriteBatch) {
			//horizontal
			for (int x = 0; x < Height + 1; x++) {
				spriteBatch.Draw(texture, new Rectangle(0, x * Cellsize, Width * Cellsize, 1), Color.White);
			}
			//vertical
			for (int y = 0; y < Width + 1; y++) {
				spriteBatch.Draw(texture, new Rectangle(y * Cellsize, 0, 1, Height * Cellsize), Color.White);
			}

		}
		public virtual void LoadContent(GraphicsDevice g) {
			texture = new Texture2D(g, 1, 1);
			texture.SetData(new Color[] { Color.Red });
		}

		public virtual void Update(GameTime gameTime) {
			hashtable.Clear();
			InitializeHashTable();

			//add and index all the objects into the hash table
			AddObjectsToHashTable();

			foreach (Mover m in movers) {
				//only performs collision on this list of nearby objects
				List<Mover> nearby = ProximityQuery(m);

				//collide before update because an object might occasionally get pushed outside the grid. 
				//If it updates first, the hashed key (object location) will be registered as a negative index and the program
				//will crash because that index isn't supposed to exist.

				//m.CollideRectangle(nearby);
				m.Update(gameTime);
			}
		}
	}
}
