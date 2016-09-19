using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Example;
using TGC.Core.Input;
using TGC.Core.SceneLoader;
using TGC.Core.Utils;

namespace TGC.Group.Model
{
    class Car
    {
        private float velocity;
        private float acceleration;
        private float deceleration;
        private float brakeAcceleration;
        private Vector3 position;
        private Matrix translation;
        private TgcMesh mesh;

        public Car()
        {
            var loader = new TgcSceneLoader();
            mesh = loader.loadSceneFromFile(Game.Default.MediaDirectory
                + "Auto\\Auto-TgcScene.xml").Meshes[0];
            mesh.AutoTransformEnable = false;
            initializeMovement();
        }

        private void initializeMovement()
        {
            acceleration = 20;
            deceleration = -10;
            brakeAcceleration = -20;
            translation = Matrix.Identity;
            position = new Vector3(0, 0, 0);
        }

        public void move(TgcD3dInput input, float elapsedTime)
        {
            if (input.keyDown(Microsoft.DirectX.DirectInput.Key.Up))
            {
                velocity = velocity + acceleration * elapsedTime;
            } else if (input.keyDown(Microsoft.DirectX.DirectInput.Key.Down))
            {
                velocity = velocity + brakeAcceleration * elapsedTime;
            } else
            {
                decelerate(elapsedTime);
            }
            position.Z -= velocity * elapsedTime;
            mesh.Transform = Matrix.Translation(position) * Matrix.RotationY(-FastMath.PI_HALF);
        }

        private void decelerate(float elapsedTime)
        {
            if (velocity <= 0)
            {
                velocity = 0;
            } else
            {
                velocity = velocity + deceleration * elapsedTime;
            }
        }

        public void render()
        {
            mesh.render();
        }

        public void dispose()
        {
            mesh.dispose();
        }
    }
}
