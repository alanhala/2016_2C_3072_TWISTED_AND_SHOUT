using System;
using System.Drawing;
using System.Drawing.Printing;
using Microsoft.DirectX;
using TGC.Core.Collision;
using TGC.Core.Geometry;
using TGC.Core.Particle;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    public class Bullet
    {
        private TgcBox mesh;
        private bool enabled;
        private Vector3 finalPosition;
        private Vector3 currentPositon;
        private Car target;

        public Bullet()
        {
            mesh = TgcBox.fromSize(new Vector3(6, 6, 6), Color.FromArgb(72, 251, 71));
            mesh.AutoTransformEnable = false;
        }

        public void enable()
        {
            enabled = true;
        }

        public void disable()
        {
            enabled = false;
        }

        public void init(Vector3 rayOrigin, Vector3 newPosition, Car enemy)
        {
            currentPositon = rayOrigin;
            finalPosition = newPosition;
            target = enemy;
        }

        public void render()
        {
            if (!enabled) return;
            mesh.Transform = Matrix.Translation(currentPositon);
            mesh.render();
            mesh.BoundingBox.render();
        }

        public void dispose()
        {
            mesh.dispose();
        }

        public void update(float elapsedTime)
        {
            if (!enabled) return;
            if (TgcCollisionUtils.testObbAABB(target.boundingBox, mesh.BoundingBox))
            {
                disable();
                target.hitWithBullet();
                return;
            }
            var posDiff = finalPosition - currentPositon;
            var posDiffLength = posDiff.LengthSq();
            if (posDiffLength > float.Epsilon)
            {
                var currentVelocity = 2000 * elapsedTime;
                posDiff.Normalize();
                posDiff.Multiply(currentVelocity);
                var newPos = currentPositon + posDiff;
                if (posDiff.LengthSq() > posDiffLength)
                {
                    newPos = finalPosition;
                }
                mesh.BoundingBox.move(newPos - mesh.BoundingBox.Position);
                currentPositon = newPos;
            }
            else
            {
                disable();
            }
        }

        public Vector3 getCurrentPosition()
        {
            return currentPositon;
        }

        public bool isEnabled()
        {
            return enabled;
        }
    }
}