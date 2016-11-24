using System;
using Microsoft.DirectX;
using TGC.Core.Input;
using TGC.Core.Utils;

namespace TGC.Group.Model
{
    public class EnemyMovement : CarMovement
    {
        private CarMovement carMovement;

        public EnemyMovement(CarMovement carMovement) 
            : base(new Vector3(0, 0, 1), 200, -100, -700)
        {
            this.carMovement = carMovement;
        }

        public override void updateCarPosition(TgcD3dInput input, float elapsedTime)
        {
            saveCurrentState();
            var direction = carMovement.getRelativePosition() - getRelativePosition();
            direction.Normalize();
            currentDirection.Normalize();
            currentDirection = Vector3.Transform(initialDirection, Matrix.RotationY((float)Math.PI + rotationAngle));
            var angle = Math.Atan2(currentDirection.Z, currentDirection.X) - Math.Atan2(direction.Z, direction.X);
            if (angle > Math.PI)
            {
                angle -= FastMath.TWO_PI;
            }
            if (angle > 0)
            {
                rotationAngle += 1.5f*elapsedTime;
            }
            else
            {
                rotationAngle -= 1.5f*elapsedTime;
            }
            currentDirection = Vector3.Transform(initialDirection, Matrix.RotationY((float)Math.PI + rotationAngle));
            if (elapsedTime < 5)
            {
                relativePosition.X += currentDirection.X * 100 * elapsedTime;
                relativePosition.Z += currentDirection.Z * 100 * elapsedTime;
            }
        }
    }
}