using System.Drawing;
using System.Drawing.Printing;
using Microsoft.DirectX;
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

        public void init(Vector3 rayOrigin, Vector3 newPosition)
        {
            currentPositon = rayOrigin;
            finalPosition = newPosition;
        }

        public void render()
        {
            if (!enabled) return;
            mesh.Transform = Matrix.Translation(currentPositon);
            mesh.render();
        }

        public void dispose()
        {
            mesh.dispose();
        }

        public void update(float elapsedTime)
        {
            if (!enabled) return;
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