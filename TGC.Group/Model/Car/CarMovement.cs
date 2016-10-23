using System;
using Microsoft.DirectX;
using TGC.Core.Input;

namespace TGC.Group.Model
{
    public class CarMovement
    {
        private float acceleration;
        private float brakeDeceleration;
        private float deceleration;
        private float rotationAngle = (float)Math.PI;
        private float velocity = 0f;
        private Vector3 initialDirection;
        private Vector3 position = new Vector3(0, 20, 0);
        private Vector4 currentDirection = new Vector4();

        public CarMovement(Vector3 carDirection, float acceleration, float deceleration,
            float brakeDecelartion)
        {
            initialDirection = carDirection;
            this.acceleration = acceleration;
            this.deceleration = deceleration;
            brakeDeceleration = brakeDecelartion;
        }

        internal Vector3 getDirection()
        {
            return new Vector3(currentDirection.X, currentDirection.Y, currentDirection.Z);
        }

        public Vector3 getPosition()
        {
            return position;
        }

        public Matrix move(TgcD3dInput input, float elapsedTime)
        {
            updateVelocity(input, elapsedTime);
            turnCar(input, elapsedTime);
            currentDirection = Vector3.Transform(initialDirection, Matrix.RotationY((float)Math.PI + rotationAngle));
            position.X += currentDirection.X * velocity * elapsedTime;
            position.Y += currentDirection.Y * velocity * elapsedTime;
            position.Z += currentDirection.Z * velocity * elapsedTime;
            return Matrix.RotationY(rotationAngle) * Matrix.Translation(position);
        }

        internal float getRotationAngle()
        {
            return rotationAngle;
        }

        private void turnCar(TgcD3dInput input, float elapsedTime)
        {
            if (velocity > 20f || velocity < -20f)
            {
                if (turnsLeft(input))
                {
                    rotationAngle -= 2f * elapsedTime;
                }
                else if (turnsRight(input))
                {
                    rotationAngle += 2f * elapsedTime;
                }
            }
        }

        private bool turnsRight(TgcD3dInput input)
        {
            return input.keyDown(Microsoft.DirectX.DirectInput.Key.Right);
        }

        private bool turnsLeft(TgcD3dInput input)
        {
            return input.keyDown(Microsoft.DirectX.DirectInput.Key.Left);
        }

        private void updateVelocity(TgcD3dInput input, float elapsedTime)
        {
            if (movesForward(input))
            {
                velocity = velocity + acceleration * elapsedTime;
            }
            else if (movesBackwards(input))
            {
                velocity = velocity + brakeDeceleration * elapsedTime;
            }
            else
            {
                decelerate(elapsedTime);
            }
        }

        private void decelerate(float elapsedTime)
        {
            if (velocity < 0.25f && velocity > -0.25f)
            {
                velocity = 0f;
            }
            else if (velocity > 0)
            {
                velocity = velocity + deceleration * elapsedTime;
            }
            else
            {
                velocity = velocity - deceleration * elapsedTime;
            }
        }

        private bool movesBackwards(TgcD3dInput input)
        {
            return input.keyDown(Microsoft.DirectX.DirectInput.Key.Down);
        }

        private bool movesForward(TgcD3dInput input)
        {
            return input.keyDown(Microsoft.DirectX.DirectInput.Key.Up);
        }

        public void setPosition(Vector3 newPosition)
        {
            position = newPosition;
        }

        public void setVelocity(float newVelocity)
        {
            velocity = newVelocity;
        }
    }
}