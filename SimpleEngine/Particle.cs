using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace CPI411.SimpleEngine
{
    public class Particle
    {
        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }
        public Vector3 Acceleration { get; set; }
        public Vector3 WindForce { get; set; }
        public float Age { get; set; }
        public float MaxAge { get; set; }
        public Vector3 Color { get; set; }
        public float Size { get; set; }
        public float SizeVelocity { get; set; }
        public float SizeAcceleration { get; set; }
        public bool Bounce { get; set; }
        public float Friction { get; set; }
        public float Resilience { get; set; }
        public Particle() { Age = -1; Bounce = false; }
        private Vector3 initialVel;
        private bool initVel = true;
        public bool Update(float ElapsedGameTime)
        {
            if (Age < 0) return false;
            if (Bounce && initVel)
            {
                initVel = false;
                initialVel = Velocity;
            }
            Velocity += Acceleration * ElapsedGameTime;
            Position += Velocity * ElapsedGameTime;
            SizeVelocity += SizeAcceleration * ElapsedGameTime;
            Size += SizeVelocity * ElapsedGameTime;
            Age += ElapsedGameTime;
            Velocity += WindForce;
            if (Bounce && Position.Y <= 0)
            {
                if (initialVel.X > 0)
                {
                    initialVel.X -= Friction;
                } else
                {
                    initialVel.X += Friction;
                }
                if (initialVel.Z > 0)
                {
                    initialVel.Z -= Friction;
                }
                else
                {
                    initialVel.Z += Friction;
                }
                if (initialVel.Y > 0)
                {
                    initialVel.Y -= Resilience;
                }
                else
                {
                    initialVel.Y += Resilience;
                }
                Velocity = initialVel;
            }
            if (Age > MaxAge)
            {
                Age = -1;
                return false;
            }
            return true;
        }
        public bool IsActive() { return Age < 0 ? false : true; }
        public void Activate() { Age = 0; }
        public void Init()
        {
            Age = 0; Size = 1; SizeVelocity = SizeAcceleration = 0;
        }
    }
}
