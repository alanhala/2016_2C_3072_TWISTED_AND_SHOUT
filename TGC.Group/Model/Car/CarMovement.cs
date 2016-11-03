using System;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using TGC.Core.Input;
using TGC.Core.Particle;

namespace TGC.Group.Model
{
    public class CarMovement
    {
        private readonly float MAX_VELOCITY = 900f;
        private readonly float MAX_REVERSE_VELOCTIY = -500;
        private float acceleration;
        private float brakeDeceleration;
        private float deceleration;
        private float velocity = 0f;

        private Vector3 initialDirection;
        private Vector4 currentDirection = new Vector4();
        private Vector3 relativePosition = new Vector3(0, 0, 0);
        private Vector3 lastRelativePosition;
        private float rotationAngle = (float)Math.PI;
        private float lastRotationAngle;
        
        private bool isJumping;
        private float timeJumping;
        private float jumpVelocity;

        public CarMovement(Vector3 carDirection, float acceleration, float deceleration,
            float brakeDecelartion)
        {
            initialDirection = carDirection;
            this.acceleration = acceleration;
            this.deceleration = deceleration;
            brakeDeceleration = brakeDecelartion;
        }

        public Vector3 getRelativePosition()
        {
            return relativePosition;
        }

        internal Vector3 getDirection()
        {
            return new Vector3(currentDirection.X, currentDirection.Y, currentDirection.Z);
        }

        public void updateCarPosition(TgcD3dInput input, float elapsedTime)
        {
            saveCurrentState();
            setUpJumpAcceleration(input);
            updateVelocity(input, elapsedTime);
            turnCar(input, elapsedTime);
            currentDirection = Vector3.Transform(initialDirection, Matrix.RotationY((float)Math.PI + rotationAngle));
            relativePosition.X += currentDirection.X * velocity * elapsedTime;
            updateYPosition(elapsedTime);
            relativePosition.Z += currentDirection.Z * velocity * elapsedTime;
        }

        private void saveCurrentState()
        {
            lastRelativePosition = relativePosition;
            lastRotationAngle = rotationAngle;
        }

        private void updateYPosition(float elapsedTime)
        {
            if (isJumping)
            {
                timeJumping += elapsedTime;
                relativePosition.Y += jumpVelocity * timeJumping - 50 * 0.5f * timeJumping * timeJumping;
                if (relativePosition.Y < 0)
                {
                    relativePosition.Y = 0;
                    isJumping = false;
                    timeJumping = 0;
                }
            }
        }

        private void setUpJumpAcceleration(TgcD3dInput input)
        {
            if (input.keyDown(Key.Space) && isJumping == false)
            {
                jumpVelocity = 25f;
                isJumping = true;
            }
        }

        internal float getRotationAngle()
        {
            return rotationAngle;
        }

        private void turnCar(TgcD3dInput input, float elapsedTime)
        {
            if (velocity > 20f || velocity < -20f)
            {
                if ((turnsLeft(input) && velocity > 0) || (turnsRight(input) && velocity < 0))
                {
                    rotationAngle -= 1.5f * elapsedTime;
                }
                else if ((turnsRight(input) && velocity > 0) || (turnsLeft(input) && velocity < 0))
                {
                    rotationAngle += 1.5f * elapsedTime;
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
                velocity = Math.Min(velocity + acceleration * elapsedTime, MAX_VELOCITY);
            }
            else if (movesBackwards(input))
            {
                velocity = Math.Max(velocity + brakeDeceleration * elapsedTime, MAX_REVERSE_VELOCTIY);
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

        public void setVelocity(float newVelocity)
        {
            velocity = newVelocity;
        }

        public float getVelocity()
        {
            return velocity;
        }

        public Vector3 getPositionDiff()
        {
            return relativePosition - lastRelativePosition;
        }

        public float getRotationAngleDiff()
        {
            return rotationAngle - lastRotationAngle;
        }

        public void rollbackMovement()
        {
            relativePosition = lastRelativePosition;
            rotationAngle = lastRotationAngle;
        }
    }
}