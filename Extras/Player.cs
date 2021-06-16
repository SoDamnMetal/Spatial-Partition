using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Spacial_Partition.QuadTrees.Movers;

namespace Spacial_Partition.Extras {
    
   public class Player : Mover {

        private Texture2D texture;

        public Player(Rectangle bounds, Vector2 world) :base(bounds, world) {
			this.World = world;
            Position = new Vector2(bounds.X, bounds.Y);
            Velocity = new Vector2(0, 0);

        }

        private void Move() {
            bool w = Keyboard.GetState().IsKeyDown(Keys.W);
            bool s = Keyboard.GetState().IsKeyDown(Keys.S);
            bool a = Keyboard.GetState().IsKeyDown(Keys.A);
            bool d = Keyboard.GetState().IsKeyDown(Keys.D);
            
            Velocity = Vector2.Zero; 

            if (w) { MoveUp(); } 
            else if (s) { MoveDown(); }
            if (a) { MoveLeft(); } 
            else if (d) { MoveRight(); }
            
            
        }

        protected void MoveUp() {
            Velocity.Y = -1.5f;
        }
        protected void MoveDown() {
            Velocity.Y = 1.5f;
        }
        protected void MoveRight() {
            Velocity.X = 1.5f;
        }
        protected void MoveLeft() {
            Velocity.X = -1.5f;
        }


        public void LoadContent(GraphicsDevice g) {

            texture = new Texture2D(g, 1, 1);
            texture.SetData(new Color[] { Color.Red });
        }

        public void Update(GameTime gameTime) {
            UpdatePosition();
            Move();
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime) {

            spriteBatch.Draw(texture, new Rectangle((int)Position.X, (int)Position.Y, (int)Width, (int)Height), Color.White);
        }
    }
}
