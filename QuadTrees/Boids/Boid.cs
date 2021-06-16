using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spacial_Partition.QuadTrees.Boids {
    public class Boid {
        public Vector2 World;
        public int Width;
        public int Height;
        public Rectangle Bounds {
            get {
                return new Rectangle((int)x, (int)y, Width, Height);
            }
        }

        private float x;
        private float y;
        private float dx;
        private float dy;
        public int VisualRange = 75;

        public Boid(Vector2 xy, Vector2 dxdy, Vector2 widthHeight, Vector2 world) {
            x = xy.X;
            y = xy.Y;
            dx = dxdy.X;
            dy = dxdy.Y;
            Width = (int)widthHeight.X;
            Height = (int)widthHeight.Y;
            World = world;
        }


        public double Distance(Boid boid1, Boid boid2) {
            return Math.Sqrt(
              (boid1.x - boid2.x) * (boid1.x - boid2.x) +
                (boid1.y - boid2.y) * (boid1.y - boid2.y)

            );
        }

        // Constrain a boid to within the window. If it gets too close to an edge,
        // nudge it back in and reverse its direction.
        private void KeepWithinBounds(Vector2 screen) {
            int margin = 2;
            int turnFactor = 1;

            if (x < margin) {
                dx += turnFactor;
            }
            if (x > screen.X - margin) {
                dx -= turnFactor;
            }
            if (y < margin) {
                dy += turnFactor;
            }
            if (y > screen.Y - margin) {
                dy -= turnFactor;
            }
        }

        // Find the center of mass of the other boids and adjust velocity slightly to
        // point towards the center of mass.
        private void FlyTowardsCenter(List<Boid> boids) {
            float centeringFactor = 0.005f;

            float centerX = 0;
            float centerY = 0;
            int numNeighbors = 0;

            foreach (Boid otherBoid in boids) {
                if (Distance(this, otherBoid) < VisualRange) {
                    centerX += otherBoid.x;
                    centerY += otherBoid.y;
                    numNeighbors += 1;
                }
            }

            if (numNeighbors > 0) {
                centerX /= numNeighbors;
                centerY /= numNeighbors;

                dx += (centerX - x) * centeringFactor;
                dy += (centerY - y) * centeringFactor;
            }
        }

        // Move away from other boids that are too close to avoid colliding
        private void AvoidOthers(List<Boid> boids) {
            float minDistance = 20f; 
            float avoidFactor = 0.05f;
            float moveX = 0;
            float moveY = 0;
            foreach (Boid otherBoid in boids) {
                if (otherBoid != this) {
                    if (Distance(this, otherBoid) < minDistance) {
                        moveX += x - otherBoid.x;
                        moveY += y - otherBoid.y;
                    }
                }
            }

            dx += moveX * avoidFactor;
            dy += moveY * avoidFactor;
        }

        // Find the average velocity (speed and direction) of the other boids and
        // adjust velocity slightly to match.
        private void MatchVelocity(List<Boid> boids) {
            float matchingFactor = 0.05f;

            float avgDX = 0;
            float avgDY = 0;
            float numNeighbors = 0;

            foreach (Boid otherBoid in boids) {
                if (Distance(this, otherBoid) < VisualRange) {
                    avgDX += otherBoid.dx;
                    avgDY += otherBoid.dy;
                    numNeighbors += 1;
                }
            }

            if (numNeighbors > 0) {
                avgDX /= numNeighbors;
                avgDY /= numNeighbors;

                dx += (avgDX - dx) * matchingFactor;
                dy += (avgDY - dy) * matchingFactor;
            }
        }

        // Speed will naturally vary in flocking behavior, but real animals can't go
        // arbitrarily fast.
        private void LimitSpeed(Boid boid) {
            float speedLimit = 7;

            double speed = Math.Sqrt(boid.dx * boid.dx + boid.dy * boid.dy);
            if (speed > speedLimit) {
                boid.dx = (float)((boid.dx / speed) * speedLimit);
                boid.dy = (float)((boid.dy / speed) * speedLimit);
            }
        }

        public void DoBoidThings(List<Boid> boids) {
            FlyTowardsCenter(boids);
            AvoidOthers(boids);
            MatchVelocity(boids);
            LimitSpeed(this);
            KeepWithinBounds(World);
        }

        public virtual void Update(GameTime gameTime) {
            x += dx;
            y += dy;
        }

        public virtual void Draw(SpriteBatch spriteBatch) {
            Vector2 origin = new Vector2(Width / 2f, Height / 2f);
            float rotation = (float)Math.Atan2(dy, dx);
            SpriteEffects flip = SpriteEffects.None;
            spriteBatch.Begin();
            spriteBatch.Draw(QuadTextures.Boid, Bounds, null, new Color(85, 140, 244), rotation, origin, flip, 0f);
            spriteBatch.End();
        }
    }
}
